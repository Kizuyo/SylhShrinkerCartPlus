using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;

namespace SylhShrinkerCartPlus.Utils
{
    public class ShrinkUtils
    {
        private static float initialShrinkFactor = 1.0f;
        
        public static ShrinkData GetShrinkData(PhysGrabObject item)
        {
            string itemName = NameUtils.CleanName(item.name);
            float shrinkFactor = 0.20f;

            if (NameUtils.TryParseEnemyValuable(itemName, out string _, out string enemyType))
            {
                if (ConfigManager.shouldShrinkEnemyOrbs.Value)
                {
                    shrinkFactor = enemyType switch
                    {
                        "Small" => ConfigManager.shrinkEnemyOrbSmall.Value,
                        "Medium" => ConfigManager.shrinkEnemyOrbMedium.Value,
                        "Big" => ConfigManager.shrinkEnemyOrbBig.Value,
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
                    if (levelValuables.tiny.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkTiny.Value)
                            ? ConfigManager.shrinkFactorTiny.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.small.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkSmall.Value)
                            ? ConfigManager.shrinkFactorSmall.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.medium.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkMedium.Value)
                            ? ConfigManager.shrinkFactorMedium.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.big.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkBig.Value)
                            ? ConfigManager.shrinkFactorBig.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.wide.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkWide.Value)
                            ? ConfigManager.shrinkFactorWide.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.tall.Exists(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        shrinkFactor = (ConfigManager.shouldShrinkTall.Value)
                            ? ConfigManager.shrinkFactorTall.Value
                            : initialShrinkFactor;

                        break;
                    }

                    if (levelValuables.veryTall.Exists(v => NameUtils.CleanName(v.name) == itemName))
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
    }
}