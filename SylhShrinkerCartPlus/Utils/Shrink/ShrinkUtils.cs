using System.Reflection;
using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;
using SylhShrinkerCartPlus.Resolver.Valuable;
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
            if (!StaticConfig.Instance.shouldShrinkEnemyOrbs)
            {
                return shrinkData;
            }

            switch (category.Category)
            {
                case EnemyValuableCategoryEnum.Small:
                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkEnemyOrbSmall;
                    break;
                case EnemyValuableCategoryEnum.Medium:
                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkEnemyOrbMedium;
                    break;
                case EnemyValuableCategoryEnum.Big:
                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkEnemyOrbBig;
                    break;
                case EnemyValuableCategoryEnum.Berserker:
                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkEnemyOrbBig;
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
                    if (!StaticConfig.Instance.shouldShrinkTiny)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorTiny;
                    break;
                case ValuableCategoryEnum.Small:
                    if (!StaticConfig.Instance.shouldShrinkSmall)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorSmall;
                    break;
                case ValuableCategoryEnum.Medium:
                    if (!StaticConfig.Instance.shouldShrinkMedium)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorMedium;
                    break;
                case ValuableCategoryEnum.Big:
                    if (!StaticConfig.Instance.shouldShrinkBig)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorBig;
                    break;
                case ValuableCategoryEnum.Wide:
                    if (!StaticConfig.Instance.shouldShrinkWide)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorWide;
                    break;
                case ValuableCategoryEnum.Tall:
                    if (!StaticConfig.Instance.shouldShrinkTall)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorTall;
                    break;
                case ValuableCategoryEnum.VeryTall:
                    if (!StaticConfig.Instance.shouldShrinkVeryTall)
                    {
                        return shrinkData;
                    }

                    shrinkData.MinShrinkRatio = StaticConfig.Instance.shrinkFactorVeryTall;
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
                shrinkData.MinShrinkRatio = StaticConfig.Instance.fallbackShrinkFactor;
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
                        shrinkData.MinShrinkRatio = StaticConfig.Instance.fallbackShrinkFactor;
                        break;
                }
            }
            
            shrinkData.ScaleShrinkFactor = GetItemScaleShrinkFactor(shrinkData);
            return shrinkData;
        }
    }
}