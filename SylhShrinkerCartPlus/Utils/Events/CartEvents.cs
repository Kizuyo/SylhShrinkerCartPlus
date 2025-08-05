using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Utils.Shrink.Config;
using SylhShrinkerCartPlus.Utils.Shrink.Utils.Cheat.Enemy;

namespace SylhShrinkerCartPlus.Utils.Shrink.Utils.Events
{
    public class CartEvents
    {
        public static event Action<PhysGrabCart, PhysGrabObject> OnCartObjectAdded;
        public static event Action<PhysGrabObject, PhysGrabCart> OnEnterCart;

        public static void FireCartObjectAdded(
            PhysGrabInCart inCart,
            PhysGrabObject obj
        )
        {
            ShrinkableTracker tracker = ShrinkerCartPatch.GetTracker(obj);
            if (tracker == null) return;

            if (ConfigManager.shouldInstantKillEnemyInCart.Value)
            {
                EnemyExecutionManager.TryMarkForExecution(tracker);
            }
            
            tracker.Detector.ImpactDisable(1.5f);
            tracker.Detector.destroyDisable = true;

            PhysGrabCart currentCart = inCart.cart;
            if (tracker.IsInsideSameCart(currentCart)) return;
            if (tracker.IsInCart()) return;

            tracker.CurrentCart = currentCart;
            OnCartObjectAdded?.Invoke(currentCart, obj);
        }

        public static void RaiseEnterCart(PhysGrabObject obj, PhysGrabCart cart)
        {
            OnEnterCart?.Invoke(obj, cart);
        }
    }
}