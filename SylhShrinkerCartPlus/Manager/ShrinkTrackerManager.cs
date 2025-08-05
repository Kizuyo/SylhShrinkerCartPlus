using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Utils;
using SylhShrinkerCartPlus.Utils.Shrink.Resolver.Valuable;

namespace SylhShrinkerCartPlus.Manager
{
    public class ShrinkTrackerManager
    {
        private static readonly ShrinkTrackerManager _instance = new ShrinkTrackerManager();
        public static ShrinkTrackerManager Instance => _instance;

        private readonly Dictionary<PhysGrabObject, ShrinkableTracker> _trackers = new();
        private readonly Dictionary<PhysGrabCart, HashSet<PhysGrabObject>> _completedShrunkItemsPerCart = new();


        private ShrinkTrackerManager() {}

        public void Register(ShrinkableTracker tracker)
        {
            if (tracker?.GrabObject == null) return;

            if (!_trackers.ContainsKey(tracker.GrabObject))
            {
                _trackers.Add(tracker.GrabObject, tracker);
                LogWrapper.Debug($"[TrackerManager] ✅ Registered: {tracker.GrabObject.name}");
            }
        }

        public void Unregister(ShrinkableTracker tracker)
        {
            if (tracker?.GrabObject == null) return;

            if (_trackers.Remove(tracker.GrabObject))
            {
                LogWrapper.Debug($"[TrackerManager] ❌ Unregistered: {tracker.GrabObject.name}");
            }
        }

        public void ClearAll()
        {
            ClearAllShrinkCompletions();
            ClearAllTrackers();
        }
        
        public void ClearAllShrinkCompletions()
        {
            _completedShrunkItemsPerCart.Clear();
            LogWrapper.Info("🧹 Tous les objets shrinkés ont été réinitialisés !");
        }
        
        public void ClearAllTrackers()
        {
            _trackers.Clear();
            LogWrapper.Info("🧹 Tous les trackers ont été réinitialisés !");
        }

        public ShrinkableTracker GetTracker(PhysGrabObject GrabObject)
        {
            if (GrabObject == null) return null;
            return _trackers.TryGetValue(GrabObject, out var tracker) ? tracker : null;
        }

        public IEnumerable<ShrinkableTracker> GetAll() => _trackers.Values;

        public IEnumerable<ShrinkableTracker> GetTrackersNotInCart()
        {
            return _trackers.Values
                .Where(t => t != null && t.CurrentCart == null);
        }
        
        public IEnumerable<ShrinkableTracker> GetTrackersInCart()
            => _trackers.Values.Where(t => t.IsInCart());
        
        public IEnumerable<ShrinkableTracker> GetTrackersInCart(PhysGrabCart cart)
        {
            return _trackers.Values.Where(t =>
                t != null &&
                t.CurrentCart == cart);
        }

        public IEnumerable<ShrinkableTracker> GetShrinkingInCart(PhysGrabCart cart)
        {
            return _trackers.Values.Where(t =>
                t != null &&
                t.CurrentCart == cart &&
                t.IsShrinking);
        }

        public IEnumerable<ShrinkableTracker> GetExpandedInCart(PhysGrabCart cart)
        {
            return _trackers.Values.Where(t =>
                t != null &&
                t.CurrentCart == cart &&
                t.IsExpanded);
        }

        public IEnumerable<ShrinkableTracker> GetIdleInCart(PhysGrabCart cart)
        {
            return _trackers.Values.Where(t =>
                t != null &&
                t.CurrentCart == cart &&
                !t.IsShrinking &&
                !t.IsExpanding &&
                !t.IsExpanded);
        }
        
        
        public IEnumerable<ShrinkableTracker> GetShrinking()
            => _trackers.Values.Where(t => t != null && t.IsShrinking);

        public IEnumerable<ShrinkableTracker> GetExpanded()
            => _trackers.Values.Where(t => t != null && t.IsExpanded);

        public IEnumerable<ShrinkableTracker> GetIdle()
            => _trackers.Values.Where(t =>
                t != null &&
                !t.IsShrinking &&
                !t.IsExpanding &&
                !t.IsExpanded);

        
        public void RegisterShrinkCompletion(PhysGrabCart cart, PhysGrabObject obj)
        {
            if (cart == null || obj == null) return;

            if (!_completedShrunkItemsPerCart.TryGetValue(cart, out var set))
            {
                set = new HashSet<PhysGrabObject>();
                _completedShrunkItemsPerCart[cart] = set;
            }

            set.Add(obj);
        }

        public void UnregisterShrinkCompletion(PhysGrabCart cart, PhysGrabObject obj)
        {
            if (cart == null || obj == null) return;

            if (_completedShrunkItemsPerCart.TryGetValue(cart, out var set))
            {
                set.Remove(obj);

                if (set.Count == 0)
                    _completedShrunkItemsPerCart.Remove(cart);
            }
        }

        public HashSet<PhysGrabObject> GetCompletedShrunkObjects(PhysGrabCart cart)
        {
            if (cart == null) return new HashSet<PhysGrabObject>();
            return _completedShrunkItemsPerCart.TryGetValue(cart, out var set)
                ? new HashSet<PhysGrabObject>(set)
                : new HashSet<PhysGrabObject>();
        }
        
        
        
        
        public void LogAll()
        {
            LogWrapper.Info($"[TrackerManager] 🧠 {_trackers.Count} tracker(s) enregistrés.");
            foreach (var tracker in _trackers.Values)
            {
                LogWrapper.Info($" - {tracker.GrabObject?.name ?? "???"} | InCart: {tracker.IsInCart} | Shrinking: {tracker.IsShrinking}");
            }
        }
    }
}

