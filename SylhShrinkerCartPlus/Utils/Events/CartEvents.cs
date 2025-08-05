namespace SylhShrinkerCartPlus.Utils.Shrink.Utils.Events
{
    public class CartEvents
    {
        public static event Action<PhysGrabCart, PhysGrabObject> OnCartObjectAdded;
        public static event Action<PhysGrabObject, PhysGrabCart> OnEnterCart;
        
        public static void FireCartObjectAdded(PhysGrabInCart inCart, PhysGrabObject obj)
        {
            if (ShrinkerCartPatch.GetTracker(obj).IsInsideSameCart(inCart.cart))
            {
                return;
            }
            
            if (ShrinkerCartPatch.GetTracker(obj).IsInCart())
            {
                return;
            }

            var cart = inCart.cart;
            OnCartObjectAdded?.Invoke(cart, obj);
        }

        public static void RaiseEnterCart(PhysGrabObject obj, PhysGrabCart cart)
        {
            OnEnterCart?.Invoke(obj, cart);
        }
    }
}