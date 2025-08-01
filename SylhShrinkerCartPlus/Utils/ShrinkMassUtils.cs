using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;
using UnityEngine;

namespace SylhShrinkerCartPlus.Utils
{
    public static class ShrinkMassUtils
    {
        private const float MASS_EPSILON = 0.01f;

        /// <summary>
        /// Calcule et applique une masse réduite selon shrinkFactor³ * shrinkMassMultiplier.
        /// </summary>
        public static void ApplyShrinkedMass(PhysGrabObject item, ShrinkData data)
        {
            if (data.OriginalMass <= 1.0f)
            {
                LogWrapper.Info($"[MassShrink] {item.name} already at smallest mass, skip.");
                return;
            };

            float expectedMass = ConfigManager.shrinkMassValue.Value;
            
            item.OverrideMass(expectedMass, 9999f);
            LogWrapper.Info($"[MassShrink] {item.name} → mass set to {expectedMass:F2} (factor={data.ScaleShrinkFactor:F2})");
        }

        /// <summary>
        /// Restaure la masse d'origine enregistrée dans le PhysGrabObject.
        /// </summary>
        public static void RestoreOriginalMass(PhysGrabObject item)
        {
            if (Mathf.Abs(item.rb.mass - item.massOriginal) > MASS_EPSILON)
            {
                item.OverrideMass(item.massOriginal, 9999f);
                LogWrapper.Info($"[MassReset] {item.name} → mass restored to {item.massOriginal:F2}");
            }
        }
    }
}
