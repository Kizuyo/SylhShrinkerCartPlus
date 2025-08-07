using SylhShrinkerCartPlus.Utils;
using UnityEngine;

namespace SylhShrinkerCartPlus.Config
{
    [Serializable]
    public class StaticConfig
    {
        private static StaticConfig instance = null;
        private static readonly object padlock = new object();
        
        public static StaticConfig Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        LogWrapper.Warning("Instance demandée avant initialisation ! Utilisation d’une config vide.");
                        instance = new StaticConfig();
                    }
                    return instance;
                }
            }
        }
        
        [Range(0.60f, 1f)]
        public float defaultShrinkSpeed = 0.60f;
        
        [Range(0.20f, 1f)]
        public float fallbackShrinkFactor = 0.20f;

        public bool shouldShrinkEnemyOrbs = true;
        
        [Range(0.20f, 1f)]
        public float shrinkEnemyOrbSmall = 0.20f;
        
        [Range(0.20f, 1f)]
        public float shrinkEnemyOrbMedium = 0.20f;
        
        [Range(0.20f, 1f)]
        public float shrinkEnemyOrbBig = 0.20f;
        public bool shouldInstantKillEnemyInCart = false;

        public bool shouldShrinkTiny = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorTiny = 0.20f;

        public bool shouldShrinkSmall = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorSmall = 0.20f;

        public bool shouldShrinkMedium = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorMedium = 0.20f;

        public bool shouldShrinkBig = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorBig = 0.20f;

        public bool shouldShrinkWide = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorWide = 0.20f;

        public bool shouldShrinkTall = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorTall = 0.20f;

        public bool shouldShrinkVeryTall = true;
        
        [Range(0.20f, 1f)]
        public float shrinkFactorVeryTall = 0.20f;

        public bool shouldKeepShrunk = false;
        public bool shouldChangingMass = true;
        
        [Range(0.25f, 1f)]
        public float shrinkMassValue = 0.25f;

        public bool shouldCartWeaponBatteryLifeInfinite = false;
        public bool shouldItemMeleeBatteryLifeInfinite = false;
        public bool shouldItemGunBatteryLifeInfinite = false;
        public bool shouldItemDroneBatteryLifeInfinite = false;
        public bool shouldItemGenericBatteryLifeInfinite = false;

        public static void RefreshInstanceFromCurrentConfig()
        {
            lock (padlock)
            {
                instance = FromCurrentConfig();
            }
        }
        
        private static StaticConfig FromCurrentConfig()
        {
            return new StaticConfig
            {
                defaultShrinkSpeed = ConfigManager.defaultShrinkSpeed.Value,
                fallbackShrinkFactor = ConfigManager.fallbackShrinkFactor.Value,

                shouldShrinkEnemyOrbs = ConfigManager.shouldShrinkEnemyOrbs.Value,
                shrinkEnemyOrbSmall = ConfigManager.shrinkEnemyOrbSmall.Value,
                shrinkEnemyOrbMedium = ConfigManager.shrinkEnemyOrbMedium.Value,
                shrinkEnemyOrbBig = ConfigManager.shrinkEnemyOrbBig.Value,
                shouldInstantKillEnemyInCart = ConfigManager.shouldInstantKillEnemyInCart.Value,

                shouldShrinkTiny = ConfigManager.shouldShrinkTiny.Value,
                shrinkFactorTiny = ConfigManager.shrinkFactorTiny.Value,

                shouldShrinkSmall = ConfigManager.shouldShrinkSmall.Value,
                shrinkFactorSmall = ConfigManager.shrinkFactorSmall.Value,

                shouldShrinkMedium = ConfigManager.shouldShrinkMedium.Value,
                shrinkFactorMedium = ConfigManager.shrinkFactorMedium.Value,

                shouldShrinkBig = ConfigManager.shouldShrinkBig.Value,
                shrinkFactorBig = ConfigManager.shrinkFactorBig.Value,

                shouldShrinkWide = ConfigManager.shouldShrinkWide.Value,
                shrinkFactorWide = ConfigManager.shrinkFactorWide.Value,

                shouldShrinkTall = ConfigManager.shouldShrinkTall.Value,
                shrinkFactorTall = ConfigManager.shrinkFactorTall.Value,

                shouldShrinkVeryTall = ConfigManager.shouldShrinkVeryTall.Value,
                shrinkFactorVeryTall = ConfigManager.shrinkFactorVeryTall.Value,

                shouldKeepShrunk = ConfigManager.shouldKeepShrunk.Value,
                shouldChangingMass = ConfigManager.shouldChangingMass.Value,
                shrinkMassValue = ConfigManager.shrinkMassValue.Value,

                shouldCartWeaponBatteryLifeInfinite = ConfigManager.shouldCartWeaponBatteryLifeInfinite.Value,
                shouldItemMeleeBatteryLifeInfinite = ConfigManager.shouldItemMeleeBatteryLifeInfinite.Value,
                shouldItemGunBatteryLifeInfinite = ConfigManager.shouldItemGunBatteryLifeInfinite.Value,
                shouldItemDroneBatteryLifeInfinite = ConfigManager.shouldItemDroneBatteryLifeInfinite.Value,
                shouldItemGenericBatteryLifeInfinite = ConfigManager.shouldItemGenericBatteryLifeInfinite.Value,
            };
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        public static void RefreshInstanceFromJson(string json)
        {
            lock (padlock)
            {
                instance = FromJson(json);
            }
        }
        
        private  static StaticConfig FromJson(string json)
        {
            try
            {
                var config = JsonUtility.FromJson<StaticConfig>(json);
                return config ?? new StaticConfig();
            }
            catch (Exception ex)
            {
                LogWrapper.Error($"❌ Erreur de désérialisation JSON : {ex.Message}");
                return new StaticConfig();
            }
        }
    }
}