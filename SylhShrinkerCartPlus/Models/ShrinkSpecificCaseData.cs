using UnityEngine;

namespace SylhShrinkerCartPlus.Models
{
    public class ShrinkSpecificCaseData
    {
        public float ShrinkFactor { get; set; }
        public float MinScale { get; set; }

        public ShrinkSpecificCaseData(
            float shrinkFactor,
            float minScale
        )
        {
            ShrinkFactor = shrinkFactor;
            MinScale = minScale;
        }

        public override string ToString()
        {
            return $"- shrink Factor: {ShrinkFactor}, " +
                   $"- Min Scale: {MinScale}";
        }
    }
}