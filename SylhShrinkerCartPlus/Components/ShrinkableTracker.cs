using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Manager;
using UnityEngine;
using SylhShrinkerCartPlus.Utils;
using SylhShrinkerCartPlus.Utils.Cheat.Cart;
using SylhShrinkerCartPlus.Utils.Cheat.Enemy;

namespace SylhShrinkerCartPlus.Components
{
    public class ShrinkableTracker : MonoBehaviour
    {
        public bool IsInCart() => CurrentCart != null;
        public bool IsInsideSameCart(PhysGrabCart otherCart) => CurrentCart.Equals(otherCart);
        
        public PhysGrabCart CurrentCart { get; set; }
        public PhysGrabCart PreviousCart { get; set; }

        public bool IsShrunk { get; set; } = false;
        public bool IsShrinking { get; set; } = false;
        public bool IsExpanded { get; set; } = true;
        public bool IsExpanding { get; set; } = false;

        public float InitialMass { get; private set; }
        public Vector3 InitialScale { get; private set; }
        public Vector3 TargetScale { get; private set; }

        public bool CanResetBattery { get; set; } = false;
        public int BatteryLife { get; set; } = 0;
        
        public PhysGrabObject GrabObject;
        private float _shrinkSpeed;

        public PhysGrabObjectImpactDetector Detector;
        public float InitialFragility { get; private set; }
        public float Fragility { get; private set; }
        public bool IsValidValuable { get; private set; }
        

        public void Init(PhysGrabObject owner)
        {
            GrabObject = owner;

            IsValidValuable = IsValidShrinkableItem();
            
            InitialScale = owner.transform.localScale;
            InitialMass = owner.massOriginal;
            _shrinkSpeed = StaticConfig.Instance.defaultShrinkSpeed;

            InitBattery();
            InitFragility();
        }
        
        public void InitBattery()
        {
            if (HasBattery(out var battery))
            {
                int currentBattery = (battery.batteryLife > 100f) ? 100 : (int)battery.batteryLife;
                BatteryLife = currentBattery;
            }
        }
        
        public void InitFragility()
        {
            Detector = GrabObject.GetComponent<PhysGrabObjectImpactDetector>();
            if (Detector == null) return;
            
            InitialFragility = Detector.fragility;
            Fragility = 0f;
        }

        public void MakeUnbreakable()
        {
            if (!IsValidValuable) return;
            Detector.fragility = Fragility;
        }

        public void MakeBreakable()
        {
            if (!IsValidValuable) return;
            Detector.fragility = InitialFragility;
        }

        private void Awake()
        {
            if (GrabObject == null)
                GrabObject = GetComponent<PhysGrabObject>();

            if (GrabObject != null && InitialScale == Vector3.zero)
                InitialScale = GrabObject.transform.localScale;
            
            ShrinkTrackerManager.Instance.Register(this);
        }
        
        void OnDestroy()
        {
            ShrinkTrackerManager.Instance.Unregister(this);
        }

        private void Update()
        {
            if (IsShrinking)
            {
                MakeUnbreakable();
                GrabObject.transform.localScale = Vector3.MoveTowards(
                    GrabObject.transform.localScale,
                    TargetScale,
                    _shrinkSpeed * Time.deltaTime
                );

                if (Vector3.Distance(GrabObject.transform.localScale, TargetScale) < 0.01f)
                {
                    IsShrinking = false;
                    IsShrunk = true;

                    // ShrinkEvents.RaiseShrinkCompleted(GrabObject, new ShrinkData
                    // {
                    //     Target = TargetScale,
                    //     OriginalScale = InitialScale,
                    //     Dimensions = InitialScale
                    // });
                    
                    ShrinkEvents.RaiseShrinkCompleted(this);
                }
            }

            if (IsExpanding)
            {
                MakeUnbreakable();
                GrabObject.transform.localScale = Vector3.MoveTowards(
                    GrabObject.transform.localScale,
                    InitialScale,
                    _shrinkSpeed * Time.deltaTime
                );

                if (Vector3.Distance(GrabObject.transform.localScale, InitialScale) < 0.01f)
                {
                    IsExpanding = false;
                    IsExpanded = true;

                    ShrinkEvents.RaiseExpandedCompleted(this);
                }
            }
        }
        
        public bool IsValuable()
        {
            return GrabObject.GetComponent<ValuableObject>() != null;
        }

        public bool IsSurplusValuable()
        {
            return GrabObject.GetComponent<SurplusValuable>() != null;
        }
        
        public bool IsEnemyValuable()
        {
            return NameUtils.TryParseEnemyValuable(GrabObject.name, out _, out var enemyType);
        }

        public bool IsEnemy()
        {
            return (EnemyExecutionManager.EnemyReflectionUtils.TryGetEnemyComponents(
                this,
                out var enemy,
                out var health,
                out int hp,
                out var onDeathEvent
            ));
        }

        public bool HasBattery(out ItemBattery battery)
        {
            return ItemBatteryUtils.TryGetBattery(GrabObject, out battery);
        }

        public bool IsValidShrinkableItem()
        {
            return IsValuable() ||
                   IsSurplusValuable() ||
                   IsEnemyValuable();
        }
        
        public void StartShrinking(Vector3 target)
        {
            TargetScale = target;
            IsShrinking = true;
            IsShrunk = false;
            IsExpanded = false;
            IsExpanding = false;
            
            ShrinkEvents.RaiseShrinkStarted(this);
        }

        public void StartExpanding()
        {
            IsExpanding = true;
            IsExpanded = false;
            IsShrinking = false;
            IsShrunk = false;

            ShrinkEvents.RaiseExpandedStarted(this);
        }
        
        public void InterruptExpanding()
        {
            if (IsExpanding)
            {
                IsExpanding = false;
                IsExpanded = false;
                LogWrapper.Debug($"[Tracker] ✋ Expansion interrompue pour {GrabObject.name}");
            }
        }
        
        public void InterruptShrinking()
        {
            if (IsShrinking)
            {
                IsShrinking = false;
                IsShrunk = false;
                LogWrapper.Debug($"[Tracker] ✋ Shrinking interrompue pour {GrabObject.name}");
            }
        }
        
        public void ResetShrinkState()
        {
            IsShrunk = false;
            IsShrinking = false;
            IsExpanded = true;
            IsExpanding = false;
        }

        public void ClearCart()
        {
            CurrentCart = null;
        }

        public void ApplyMass(float mass)
        {
            GrabObject.OverrideMass(mass);
            ShrinkEvents.RaiseMassChanged(this, mass);
        }
        
        public void RestoreMass()
        {
            ApplyMass(InitialMass);
        }

        public bool IsShrinkable()
        {
            Vector3 currentScale = GrabObject.transform.localScale;

            return currentScale.x >= InitialScale.x - 0.01f &&
                   currentScale.y >= InitialScale.y - 0.01f &&
                   currentScale.z >= InitialScale.z - 0.01f;
        }

        public bool IsExpandable()
        {
            Vector3 currentScale = GrabObject.transform.localScale;
            
            return 
                currentScale.x < InitialScale.x && 
                currentScale.y < InitialScale.y && 
                currentScale.z < InitialScale.z;
        }
    }
}
