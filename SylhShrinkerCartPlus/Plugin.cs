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
            var items = itemsInCartRef(__instance);
            if (items == null) return;

            objectsToShrink.RemoveWhere(obj => obj == null || !items.Contains(obj));

            foreach (var item in items)
            {
                if (item == null || !item.GetComponent<ValuableObject>())
                    continue;

                if (!originalScales.ContainsKey(item))
                {
                    originalScales[item] = item.transform.localScale;
                    objectsToShrink.Add(item);
                }
            }

            List<PhysGrabObject> completedShrinks = new();

            foreach (var item in objectsToShrink)
            {
                if (item == null || !originalScales.ContainsKey(item))
                    continue;

                ShrinkData data = ShrinkUtils.GetShrinkData(item);
                Vector3 original = originalScales[item];
                Vector3 target = Vector3.Max(original * data.ShrinkFactor, Vector3.one * minScale);

                item.transform.localScale =
                    Vector3.MoveTowards(item.transform.localScale, target, shrinkSpeed * Time.deltaTime);

                if (Vector3.Distance(item.transform.localScale, target) < 0.01f)
                    completedShrinks.Add(item);
            }

            foreach (var item in completedShrinks)
                objectsToShrink.Remove(item);

            // Optional: restore objects removed from the cart
            List<PhysGrabObject> toRestore = new();
            foreach (var kvp in originalScales)
            {
                var obj = kvp.Key;
                if (obj == null || items.Contains(obj)) continue;

                obj.transform.localScale =
                    Vector3.MoveTowards(obj.transform.localScale, kvp.Value, shrinkSpeed * Time.deltaTime);

                if (Vector3.Distance(obj.transform.localScale, kvp.Value) < 0.01f)
                    toRestore.Add(obj);
            }

            foreach (var obj in toRestore)
                originalScales.Remove(obj);
        }
    }

    [HarmonyPatch(typeof(EnemyHealth))]
    public static class EnemyValuablePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Awake")]
        public static void UpdateSpawnValuableMax(EnemyHealth __instance)
        {
            __instance.spawnValuableMax = ConfigManager.maxSpawnableOrb.Value;
        }
    }
}