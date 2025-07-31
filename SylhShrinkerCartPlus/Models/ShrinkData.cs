using UnityEngine;

namespace SylhShrinkerCartPlus.Models
{
    public class ShrinkData
    {
        public string Name { get; set; }
        public float ShrinkFactor { get; set; }
        public Vector3 OriginalScale { get; set; }
        public float OriginalMass { get; set; }
        public float MinScale { get; set; }

        public ShrinkData(
            string name,
            float shrinkFactor,
            Vector3 originalScale,
            float originalMass,
            float minScale
        )
        {
            Name = name;
            ShrinkFactor = shrinkFactor;
            OriginalScale = originalScale;
            OriginalMass = originalMass;
            MinScale = minScale;
        }

        public override string ToString()
        {
            return $"Shrinking '{Name}' " +
                   $"- shrink Factor: {ShrinkFactor}, " +
                   $"- Original Scale: {OriginalScale}" +
                   $"- Original Mass: {OriginalMass}" +
                   $"- Min Scale: {MinScale}";
        }
    }
}