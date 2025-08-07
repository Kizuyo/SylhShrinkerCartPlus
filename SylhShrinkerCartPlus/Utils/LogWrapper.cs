using BepInEx.Logging;
using SylhShrinkerCartPlus.Config;
using System;

namespace SylhShrinkerCartPlus.Utils
{
    public static class LogWrapper
    {
        private static ManualLogSource Log => Plugin.Log;

        private static string GenerateMessageStructure(string message, string prefix = "")
        {
            string finalPrefix = string.IsNullOrWhiteSpace(prefix)
                ? "SylhShrinkerCartPlus"
                : prefix;

            return $"[{DateTime.Now:HH:mm:ss}][{finalPrefix}] - {message}";
        }

        private static void LogMessage(LogLevel level, string message, string prefix = "", bool force = false)
        {
            if (!force && !ConfigManager.enableDebugLogs.Value)
                return;

            string structuredMessage = GenerateMessageStructure(message, prefix);

            switch (level)
            {
                case LogLevel.Info:
                    Log.LogInfo(structuredMessage);
                    break;
                case LogLevel.Debug:
                    Log.LogDebug(structuredMessage);
                    break;
                case LogLevel.Warning:
                    Log.LogWarning(structuredMessage);
                    break;
                case LogLevel.Error:
                    Log.LogError(structuredMessage);
                    break;
                default:
                    Log.LogMessage(structuredMessage);
                    break;
            }
        }

        public static void Info(string message, string prefix = "") => LogMessage(LogLevel.Info, message, prefix);
        public static void Debug(string message, string prefix = "") => LogMessage(LogLevel.Debug, message, prefix);
        public static void Warning(string message, string prefix = "") => LogMessage(LogLevel.Warning, message, prefix);
        public static void Error(string message, string prefix = "") => LogMessage(LogLevel.Error, message, prefix);

        public static void ForceInfo(string message, string prefix = "") => LogMessage(LogLevel.Info, message, prefix, true);
        public static void ForceWarning(string message, string prefix = "") => LogMessage(LogLevel.Warning, message, prefix, true);
        public static void ForceError(string message, string prefix = "") => LogMessage(LogLevel.Error, message, prefix, true);
    }
}
