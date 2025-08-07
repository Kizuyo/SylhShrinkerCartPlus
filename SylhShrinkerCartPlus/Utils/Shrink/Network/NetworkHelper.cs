using Photon.Pun;
using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Models;
using SylhShrinkerCartPlus.Utils.GameManagerUtils;
using UnityEngine;

namespace SylhShrinkerCartPlus.Utils.Shrink.Network;

public static class NetworkHelper
{
    public static void ProcessingShrinking(ShrinkableTracker tracker, ShrinkData data)
    {
        if (GameModeHelper.IsSinglePlayer)
        {
            ApplyShrink(tracker, data);
            return;
        }

        var photonView = tracker.GrabObject.GetComponent<PhotonView>();
        var dispatcher = tracker.GrabObject.GetComponent<ShrinkNetworkDispatcher>();

        if (photonView == null || dispatcher == null)
        {
            Debug.LogError("[Shrink] PhotonView ou Dispatcher manquant !");
            return;
        }

        Vector3 target = ShrinkerCartPatch.SetTargetShrink(tracker.InitialScale, data);
        data.Target = target;

        photonView.RPC("ProcessingShrinkingRPC", RpcTarget.All,
            data.OriginalScale,
            target,
            data.ScaleShrinkFactor,
            data.MinShrinkRatio
        );
    }
    
    public static void ApplyShrink(ShrinkableTracker tracker, ShrinkData data)
    {
        tracker.StartShrinking(data.Target);
    }

    public static void ProcessingChangingMass(
        ShrinkableTracker tracker
    )
    {
        if (GameModeHelper.IsSinglePlayer)
        {
            ApplyMass(tracker, StaticConfig.Instance.shrinkMassValue);
            return;
        }

        var photonView = tracker.GrabObject.GetComponent<PhotonView>();
        var dispatcher = tracker.GrabObject.GetComponent<ShrinkNetworkDispatcher>();

        if (photonView == null || dispatcher == null)
        {
            Debug.LogError("[Shrink] PhotonView ou Dispatcher manquant !");
            return;
        }

        photonView.RPC("ProcessingChangingMassRPC", RpcTarget.All,
            StaticConfig.Instance.shrinkMassValue,
            StaticConfig.Instance.shouldChangingMass
        );
    }
    
    public static void ApplyMass(ShrinkableTracker tracker, float amount)
    {
        tracker.ApplyMass(amount);
    }
    
    public static void ProcessingExpanding(ShrinkableTracker tracker)
    {
        if (GameModeHelper.IsSinglePlayer)
        {
            ApplyExpanding(tracker);
            RestoreMass(tracker);
            return;
        }

        var photonView = tracker.GrabObject.GetComponent<PhotonView>();
        var dispatcher = tracker.GrabObject.GetComponent<ShrinkNetworkDispatcher>();

        if (photonView == null || dispatcher == null)
        {
            Debug.LogError("[Expand] PhotonView ou Dispatcher manquant !");
            return;
        }

        photonView.RPC("ProcessingExpandingRPC", RpcTarget.All);
    }
    
    public static void ApplyExpanding(ShrinkableTracker tracker)
    {
        tracker.StartExpanding();
    }
    
    public static void ProcessingRestoringMass(
        ShrinkableTracker tracker
    )
    {
        if (GameModeHelper.IsSinglePlayer)
        {
            RestoreMass(tracker);
            return;
        }

        var photonView = tracker.GrabObject.GetComponent<PhotonView>();
        var dispatcher = tracker.GrabObject.GetComponent<ShrinkNetworkDispatcher>();

        if (photonView == null || dispatcher == null)
        {
            Debug.LogError("[Shrink] PhotonView ou Dispatcher manquant !");
            return;
        }

        photonView.RPC("ProcessingRestoringMassRPC", RpcTarget.All);
    }

    public static void RestoreMass(ShrinkableTracker tracker)
    {
        tracker.RestoreMass();
    }
    
    public static void ProcessingChangingBatteryLife(ShrinkableTracker tracker)
    {
        if (GameModeHelper.IsSinglePlayer)
        {
            ApplyChangingBatteryLife(tracker);
            return;
        }

        var photonView = tracker.GrabObject.GetComponent<PhotonView>();
        var dispatcher = tracker.GrabObject.GetComponent<ShrinkNetworkDispatcher>();

        if (photonView == null || dispatcher == null)
        {
            Debug.LogError("[Expand] PhotonView ou Dispatcher manquant !");
            return;
        }

        photonView.RPC("ProcessingChangingBatteryLifeRPC", RpcTarget.All);
    }

    public static void ApplyChangingBatteryLife(ShrinkableTracker tracker)
    {
        ShrinkBatteryUtils.ApplyBatteryLifeAll(tracker);
    }
}