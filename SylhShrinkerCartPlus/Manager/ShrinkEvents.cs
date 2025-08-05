using SylhShrinkerCartPlus.Components;
using UnityEngine.Events;

namespace SylhShrinkerCartPlus.Manager
{
    public static class ShrinkEvents
    {
        public static event UnityAction<ShrinkableTracker> OnShrinkStarted;
        public static event UnityAction<ShrinkableTracker> OnShrinkCompleted;
        public static event UnityAction<ShrinkableTracker> OnExpandStarted;
        public static event UnityAction<ShrinkableTracker> OnExpandCompleted;
        public static event UnityAction<ShrinkableTracker> OnEnteredCart;
        public static event UnityAction<ShrinkableTracker> OnExitedCart;
            
        public static event UnityAction<ShrinkableTracker, float> OnMassChanged;
        
        internal static void RaiseShrinkStarted(ShrinkableTracker tracker)
        {
            OnShrinkStarted?.Invoke(tracker);
        }
        
        internal static void RaiseShrinkCompleted(ShrinkableTracker tracker)
        {
            OnShrinkCompleted?.Invoke(tracker);
        }
        
        internal static void RaiseExpandedStarted(ShrinkableTracker tracker)
        {
            OnExpandStarted?.Invoke(tracker);
        }
        
        internal static void RaiseExpandedCompleted(ShrinkableTracker tracker)
        {
            OnExpandCompleted?.Invoke(tracker);
        }
        
        internal static void RaiseExitCart(ShrinkableTracker tracker)
        {
            OnExitedCart?.Invoke(tracker);
        }
        
        internal static void RaiseEnterCart(ShrinkableTracker tracker)
        {
            OnEnteredCart?.Invoke(tracker);
        }
        
        internal static void RaiseMassChanged(ShrinkableTracker tracker, float mass)
        {
            OnMassChanged?.Invoke(tracker, mass);
        }
    }
}