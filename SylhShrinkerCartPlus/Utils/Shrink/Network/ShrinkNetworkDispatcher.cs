using Photon.Pun;
using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Models;
using UnityEngine;

namespace SylhShrinkerCartPlus.Utils.Shrink.Network
{
    public class ShrinkNetworkDispatcher : MonoBehaviourPun
    {
        [PunRPC]
        public void ProcessingShrinkingRPC(
            Vector3 originalScale,
            Vector3 targetScale,
            float shrinkFactor,
            float minShrinkRatio
        )
        {
            var item = GetComponent<PhysGrabObject>();
            if (item == null)
            {
                Debug.LogError("[ShrinkRPC] Aucun PhysGrabObject trouvé sur l'objet !");
                return;
            }

            ShrinkableTracker tracker = ShrinkerCartPatch.GetTracker(item);

            var data = new ShrinkData(
                item.name,
                tracker.InitialScale,
                tracker.InitialMass
            );

            data.ScaleShrinkFactor = shrinkFactor;
            data.MinShrinkRatio = minShrinkRatio;
            data.Target = targetScale;

            // // ✅ SET LE BON TARGET LÀ
            // Vector3 target = ShrinkerCartPatch.SetTargetShrink(tracker.InitialScale, data);
            // data.Target = target;
            
            NetworkHelper.ApplyShrink(tracker, data);
        }
        
        [PunRPC]
        public void ProcessingChangingMassRPC(
            float massValue,
            bool isChangingMass
        )
        {
            var item = GetComponent<PhysGrabObject>();
            if (item == null)
            {
                Debug.LogError("[ShrinkRPC] Aucun PhysGrabObject trouvé sur l'objet !");
                return;
            }

            ShrinkableTracker tracker = ShrinkerCartPatch.GetTracker(item);
            if (isChangingMass) NetworkHelper.ApplyMass(tracker, massValue);
        }
        
        [PunRPC]
        public void ProcessingRestoringMassRPC()
        {
            var item = GetComponent<PhysGrabObject>();
            if (item == null)
            {
                Debug.LogError("[ShrinkRPC] Aucun PhysGrabObject trouvé sur l'objet !");
                return;
            }

            ShrinkableTracker tracker = ShrinkerCartPatch.GetTracker(item);
            NetworkHelper.RestoreMass(tracker);
        }
        
        [PunRPC]
        public void ProcessingExpandingRPC()
        {
            var item = GetComponent<PhysGrabObject>();
            if (item == null)
            {
                Debug.LogError("[ExpandRPC] Aucun PhysGrabObject trouvé !");
                return;
            }

            ShrinkableTracker tracker = ShrinkerCartPatch.GetTracker(item);
            NetworkHelper.ApplyExpanding(tracker);
            NetworkHelper.RestoreMass(tracker);
        }
        
        [PunRPC]
        public void ProcessingChangingBatteryLifeRPC()
        {
            var item = GetComponent<PhysGrabObject>();
            if (item == null)
            {
                Debug.LogError("[ExpandRPC] Aucun PhysGrabObject trouvé !");
                return;
            }

            ShrinkableTracker tracker = ShrinkerCartPatch.GetTracker(item);
            NetworkHelper.ApplyChangingBatteryLife(tracker);
        }
    }
}