using BepInEx.Logging;
using SylhShrinkerCartPlus.Config;

namespace SylhShrinkerCartPlus.Utils
{
    public static class LogWrapper
    {
        private static ManualLogSource Log => Plugin.Log;

        public static void Info(string message)
        {
            if (ConfigManager.enableDebugLogs.Value)
                Log.LogInfo(message);
        }

        public static void Warning(string message)
        {
            if (ConfigManager.enableDebugLogs.Value)
                Log.LogWarning(message);
        }

        public static void Error(string message)
        {
            if (ConfigManager.enableDebugLogs.Value)
                Log.LogError(message);
        }

        public static void ForceInfo(string message) => Log.LogInfo(message);
        public static void ForceWarning(string message) => Log.LogWarning(message);
        public static void ForceError(string message) => Log.LogError(message);
    }
}