using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;
using UnityEngine.SceneManagement;

namespace SylhShrinkerCartPlus
{
    [BepInPlugin("sylhaance.SylhShrinkerCartPlus", "Sylh Shrinker Cart Plus", "0.0.7")]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony harmony;
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("[Sylh ShrinkerCartPlus] Plugin loaded.");

            ConfigManager.Initialize(this);

            harmony = new Harmony("sylhaance.SylhShrinkerCartPlus");
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
        private static float initialShrinkFactor = 1.0f;

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

                ShrinkData data = GetShrinkData(item);
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

        private static ShrinkData GetShrinkData(PhysGrabObject item)
        {
            string itemName = CleanName(item.name);
            float shrinkFactor = 0.20f;

            if (TryParseEnemyValuable(itemName, out string _, out string enemyType))
            {
                if (ConfigManager.shouldShrinkEnemyOrbs.Value)
                {
                    shrinkFactor = enemyType switch
                    {
                        "Small" => 1.0f,
                        "Medium" => 0.50f,
                        "Big" => 0.30f,
                        _ => 0.50f
                    };
                }
                else
                {
                    shrinkFactor = initialShrinkFactor;
                }
            }
            else
            {
                foreach (LevelValuables levelValuables in RunManager.instance.levelCurrent.ValuablePresets)
                {
                    if (levelValuables.tiny.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkTiny.Value)
                            ? ConfigManager.shrinkFactorTiny.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.small.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkSmall.Value)
                            ? ConfigManager.shrinkFactorSmall.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.medium.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkMedium.Value)
                            ? ConfigManager.shrinkFactorMedium.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.big.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkBig.Value)
                            ? ConfigManager.shrinkFactorBig.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.wide.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkWide.Value)
                            ? ConfigManager.shrinkFactorWide.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.tall.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkTall.Value)
                            ? ConfigManager.shrinkFactorTall.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.veryTall.Exists(v => CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkVeryTall.Value)
                            ? ConfigManager.shrinkFactorVeryTall.Value
                            : initialShrinkFactor;

                        break;
                    }
                }
            }
            
            return new ShrinkData(
                item.name,
                shrinkFactor,
                item.transform.localScale,
                item.massOriginal
            );
        }

        private static string CleanName(string name)
        {
            return name.Replace("Valuable ", "").Replace("(Clone)", "").Trim();
        }

        private static bool TryParseEnemyValuable(
            string name,
            out string baseName,
            out string type
        )
        {
            baseName = null;
            type = null;

            // Clean the name first
            string cleanName = CleanName(name);

            if (!cleanName.StartsWith("Enemy")) return false;

            var parts = cleanName.Split('-');
            if (parts.Length < 2) return false;

            baseName = parts[0].Trim(); // "Enemy"
            type = parts[1].Trim(); // "Big", "Medium", or "Small"

            return true;
        }
    }
}