using UnityEngine;

namespace SylhShrinkerCartPlus.Models
{
    public class ShrinkData
    {
        [Range(0.01f, 1.0f)]
        public float MinShrinkRatio;
        
        public string Name { get; set; }
        public Vector3 OriginalScale { get; set; }
        public float OriginalMass { get; set; }
        public Vector3 Dimensions { get; set; }
        public float ScaleShrinkFactor { get; set; }
        public ValuableCategoryBase Category { get; set; }

        public ShrinkData(
            string name,
            Vector3 originalScale,
            float originalMass,
            float minShrinkRatio = 1.0f,
            float scaleShrinkFactor = 1.0f
        )
        {
            Name = name;
            OriginalScale = originalScale;
            OriginalMass = originalMass;
            MinShrinkRatio = minShrinkRatio;
            ScaleShrinkFactor = scaleShrinkFactor;
        }

        public override string ToString()
        {
            return $"Shrinking '{Name}' " +
                   $"- Original Scale: {OriginalScale}" +
                   $"- Original Mass: {OriginalMass}" +
                   $"- Min Shrink Ratio: {MinShrinkRatio}";
        }
    }
}