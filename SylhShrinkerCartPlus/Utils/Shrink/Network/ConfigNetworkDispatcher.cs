using Photon.Pun;
using SylhShrinkerCartPlus.Config;

namespace SylhShrinkerCartPlus.Utils.Shrink.Network
{
    public class ConfigNetworkDispatcher : MonoBehaviourPun
    {
        [PunRPC]
        public void SyncShrinkConfigRPC(string json)
        {
            StaticConfig.RefreshInstanceFromJson(json);
            LogWrapper.Warning("[ShrinkSync] ✅ Configuration shrink synchronisée depuis le host !");
        }
    }
}