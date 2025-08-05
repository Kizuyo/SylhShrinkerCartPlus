using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Manager;
using SylhShrinkerCartPlus.Models;
using SylhShrinkerCartPlus.Resolver.Valuable;
using SylhShrinkerCartPlus.Utils;
using SylhShrinkerCartPlus.Utils.Events;
using SylhShrinkerCartPlus.Utils.RunManagerUtils;
using SylhShrinkerCartPlus.Utils.Shrink;
using SylhShrinkerCartPlus.Utils.Shrink.Config;
using SylhShrinkerCartPlus.Utils.Shrink.Network;

namespace SylhShrinkerCartPlus
{
    [BepInPlugin(mod_guid, mod_name, mod_version)]
    public class Plugin : BaseUnityPlugin
    {
        private const string mod_guid = "sylhaance.SylhShrinkerCartPlus";
        private const string mod_name = "Sylh Shrinker Cart Plus";
        private const string mod_version = "0.4.1";

        private Harmony harmony;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("[SylhShrinkerCartPlus] Plugin loaded.");

            ConfigManager.Initialize(this);
            ConfigEvents.Initialize();

            CategoryResolverRegistry.Register(new EnemyCategoryResolver());
            CategoryResolverRegistry.Register(new StandardValuableCategoryResolver());
            CategoryResolverRegistry.Register(new SpecialCategoryResolver());

            harmony = new Harmony(mod_guid);
            harmony.PatchAll();

            ShrinkEvents.OnShrinkStarted += (tracker) =>
            {
                LogWrapper.Warning($"⏳ [Event Hook] Début du rétrécissement pour {tracker.name} !");
                NetworkHelper.ProcessingChangingMass(tracker);
            };

            ShrinkEvents.OnShrinkCompleted += (tracker) =>
            {
                LogWrapper.Warning($"🎉 [Event Hook] Fin du rétrécissement pour {tracker.name} !");
            };

            ShrinkEvents.OnExpandStarted += (tracker) =>
            {
                LogWrapper.Warning($"⏳ [Event Hook] Début de l'agrandissement pour {tracker.name} !");
                NetworkHelper.ProcessingRestoringMass(tracker);
            };

            ShrinkEvents.OnExpandCompleted += (tracker) =>
            {
                LogWrapper.Warning($"🎉 [Event Hook] Fin de l'agrandissement pour {tracker.name} !");
            };

            ShrinkEvents.OnEnteredCart += (tracker) =>
            {
                LogWrapper.Warning($"📥 [Event Hook] {tracker.name} vient d'entrer dans un CART !");
                NetworkHelper.ProcessingChangingBatteryLife(tracker);
                
                // ShrinkUnbreakableUtils.ApplyUnbreakableLogic(tracker);
            };

            ShrinkEvents.OnExitedCart += (tracker) =>
            {
                LogWrapper.Warning($"📤 [Event Hook] {tracker.name} vient de sortir d’un CART !");
            };

            ShrinkEvents.OnMassChanged += (obj, newMass) =>
            {
                LogWrapper.Warning($"⚖️ [Event Hook] {obj.name} → masse changée à {newMass:F2}");
            };

            CartEvents.OnCartObjectAdded += (cart, obj) =>
            {
                LogWrapper.Warning($"[CartEventBus] 🧲 Objet {obj.name} ajouté dans le cart : {cart.name}");
                CartEvents.RaiseEnterCart(obj, cart);
            };
        }
    }

    [HarmonyPatch(typeof(PhysGrabInCart), nameof(PhysGrabInCart.Add))]
    public static class PhysGrabInCartAddPatch
    {
        static void Postfix(PhysGrabInCart __instance, PhysGrabObject _physGrabObject)
        {
            if (!RunManagerHelper.IsInsideValidLevel())
            {
                return;
            }

            CartEvents.FireCartObjectAdded(__instance, _physGrabObject);
        }
    }

    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector), "Start")]
    public static class Patch_PhysGrabObjectImpactDetector_Start
    {
        static void Postfix(PhysGrabObjectImpactDetector __instance)
        {
            if (!RunManagerHelper.IsInsideValidLevel())
            {
                return;
            }
            
            __instance.onDestroy.AddListener(() =>
            {
                var physGrabObject = __instance.GetComponentInChildren<PhysGrabObject>();
                if (physGrabObject == null)
                {
                    Debug.LogWarning("[ShrinkerPlus] Aucun PhysGrabObject trouvé pour destruction.");
                    return;
                }
                
                ShrinkableTracker tracker = __instance.gameObject.GetComponent<ShrinkableTracker>();
                if (tracker == null) return;
                
                string cartName = tracker.CurrentCart.name;

                ShrinkTrackerManager.Instance.UnregisterShrinkCompletion(tracker.CurrentCart, tracker.GrabObject);
                ShrinkTrackerManager.Instance.Unregister(tracker);
                
                Debug.Log($"[ShrinkerPlus] 🧹 '{physGrabObject.name}' supprimé des listes de '{cartName}'.");
            });
        }
    }

    [HarmonyPatch(typeof(PhysGrabObject), "DestroyPhysGrabObjectRPC")]
    public static class DestroyPhysGrabObjectRPCPatch
    {
        public static void Prefix(PhysGrabObject __instance)
        {
            if (!RunManagerHelper.IsInsideValidLevel())
            {
                return;
            }
            
            ShrinkableTracker tracker = __instance.gameObject.GetComponent<ShrinkableTracker>();
            
            ShrinkTrackerManager.Instance.UnregisterShrinkCompletion(tracker.CurrentCart, tracker.GrabObject);
            ShrinkTrackerManager.Instance.Unregister(tracker);
            
            LogWrapper.Warning(
                $"L'objet {__instance.name} vient de subir une destruction par le biais d'un autre méthode que subir des dégâts");
        }
    }

    [HarmonyPatch(typeof(PhysGrabObject), "Start")]
    public static class PhysGrabObjectAwakePatch
    {
        [HarmonyPostfix]
        public static void PostFix(PhysGrabObject __instance)
        {
            try
            {
                if (!RunManagerHelper.IsInsideValidLevel())
                {
                    return;
                }
                
                var tracker = __instance.GetComponent<ShrinkableTracker>();
                if (tracker == null)
                {
                    tracker = __instance.gameObject.AddComponent<ShrinkableTracker>();
                    tracker.Init(__instance);
                }
                
                // ✅ Ajout du PhotonView si manquant
                if (__instance.GetComponent<PhotonView>() == null)
                {
                    var pv = __instance.gameObject.AddComponent<PhotonView>();
                    pv.ViewID = 0; // Laisse Photon s'en charger si tu n’as pas de gestion manuelle
                    LogWrapper.Warning($"[StartPatch] 🛰️ PhotonView ajouté à {__instance.name}");
                }

                // ✅ Ajout du dispatcher réseau si manquant
                if (__instance.GetComponent<ShrinkNetworkDispatcher>() == null)
                {
                    __instance.gameObject.AddComponent<ShrinkNetworkDispatcher>();
                    LogWrapper.Warning($"[StartPatch] 🔌 Dispatcher réseau ajouté à {__instance.name}");
                }

                LogWrapper.Warning($"[StartPatch] ✅ Tracker ajouté à {__instance.name}");
            }
            catch (Exception ex)
            {
                LogWrapper.Error($"[StartPatch] ❌ Erreur pendant le patch Start : {ex}");
            }
        }
    }

    [HarmonyPatch(typeof(PhysGrabCart), "ObjectsInCart")]
    public static class ShrinkerCartPatch
    {
        static ShrinkerCartPatch()
        {
            SceneManager.sceneLoaded += (_, _) =>
            {
                LogWrapper.Info("[Sylh ShrinkerCartPlus] New scene loaded — clearing lists.");
                ShrinkTrackerManager.Instance.ClearAll();
            };
        }

        public static void Postfix(PhysGrabCart __instance)
        {
            if (!RunManagerHelper.IsInsideValidLevel())
            {
                return;
            }

            var cartHelper = new FastReflectionHelper<PhysGrabCart>(__instance);
            if (!cartHelper.TryGetField("itemsInCart", out List<PhysGrabObject> items) || items == null)
                return;

            var newItems = new HashSet<PhysGrabObject>(items);
            var oldItems = new HashSet<PhysGrabObject>(
                ShrinkTrackerManager.Instance.GetCompletedShrunkObjects(__instance)
                );

            foreach (var entered in newItems.Except(oldItems))
            {
                var tracker = GetTracker(entered);
                if (tracker == null) continue;
            
                tracker.PreviousCart = tracker.CurrentCart;
                tracker.CurrentCart = __instance;
            
                tracker.InterruptExpanding();
                if (tracker.IsValidShrinkableItem())
                {
                    ShrinkData data = ShrinkUtils.GetShrinkData(tracker.GrabObject);

                    Vector3 original = tracker.InitialScale;
                    Vector3 target = SetTargetShrink(original, data);
                    data.Target = target;

                    NetworkHelper.ProcessingShrinking(tracker, data);
                }
            
                ShrinkEvents.RaiseEnterCart(tracker);
                ShrinkTrackerManager.Instance.RegisterShrinkCompletion(__instance, tracker.GrabObject);
            }

            // ➖ Objets qui sont sortis du cart
            foreach (var exited in oldItems.Except(newItems))
            {
                var tracker = GetTracker(exited);
                if (tracker == null) continue;

                tracker.PreviousCart = tracker.CurrentCart;
                tracker.ClearCart();

                if (!ConfigManager.shouldKeepShrunk.Value)
                {
                    if (tracker.IsExpandable() && tracker.IsValidShrinkableItem())
                    {
                        NetworkHelper.ProcessingExpanding(tracker);
                    }
                }

                ShrinkEvents.RaiseExitCart(tracker);
                ShrinkTrackerManager.Instance.UnregisterShrinkCompletion(__instance, tracker.GrabObject);
            }
        }

        public static ShrinkableTracker GetTracker(PhysGrabObject obj)
        {
            if (obj == null || obj.gameObject == null)
                return null;

            var tracker = obj.GetComponent<ShrinkableTracker>();
            if (tracker == null)
            {
                tracker = obj.gameObject.AddComponent<ShrinkableTracker>();
                tracker.Init(obj);
            }

            return tracker;
        }

        public static Vector3 SetTargetShrink(
            Vector3 original,
            ShrinkData data
        )
        {
            Vector3 target = Vector3.Max(original * data.ScaleShrinkFactor, original * data.MinShrinkRatio);
            target = Vector3.Min(target, original);

            return target;
        }
    }
}