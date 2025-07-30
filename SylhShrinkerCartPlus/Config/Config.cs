using BepInEx.Configuration;
using UnityEngine;

namespace SylhShrinkerCartPlus.Config
{
    public class ConfigManager
    {
        public static ConfigEntry<float> defaultShrinkSpeed;
        
        public static ConfigEntry<bool> shouldShrinkEnemyOrbs;
        public static ConfigEntry<float> shrinkEnemyOrbSmall;
        public static ConfigEntry<float> shrinkEnemyOrbMedium;
        public static ConfigEntry<float> shrinkEnemyOrbBig;
        
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
        
        public static ConfigEntry<bool> shouldKeepShrunk;
        public static ConfigEntry<bool> shouldChangingMass;
        public static ConfigEntry<float> shrinkMassValue;
        
        internal static void Initialize(Plugin plugin)
        {
            defaultShrinkSpeed = plugin.Config.Bind(
                "Options", "Default Shrink Speed Value", 0.75f,
                new ConfigDescription(
                    "The default shrink speed value.", 
                    new AcceptableValueRange<float>(0.60f, 0.99f)
                )
            );
            
            shouldKeepShrunk = plugin.Config.Bind(
                "Options", "Keep Shrunk Valuable", false,
                new ConfigDescription(
                    "When activated, Keeps the item shrunk even after it's taken out of the cart."
                )
            );
            
            shouldChangingMass = plugin.Config.Bind(
                "Options", "Changing Mass", true,
                new ConfigDescription(
                    "When activated, Item Mass will change when leaving / entering in the cart."
                )
            );
            shrinkMassValue = plugin.Config.Bind(
                "Options", "Shrink Mass Multiplier", 1.0f,
                new ConfigDescription(
                    "Value applied to the mass after shrink.",
                    new AcceptableValueRange<float>(0.25f, 2.0f)
                )
            );
            
            shouldShrinkEnemyOrbs = plugin.Config.Bind(
                "Enemy", "Shrink Enemy Orbs", true,
                new ConfigDescription(
                    "When activated, all Enemy Orbs-class Valuables will be shrunk."
                )
            );
            shrinkEnemyOrbSmall = plugin.Config.Bind(
                "Enemy", "Shrink Enemy Orb Small", 1.0f,
                new ConfigDescription(
                    "The shrink factor for small enemy orb valuable (if shouldShrinkEnemyOrbs is true).", 
                    new AcceptableValueRange<float>(0.90f, 1.0f)
                )
            );
            shrinkEnemyOrbMedium = plugin.Config.Bind(
                "Enemy", "Shrink Enemy Orb Medium", 0.5f,
                new ConfigDescription(
                    "The shrink factor for medium enemy orb valuable (if shouldShrinkEnemyOrbs is true).", 
                    new AcceptableValueRange<float>(0.50f, 1.0f)
                )
            );
            shrinkEnemyOrbBig = plugin.Config.Bind(
                "Enemy", "Shrink Enemy Orb Big", 0.3f,
                new ConfigDescription(
                    "The shrink factor for big enemy orb valuable (if shouldShrinkEnemyOrbs is true).", 
                    new AcceptableValueRange<float>(0.30f, 1.0f)
                )
            );
            
            shouldShrinkTiny = plugin.Config.Bind(
                "Valuable", "Shrink Tiny", false,
                new ConfigDescription(
                    "When activated, all Tiny-class Valuables will be shrunk."
                )
            );
            shrinkFactorTiny = plugin.Config.Bind(
                "Valuable", "Shrink Factor Tiny", 1.0f,
                new ConfigDescription(
                    "The shrink factor for all Tiny valuable (if shouldShrinkTiny is true).", 
                    new AcceptableValueRange<float>(0.90f, 1.0f)
                )
            );
            
            shouldShrinkSmall = plugin.Config.Bind(
                "Valuable", "Shrink Small", false,
                new ConfigDescription(
                    "When activated, all Small-class Valuables will be shrunk."
                )
            );
            shrinkFactorSmall = plugin.Config.Bind(
                "Valuable", "Shrink Factor Small", 1.0f,
                new ConfigDescription(
                    "The shrink factor for all Small valuable (if shouldShrinkSmall is true).", 
                    new AcceptableValueRange<float>(0.90f, 1.0f)
                )
            );
            
            shouldShrinkMedium = plugin.Config.Bind(
                "Valuable", "Shrink Medium", true,
                new ConfigDescription(
                    "When activated, all Medium-class Valuables will be shrunk."
                )
            );
            shrinkFactorMedium = plugin.Config.Bind(
                "Valuable", "Shrink Factor Medium", 0.50f,
                new ConfigDescription(
                    "The shrink factor for all Medium valuable (if shouldShrinkMedium is true).", 
                    new AcceptableValueRange<float>(0.50f, 1.0f)
                )
            );
            
            shouldShrinkBig = plugin.Config.Bind(
                "Valuable", "Shrink Big", true,
                new ConfigDescription(
                    "When activated, all Big-class Valuables will be shrunk."
                )
            );
            shrinkFactorBig = plugin.Config.Bind(
                "Valuable", "Shrink Factor Big", 0.3f,
                new ConfigDescription(
                    "The shrink factor for all Big valuable (if shouldShrinkBig is true).", 
                    new AcceptableValueRange<float>(0.30f, 1.0f)
                )
            );
            
            shouldShrinkWide = plugin.Config.Bind(
                "Valuable", "Shrink Wide", true,
                new ConfigDescription(
                    "When activated, all Wide-class Valuables will be shrunk."
                )
            );
            shrinkFactorWide = plugin.Config.Bind(
                "Valuable", "Shrink Factor Wide", 0.25f,
                new ConfigDescription(
                    "The shrink factor for all Wide valuable (if shouldShrinkWide is true).", 
                    new AcceptableValueRange<float>(0.25f, 1.0f)
                )
            );
            
            shouldShrinkTall = plugin.Config.Bind(
                "Valuable", "Shrink Tall", true,
                new ConfigDescription(
                    "When activated, all Tall-class Valuables will be shrunk."
                )
            );
            shrinkFactorTall = plugin.Config.Bind(
                "Valuable", "Shrink Factor Tall", 0.3f,
                new ConfigDescription(
                    "The shrink factor for all Tall valuable (if shouldShrinkTall is true).", 
                    new AcceptableValueRange<float>(0.3f, 1.0f)
                )
            );
            
            shouldShrinkVeryTall = plugin.Config.Bind(
                "Valuable", "Shrink VeryTall", true,
                new ConfigDescription(
                    "When activated, all Very-class Valuables will be shrunk."
                )
            );
            shrinkFactorVeryTall = plugin.Config.Bind(
                "Valuable", "Shrink Factor VeryTall", 0.2f,
                new ConfigDescription(
                    "The shrink factor for all VeryTall valuable (if shouldShrinkVeryTall is true).", 
                    new AcceptableValueRange<float>(0.2f, 1.0f)
                )
            );
        }
    }
}