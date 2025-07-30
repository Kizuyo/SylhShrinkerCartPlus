using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
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
        private const string mod_version = "0.0.8";

        private Harmony harmony;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("[Sylh ShrinkerCartPlus] Plugin loaded.");

            ConfigManager.Initialize(this);

            harmony = new Harmony(mod_guid);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(PhysGrabCart), "ObjectsInCart")]
    public static class ShrinkerCartPatch
    {
        private static readonly AccessTools.FieldRef<PhysGrabCart, List<PhysGrabObject>> itemsInCartRef =
            AccessTools.FieldRefAccess<PhysGrabCart, List<PhysGrabObject>>("itemsInCart");

        private static readonly HashSet<PhysGrabObject> objectsToShrink = new();

        private static readonly Dictionary<PhysGrabCart, CartShrinkState> cartStates = new();

        private static readonly Dictionary<PhysGrabObject, Vector3> originalScales = new();
        private static float shrinkSpeed = ConfigManager.defaultShrinkSpeed.Value;
        private static float minScale = 0.20f;

        static ShrinkerCartPatch()
        {
            SceneManager.sceneLoaded += (_, _) =>
            {
                Plugin.Log.LogInfo("[Sylh ShrinkerCartPlus] New scene loaded — clearing lists.");
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

            var items = itemsInCartRef(__instance);
            if (items == null) return;

            state.ObjectsToShrink.RemoveWhere(obj => obj == null || !items.Contains(obj));

            foreach (var item in items)
            {
                if (item == null || !item.GetComponent<ValuableObject>())
                    continue;

                if (!state.OriginalScales.ContainsKey(item))
                {
                    state.OriginalScales[item] = item.transform.localScale;
                    state.ObjectsToShrink.Add(item);
                }
            }

            List<PhysGrabObject> completedShrinks = new();

            foreach (var item in state.ObjectsToShrink)
            {
                if (item == null || !state.OriginalScales.ContainsKey(item))
                    continue;

                ShrinkData data = ShrinkUtils.GetShrinkData(item);
                Vector3 original = state.OriginalScales[item];
                Vector3 target = Vector3.Max(original * data.ShrinkFactor, Vector3.one * minScale);

                item.transform.localScale =
                    Vector3.MoveTowards(item.transform.localScale, target, shrinkSpeed * Time.deltaTime);

                if (Vector3.Distance(item.transform.localScale, target) < 0.01f)
                {
                    if (ConfigManager.shouldChangingMass.Value)
                    {
                        ShrinkMassUtils.ApplyShrinkedMass(item, data);
                    }

                    completedShrinks.Add(item);
                }
            }

            foreach (var item in completedShrinks)
                state.ObjectsToShrink.Remove(item);

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
                }

                foreach (var obj in toRestore)
                    state.OriginalScales.Remove(obj);
            }
        }

        private static void CleanupCartStates()
        {
            List<PhysGrabCart> cartsToRemove = new();

            foreach (var kvp in cartStates)
            {
                var cart = kvp.Key;
                if (cart == null || !cart.gameObject || cart.gameObject.scene.name == null)
                {
                    cartsToRemove.Add(cart);
                }
            }

            foreach (var cart in cartsToRemove)
            {
                cartStates.Remove(cart);
                Plugin.Log.LogInfo($"[Sylh ShrinkerCartPlus] Removed destroyed cart from tracking.");
            }
        }
    }
}