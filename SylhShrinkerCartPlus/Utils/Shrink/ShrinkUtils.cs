using System.Reflection;
using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Models;
using SylhShrinkerCartPlus.Resolver.Valuable;
using SylhShrinkerCartPlus.Utils.Shrink.Config;
using UnityEngine;

namespace SylhShrinkerCartPlus.Utils.Shrink
{
    public static class ShrinkUtils
    {
        private static float initialShrinkFactor = 1.0f;

        public static ShrinkData HandleEnemyValuable(
            EnemyValuableCategory category,
            ShrinkData shrinkData
        )
        {
            if (!ConfigManager.shouldShrinkEnemyOrbs.Value)
            {
                return shrinkData;
            }

            switch (category.Category)
            {
                case EnemyValuableCategoryEnum.Small:
                    shrinkData.MinShrinkRatio = ConfigManager.shrinkEnemyOrbSmall.Value;
                    break;
                case EnemyValuableCategoryEnum.Medium:
                    shrinkData.MinShrinkRatio = ConfigManager.shrinkEnemyOrbMedium.Value;
                    break;
                case EnemyValuableCategoryEnum.Big:
                    shrinkData.MinShrinkRatio = ConfigManager.shrinkEnemyOrbBig.Value;
                    break;
                case EnemyValuableCategoryEnum.Berserker:
                    shrinkData.MinShrinkRatio = ConfigManager.shrinkEnemyOrbBig.Value;
                    break;
            }

            return shrinkData;
        }

        public static ShrinkData HandleSpecialValuable(
            SpecialValuableCategory category,
            ShrinkData shrinkData
        )
        {
            switch (category.Category)
            {
                case SpecialValuableCategoryEnum.SurplusValuable:
                    shrinkData.MinShrinkRatio = 0.2f;
                    break;
            }

            return shrinkData;
        }

        public static ShrinkData HandleValuable(
            ValuableCategory category,
            ShrinkData shrinkData
        )
        {
            switch (category.Category)
            {
                case ValuableCategoryEnum.Tiny:
                    if (!ConfigManager.shouldShrinkTiny.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorTiny.Value;
                    break;
                case ValuableCategoryEnum.Small:
                    if (!ConfigManager.shouldShrinkSmall.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorSmall.Value;
                    break;
                case ValuableCategoryEnum.Medium:
                    if (!ConfigManager.shouldShrinkMedium.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorMedium.Value;
                    break;
                case ValuableCategoryEnum.Big:
                    if (!ConfigManager.shouldShrinkBig.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorBig.Value;
                    break;
                case ValuableCategoryEnum.Wide:
                    if (!ConfigManager.shouldShrinkWide.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorWide.Value;
                    break;
                case ValuableCategoryEnum.Tall:
                    if (!ConfigManager.shouldShrinkTall.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorTall.Value;
                    break;
                case ValuableCategoryEnum.VeryTall:
                    if (!ConfigManager.shouldShrinkVeryTall.Value)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = ConfigManager.shrinkFactorVeryTall.Value;
                    break;
            }

            return shrinkData;
        }

        public static Vector3 GetItemDimensions(PhysGrabObject item)
        {
            var helper = new FastReflectionHelper<PhysGrabObject>(item);
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;

            if (
                !helper.TryGetField("itemWidthX", out float width, flags) ||
                !helper.TryGetField("itemHeightY", out float height, flags) ||
                !helper.TryGetField("itemLengthZ", out float length, flags))
            {
                return Vector3.zero;
            }

            return new Vector3(width, height, length);
        }

        public static float GetItemScaleShrinkFactor(
            ShrinkData shrinkData
        )
        {
            float min = Mathf.Min(
                shrinkData.Dimensions.x,
                shrinkData.Dimensions.y,
                shrinkData.Dimensions.z
            );

            float max = Mathf.Max(
                shrinkData.Dimensions.x,
                shrinkData.Dimensions.y,
                shrinkData.Dimensions.z
            );

            float shrinkFactor;

            float ratio = max / min;

            if (ratio <= 2.5f)
            {
                shrinkFactor = shrinkData.MinShrinkRatio / min;
            }
            else
            {
                shrinkFactor = shrinkData.MinShrinkRatio / max;
            }

            return Mathf.Clamp(shrinkFactor, shrinkData.MinShrinkRatio, 1.0f);
        }

        public static ShrinkData GetShrinkData(
            PhysGrabObject item
        )
        {
            string itemName = NameUtils.CleanName(item.name);

            var category = CategoryResolverRegistry.Resolve(item);

            ShrinkableTracker? tracker = item.GetComponent<ShrinkableTracker>();
            ShrinkData shrinkData = new ShrinkData(
                itemName,
                tracker ? tracker.InitialScale : item.transform.localScale,
                item.massOriginal
            );
            shrinkData.Dimensions = GetItemDimensions(item);

            if (category == null)
            {
                shrinkData.MinShrinkRatio = ConfigManager.fallbackShrinkFactor.Value;
            }
            else
            {
                shrinkData.Category = category;

                switch (category)
                {
                    case EnemyValuableCategory enemy:
                        shrinkData = HandleEnemyValuable(enemy, shrinkData);
                        break;

                    case ValuableCategory valuable:
                        shrinkData = HandleValuable(valuable, shrinkData);
                        break;

                    case SpecialValuableCategory valuable:
                        shrinkData = HandleSpecialValuable(valuable, shrinkData);
                        break;
                    
                    default:
                        shrinkData.MinShrinkRatio = ConfigManager.fallbackShrinkFactor.Value;
                        break;
                }
            }
            
            shrinkData.ScaleShrinkFactor = GetItemScaleShrinkFactor(shrinkData);
            return shrinkData;
        }
    }
}