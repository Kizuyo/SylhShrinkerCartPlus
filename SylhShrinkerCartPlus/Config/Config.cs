using BepInEx.Configuration;
using UnityEngine;

namespace SylhShrinkerCartPlus.Config
{
    public class ConfigManager
    {
        public static ConfigEntry<float> defaultShrinkSpeed;
        
        public static ConfigEntry<bool> shouldShrinkEnemyOrbs;
        
        public static ConfigEntry<bool> shouldShrinkTiny;
        public static ConfigEntry<float> shrinkFactorTiny;
        
        public static ConfigEntry<bool> shouldShrinkSmall;
        public static ConfigEntry<float> shrinkFactorSmall;
        
        public static ConfigEntry<bool> shouldShrinkMedium;
        public static ConfigEntry<float> shrinkFactorMedium;
        
        public static ConfigEntry<bool> shouldShrinkBig;
        public static ConfigEntry<float> shrinkFactorBig;
        
        public static ConfigEntry<bool> shouldShrinkWide;
        public static ConfigEntry<float> shrinkFactorWide;
        
        public static ConfigEntry<bool> shouldShrinkTall;
        public static ConfigEntry<float> shrinkFactorTall;
        
        public static ConfigEntry<bool> shouldShrinkVeryTall;
        public static ConfigEntry<float> shrinkFactorVeryTall;
        
        internal static void Initialize(Plugin plugin)
        {
            defaultShrinkSpeed = plugin.Config.Bind(
                "Options", "Default Shrink Speed Value", 0.75f,
                new ConfigDescription(
                    "The default shrink speed value.", 
                    new AcceptableValueRange<float>(0.60f, 0.99f)
                )
            );
            
            shouldShrinkEnemyOrbs = plugin.Config.Bind(
                "Options", "Shrink Enemy Orbs", true,
                new ConfigDescription(
                    "When activated, all Enemy Orbs-class Valuables will be shrunk."
                )
            );
            
            shouldShrinkTiny = plugin.Config.Bind(
                "Options", "Shrink Tiny", false,
                new ConfigDescription(
                    "When activated, all Tiny-class Valuables will be shrunk."
                )
            );
            shrinkFactorTiny = plugin.Config.Bind(
                "Options", "Shrink Factor Tiny", 1.0f,
                new ConfigDescription(
                    "The shrink factor for all Tiny valuable (if shouldShrinkTiny is true).", 
                    new AcceptableValueRange<float>(0.90f, 1.0f)
                )
            );
            
            shouldShrinkSmall = plugin.Config.Bind(
                "Options", "Shrink Small", false,
                new ConfigDescription(
                    "When activated, all Small-class Valuables will be shrunk."
                )
            );
            shrinkFactorSmall = plugin.Config.Bind(
                "Options", "Shrink Factor Small", 1.0f,
                new ConfigDescription(
                    "The shrink factor for all Small valuable (if shouldShrinkSmall is true).", 
                    new AcceptableValueRange<float>(0.90f, 1.0f)
                )
            );
            
            shouldShrinkMedium = plugin.Config.Bind(
                "Options", "Shrink Medium", true,
                new ConfigDescription(
                    "When activated, all Medium-class Valuables will be shrunk."
                )
            );
            shrinkFactorMedium = plugin.Config.Bind(
                "Options", "Shrink Factor Medium", 0.50f,
                new ConfigDescription(
                    "The shrink factor for all Medium valuable (if shouldShrinkMedium is true).", 
                    new AcceptableValueRange<float>(0.50f, 1.0f)
                )
            );
            
            shouldShrinkBig = plugin.Config.Bind(
                "Options", "Shrink Big", true,
                new ConfigDescription(
                    "When activated, all Big-class Valuables will be shrunk."
                )
            );
            shrinkFactorBig = plugin.Config.Bind(
                "Options", "Shrink Factor Big", 0.3f,
                new ConfigDescription(
                    "The shrink factor for all Big valuable (if shouldShrinkBig is true).", 
                    new AcceptableValueRange<float>(0.30f, 1.0f)
                )
            );
            
            shouldShrinkWide = plugin.Config.Bind(
                "Options", "Shrink Wide", true,
                new ConfigDescription(
                    "When activated, all Wide-class Valuables will be shrunk."
                )
            );
            shrinkFactorWide = plugin.Config.Bind(
                "Options", "Shrink Factor Wide", 0.25f,
                new ConfigDescription(
                    "The shrink factor for all Wide valuable (if shouldShrinkWide is true).", 
                    new AcceptableValueRange<float>(0.25f, 1.0f)
                )
            );
            
            shouldShrinkTall = plugin.Config.Bind(
                "Options", "Shrink Tall", true,
                new ConfigDescription(
                    "When activated, all Tall-class Valuables will be shrunk."
                )
            );
            shrinkFactorTall = plugin.Config.Bind(
                "Options", "Shrink Factor Tall", 0.3f,
                new ConfigDescription(
                    "The shrink factor for all Tall valuable (if shouldShrinkTall is true).", 
                    new AcceptableValueRange<float>(0.3f, 1.0f)
                )
            );
            
            shouldShrinkVeryTall = plugin.Config.Bind(
                "Options", "Shrink VeryTall", true,
                new ConfigDescription(
                    "When activated, all Very-class Valuables will be shrunk."
                )
            );
            shrinkFactorVeryTall = plugin.Config.Bind(
                "Options", "Shrink Factor VeryTall", 0.2f,
                new ConfigDescription(
                    "The shrink factor for all VeryTall valuable (if shouldShrinkVeryTall is true).", 
                    new AcceptableValueRange<float>(0.2f, 1.0f)
                )
            );
        }
    }
}