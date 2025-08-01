using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SylhShrinkerCartPlus.Components;
using UnityEngine;
using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;
using SylhShrinkerCartPlus.Utils;
using UnityEngine.SceneManagement;

namespace SylhShrinkerCartPlus
{
    [BepInPlugin(mod_guid, mod_name, mod_version)]
    public class Plugin : BaseUnityPlugin
    {
        private const string mod_guid = "sylhaance.SylhShrinkerCartPlus";
        private const string mod_name = "Sylh Shrinker Cart Plus";
        private const string mod_version = "0.2.0";

        private Harmony harmony;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("[SylhShrinkerCartPlus] Plugin loaded.");

            ConfigManager.Initialize(this);
            
            CategoryResolverRegistry.Register(new EnemyCategoryResolver());
            CategoryResolverRegistry.Register(new StandardValuableCategoryResolver());

            harmony = new Harmony(mod_guid);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(PhysGrabCart), "ObjectsInCart")]
    public static class ShrinkerCartPatch
    {
        // private static readonly AccessTools.FieldRef<PhysGrabCart, List<PhysGrabObject>> itemsInCartRef =
        // AccessTools.FieldRefAccess<PhysGrabCart, List<PhysGrabObject>>("itemsInCart");

        private static readonly HashSet<PhysGrabObject> objectsToShrink = new();

        private static readonly Dictionary<PhysGrabCart, CartShrinkState> cartStates = new();

        private static readonly Dictionary<PhysGrabObject, Vector3> originalScales = new();
        private static float shrinkSpeed = ConfigManager.defaultShrinkSpeed.Value;
        private static float minScale = 0.20f;

        static ShrinkerCartPatch()
        {
            SceneManager.sceneLoaded += (_, _) =>
            {
                LogWrapper.Info("[Sylh ShrinkerCartPlus] New scene loaded — clearing lists.");
                originalScales.Clear();
                objectsToShrink.Clear();
            };
        }

        public static void Postfix(PhysGrabCart __instance)
        {
            CleanupCartStates();

            if (!cartStates.TryGetValue(__instance, out var state))
            {
                state = new CartShrinkState();
                cartStates[__instance] = state;
            }

            var cartHelper = new FastReflectionHelper<PhysGrabCart>(__instance);
            if (!cartHelper.TryGetField("itemsInCart", out List<PhysGrabObject> items) || items == null)
                return;

            state.ObjectsToShrink.RemoveWhere(obj => obj == null || !items.Contains(obj));


            foreach (var item in items)
            {
                if (ConfigManager.shouldInstantKillEnemyInCart.Value)
                {
                    EnemyExecutionManager.TryMarkForExecution(item);
                }

                ApplyBatteryLifeOnce(item, state);
                ApplyUnbreakableLogic(item, state);

                if (item == null || !item.GetComponent<ValuableObject>())
                    continue;

                if (!ShrinkOperationManager.TryAssign(item, __instance))
                {
                    LogWrapper.Warning($"[ShrinkControl] Object {item.name} is already being scaled by another cart.");
                    continue;
                }

                if (!state.OriginalScales.ContainsKey(item))
                {
                    var tracker = item.GetComponent<OriginalScaleTracker>() ??
                                  item.gameObject.AddComponent<OriginalScaleTracker>();
                    tracker.InitializeIfNeeded();

                    state.OriginalScales[item] = tracker.InitialScale;
                    state.ObjectsToShrink.Add(item);

                    LogWrapper.Debug(
                        $"[ShrinkControl] Registered original scale for {item.name}: {tracker.InitialScale}");
                }
            }

            List<PhysGrabObject> completedShrinks = new();

            foreach (var item in state.ObjectsToShrink)
            {
                if (item == null || !state.OriginalScales.ContainsKey(item))
                    continue;

                ShrinkData data = ShrinkUtils.GetShrinkData(item);
                Vector3 original = state.OriginalScales[item];

                Vector3 target = Vector3.Max(original * data.ScaleShrinkFactor, original * data.MinShrinkRatio);
                target = Vector3.Min(target, original);
                
                PhysGrabObjectImpactDetector detector = item.GetComponent<PhysGrabObjectImpactDetector>();
                detector.destroyDisable = true;

                item.transform.localScale =
                    Vector3.MoveTowards(item.transform.localScale, target, shrinkSpeed * Time.deltaTime);

                if (Vector3.Distance(item.transform.localScale, target) < 0.01f)
                {
                    if (ConfigManager.shouldChangingMass.Value)
                    {
                        ShrinkMassUtils.ApplyShrinkedMass(item, data);
                    }

                    completedShrinks.Add(item);
                    LogWrapper.Warning($"Shrink {item.name} → original: {original}, target: {target}, final scale ratio: {target.x / original.x:0.00}, dimensions: {data.Dimensions}");
                    LogWrapper.Info($"[ShrinkQueue] Adding '{item.name}' to shrink list.");
                }
            }

            foreach (var item in completedShrinks)
            {
                state.ObjectsToShrink.Remove(item);
                ShrinkOperationManager.Release(item);
            }

            // Restore logic
            if (!ConfigManager.shouldKeepShrunk.Value)
            {
                List<PhysGrabObject> toRestore = new();
                foreach (var kvp in state.OriginalScales)
                {
                    var obj = kvp.Key;
                    if (obj == null || items.Contains(obj)) continue;

                    Vector3 original = kvp.Value;

                    obj.transform.localScale =
                        Vector3.MoveTowards(obj.transform.localScale, original, shrinkSpeed * Time.deltaTime);

                    if (Vector3.Distance(obj.transform.localScale, kvp.Value) < 0.01f)
                    {
                        if (ConfigManager.shouldChangingMass.Value)
                        {
                            ShrinkMassUtils.RestoreOriginalMass(obj);
                        }

                        toRestore.Add(obj);
                    }

                    // Gestion des objets incassables à vie ou non
                    var detector = obj.GetComponent<PhysGrabObjectImpactDetector>();
                    if (detector != null)
                    {
                        if (ConfigManager.shouldValuableStayUnbreakable.Value &&
                            state.MarkedUnbreakableObjects.Contains(obj))
                        {
                            detector.destroyDisable = true;
                        }
                        else
                        {
                            detector.destroyDisable = false;
                        }
                    }
                }

                foreach (var obj in toRestore)
                {
                    state.OriginalScales.Remove(obj);
                    state.ModifiedBatteryLifeObjects.Remove(obj);
                    ShrinkOperationManager.Release(obj);
                }
            }
        }

        private static void CleanupCartStates()
        {
            List<PhysGrabCart> cartsToRemove = new();

            foreach (var kvp in cartStates)
            {
                var cart = kvp.Key;
                var state = kvp.Value;

                if (cart == null || !cart.gameObject || cart.gameObject.scene.name == null)
                {
                    state.OriginalScales.Clear();
                    state.ObjectsToShrink.Clear();
                    state.ModifiedBatteryLifeObjects.Clear();
                    state.MarkedUnbreakableObjects.Clear();

                    cartsToRemove.Add(cart);
                }
            }

            foreach (var cart in cartsToRemove)
            {
                cartStates.Remove(cart);
                LogWrapper.Info($"[CartCleanup] Removed destroyed cart '{cart?.name}' from tracking.");

                ShrinkOperationManager.ClearCart(cart);
            }
        }

        private static void ApplyBatteryLifeOnce(PhysGrabObject item, CartShrinkState state)
        {
            if (state.ModifiedBatteryLifeObjects.Contains(item)) return;

            bool modified = false;

            if (ConfigManager.shouldCartWeaponBatteryLifeInfinite.Value &&
                ItemCartWeaponUtils.TryChangeCartWeaponBatteryLife(item))
            {
                modified = true;
            }

            if (ConfigManager.shouldItemMeleeBatteryLifeInfinite.Value)
            {
                ItemBatteryUtils.SetMeleeBatteryLife(item);
                modified = true;
            }

            if (ConfigManager.shouldItemGunBatteryLifeInfinite.Value)
            {
                ItemBatteryUtils.SetGunBatteryLife(item);
                modified = true;
            }

            if (ConfigManager.shouldItemDroneBatteryLifeInfinite.Value)
            {
                ItemBatteryUtils.SetDroneBatteryLife(item);
                modified = true;
            }

            if (modified)
            {
                state.ModifiedBatteryLifeObjects.Add(item);
                LogWrapper.Info($"[BatteryLife] Applied infinite battery to: {item.name}");
            }
        }

        private static void ApplyUnbreakableLogic(PhysGrabObject item, CartShrinkState state)
        {
            if (item == null) return;

            var detector = item.GetComponent<PhysGrabObjectImpactDetector>();
            if (detector == null) return;

            // Si le config est active, applique le flag dans le cart
            if (ConfigManager.shouldValuableSafeInsideCart.Value)
            {
                detector.destroyDisable = true;

                // Si on doit le rendre permanent
                if (ConfigManager.shouldValuableStayUnbreakable.Value)
                {
                    if (!state.MarkedUnbreakableObjects.Contains(item))
                    {
                        state.MarkedUnbreakableObjects.Add(item);
                        LogWrapper.Info($"[Unbreakable] Marked {item.name} as permanently unbreakable.");
                    }
                }
            }

            // Si jamais il a été marqué pour être permanent, on continue à forcer même après la sortie du cart
            if (ConfigManager.shouldValuableStayUnbreakable.Value && state.MarkedUnbreakableObjects.Contains(item))
            {
                detector.destroyDisable = true;
            }
        }
    }
}