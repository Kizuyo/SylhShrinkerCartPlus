using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Utils.Shrink.Config;

namespace SylhShrinkerCartPlus.Utils.Shrink
{
    public static class ShrinkUnbreakableUtils
    {
        public static void ApplyUnbreakableLogic(ShrinkableTracker tracker)
        {
            if (tracker.IsInCart())
            {
                if (ConfigManager.shouldValuableSafeInsideCart.Value)
                {
                    tracker.MakeUnbreakable();
                }
                else
                {
                    tracker.MakeBreakable();
                }
            }
            else
            {
                if (ConfigManager.shouldValuableSafeInsideCart.Value &&
                    ConfigManager.shouldValuableStayUnbreakable.Value)
                {
                    tracker.MakeUnbreakable();
                }
                else
                {
                    tracker.MakeBreakable();
                }
            }
        }
    }
}