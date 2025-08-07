namespace SylhShrinkerCartPlus.Manager
{
    public static class CartEventHookManager
    {
        private static readonly Dictionary<PhysGrabObject, bool> currentCartStates = new();

        public static void Update()
        {
            foreach (var tracker in ShrinkTrackerManager.Instance.GetAll())
            {
                var obj = tracker.GrabObject;
                if (obj == null) continue;

                bool inCartNow = tracker.CurrentCart != null;
                bool wasInCart = currentCartStates.TryGetValue(obj, out bool oldState) && oldState;

                if (inCartNow && !wasInCart)
                {
                    ShrinkEvents.RaiseEnterCart(tracker);
                }
                else if (!inCartNow && wasInCart)
                {
                    ShrinkEvents.RaiseExitCart(tracker);
                }

                currentCartStates[obj] = inCartNow;
            }
        }

        public static void Clear()
        {
            currentCartStates.Clear();
        }
    }
}