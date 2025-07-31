using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;
using UnityEngine;

namespace SylhShrinkerCartPlus.Utils
{
    public class ShrinkUtils
    {
        private static float initialShrinkFactor = 1.0f;

        public static ShrinkData GetShrinkData(
            PhysGrabObject item,
            ShrinkSpecificCaseData data
        )
        {
            string itemName = NameUtils.CleanName(item.name);

            if (NameUtils.TryParseEnemyValuable(itemName, out string _, out string enemyType))
            {
                if (ConfigManager.shouldShrinkEnemyOrbs.Value)
                {
                    data.ShrinkFactor = enemyType switch
                    {
                        "Small" => ConfigManager.shrinkEnemyOrbSmall.Value,
                        "Medium" => ConfigManager.shrinkEnemyOrbMedium.Value,
                        "Big" => ConfigManager.shrinkEnemyOrbBig.Value,
                        _ => 0.50f
                    };
                }
                else
                {
                    data.ShrinkFactor = initialShrinkFactor;
                }
            }
            else
            {
                foreach (LevelValuables levelValuables in RunManager.instance.levelCurrent.ValuablePresets)
                {
                    if (levelValuables.tiny.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkTiny.Value)
                            ? ConfigManager.shrinkFactorTiny.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.small.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkSmall.Value)
                            ? ConfigManager.shrinkFactorSmall.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.medium.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkMedium.Value)
                            ? ConfigManager.shrinkFactorMedium.Value
                            : initialShrinkFactor;
                        break;
                    }

                    if (levelValuables.big.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkBig.Value)
                            ? ConfigManager.shrinkFactorBig.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.wide.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkWide.Value)
                            ? ConfigManager.shrinkFactorWide.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.tall.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkTall.Value)
                            ? ConfigManager.shrinkFactorTall.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.veryTall.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        data.ShrinkFactor = (ConfigManager.shouldShrinkVeryTall.Value)
                            ? ConfigManager.shrinkFactorVeryTall.Value
                            : initialShrinkFactor;

                        break;
                    }
                }

                data = ApplySpecificShrinkRules(item, data);
            }

            return new ShrinkData(
                item.name,
                data.ShrinkFactor,
                item.transform.localScale,
                item.massOriginal,
                data.ShrinkFactor
            );
        }

        private static readonly List<(Func<string, bool> match, Func<float> factor)> SpecificShrinkRules = new()
        {
            (name => NameUtils.ContainsIgnoreCase(name, "Treasure Chest"),
                () => ConfigManager.shouldShrinkMedium.Value ? 0.10f : 0.25f),

            // Ajoute d’autres règles ici
        };

        public static ShrinkSpecificCaseData ApplySpecificShrinkRules(
            PhysGrabObject item,
            ShrinkSpecificCaseData data
        )
        {
            string itemName = NameUtils.CleanName(item.name);

            foreach (var rule in SpecificShrinkRules)
            {
                if (rule.match(itemName))
                {
                    data.ShrinkFactor = rule.factor();
                    data.MinScale = data.ShrinkFactor;
                    return data;
                }
            }

            return data;
        }
    }
}