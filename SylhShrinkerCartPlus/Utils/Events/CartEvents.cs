using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Utils.Cheat.Enemy;

namespace SylhShrinkerCartPlus.Utils.Events
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

            tracker.InitProtectTimer();
            if (StaticConfig.Instance.shouldInstantKillEnemyInCart)
            {
                EnemyExecutionManager.TryMarkForExecution(tracker);
            }
            
            PhysGrabCart currentCart = inCart.cart;
            if (tracker.IsInsideSameCart(currentCart)) return;
            if (tracker.IsInCart()) return;

            // tracker.CurrentCart = currentCart;
            OnCartObjectAdded?.Invoke(currentCart, obj);
        }

        public static void RaiseEnterCart(PhysGrabObject obj, PhysGrabCart cart)
        {
            OnEnterCart?.Invoke(obj, cart);
        }
    }
}