namespace SylhShrinkerCartPlus.Utils.RunManagerUtils
{
    public static class RunManagerHelper
    {
        public static bool IsLevelShop()
        {
            return (global::RunManager.instance.levelCurrent == global::RunManager.instance.levelShop);
        }
        
        public static bool IsLevelLobby()
        {
            return (global::RunManager.instance.levelCurrent == global::RunManager.instance.levelLobby);
        }
        
        public static bool IsLevelSplashScreen()
        {
            return (global::RunManager.instance.levelCurrent == global::RunManager.instance.levelSplashScreen);
        }
        
        public static bool IsLevelArena()
        {
            return (global::RunManager.instance.levelCurrent == global::RunManager.instance.levelArena);
        }

        public static bool IsInsideValidLevel()
        {
            return !IsLevelShop() &&
                   !IsLevelLobby() &&
                   !IsLevelSplashScreen() &&
                   !IsLevelArena();
        }
    }
}