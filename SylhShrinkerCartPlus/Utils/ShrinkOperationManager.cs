namespace SylhShrinkerCartPlus.Utils
{
    public static class ShrinkOperationManager
    {
        private static readonly Dictionary<PhysGrabObject, PhysGrabCart> objectToCart = new();

        public static bool TryAssign(PhysGrabObject obj, PhysGrabCart cart)
        {
            if (obj == null) return false;

            lock (objectToCart)
            {
                if (objectToCart.TryGetValue(obj, out var existingCart))
                {
                    if (existingCart == cart)
                        return true; // already owned by same cart

                    return false; // already shrinked by another cart
                }

                objectToCart[obj] = cart;
                return true;
            }
        }

        public static void Release(PhysGrabObject obj)
        {
            if (obj == null) return;

            lock (objectToCart)
            {
                objectToCart.Remove(obj);
            }
        }

        public static bool IsInUse(PhysGrabObject obj)
        {
            lock (objectToCart)
            {
                return objectToCart.ContainsKey(obj);
            }
        }

        public static void ClearCart(PhysGrabCart cart)
        {
            lock (objectToCart)
            {
                var toRemove = objectToCart.Where(pair => pair.Value == cart).Select(pair => pair.Key).ToList();
                foreach (var obj in toRemove)
                    objectToCart.Remove(obj);
            }
        }
    }
}
