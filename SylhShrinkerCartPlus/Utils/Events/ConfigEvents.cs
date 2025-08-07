using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Manager;
using SylhShrinkerCartPlus.Utils.Cheat.Enemy;
using SylhShrinkerCartPlus.Utils.RunManagerUtils;
using SylhShrinkerCartPlus.Utils.Shrink;

namespace SylhShrinkerCartPlus.Utils.Events
{
    public static class ConfigEvents
    {
        public static void Initialize()
        {
            ConfigManager. defaultShrinkSpeed.SettingChanged += OnSettingChanged;
            ConfigManager. fallbackShrinkFactor.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkEnemyOrbs.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkEnemyOrbSmall.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkEnemyOrbMedium.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkEnemyOrbBig.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkTiny.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorTiny.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkSmall.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorSmall.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkMedium.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorMedium.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkBig.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorBig.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkWide.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorWide.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkTall.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorTall.SettingChanged += OnSettingChanged;

            ConfigManager.shouldShrinkVeryTall.SettingChanged += OnSettingChanged;
            ConfigManager. shrinkFactorVeryTall.SettingChanged += OnSettingChanged;

            ConfigManager.shouldKeepShrunk.SettingChanged += OnSettingChanged;
            ConfigManager.shouldChangingMass.SettingChanged += OnShouldChangingMassSettingChanged;
            ConfigManager. shrinkMassValue.SettingChanged += OnSettingChanged;
            
            ConfigManager.shouldInstantKillEnemyInCart.SettingChanged += OnInstantKillSettingChanged;
            ConfigManager.shouldCartWeaponBatteryLifeInfinite.SettingChanged +=
                OnShouldItemBatteryLifeInfiniteSettingChanged;
            ConfigManager.shouldItemGunBatteryLifeInfinite.SettingChanged +=
                OnShouldItemBatteryLifeInfiniteSettingChanged;
            ConfigManager.shouldItemMeleeBatteryLifeInfinite.SettingChanged +=
                OnShouldItemBatteryLifeInfiniteSettingChanged;
            ConfigManager.shouldItemDroneBatteryLifeInfinite.SettingChanged +=
                OnShouldItemBatteryLifeInfiniteSettingChanged;
            ConfigManager.shouldItemGenericBatteryLifeInfinite.SettingChanged +=
                OnShouldItemBatteryLifeInfiniteSettingChanged;
        }

        public static void RefreshingConfigToClients()
        {
            ShrinkerCartPatch.PushConfigToClients();
        }

        public static void OnSettingChanged(object sender, EventArgs e)
        {
            RefreshingConfigToClients();
        }

        public static void OnInstantKillSettingChanged(object sender, EventArgs e)
        {
            RefreshingConfigToClients();
            RefreshEnemiesInAllCarts();
        }

        public static void RefreshEnemiesInAllCarts()
        {
            if (!RunManagerHelper.IsInsideValidLevel()) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;
            
            LogWrapper.Warning(
                "[Config Refresh] ⚙️ Mise à jour des ennemis dans les CARTs suite à changement d'option");

            if (!StaticConfig.Instance.shouldInstantKillEnemyInCart) return;

            var trackers = ShrinkTrackerManager.Instance.GetTrackersInCart().ToList();
            if (!trackers.Any()) return;

            foreach (var tracker in trackers)
            {
                EnemyExecutionManager.TryMarkForExecution(tracker);
                LogWrapper.Info($"💀 Exécution instantanée forcée de {tracker.GrabObject.name}");
            }
        }

        public static void OnShouldItemBatteryLifeInfiniteSettingChanged(object sender, EventArgs e)
        {
            RefreshingConfigToClients();
            RefreshItemBatteryInAllCarts();
        }

        public static void RefreshItemBatteryInAllCarts()
        {
            if (!RunManagerHelper.IsInsideValidLevel()) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;
            
            LogWrapper.Warning("⚙️ Mise à jour des Batteries");

            var trackers = ShrinkTrackerManager.Instance.GetAll().ToList();
            if (!trackers.Any()) return;

            foreach (var tracker in trackers)
            {
                ShrinkBatteryUtils.ApplyBatteryLifeAll(tracker);
            }
        }

        public static void OnShouldChangingMassSettingChanged(object sender, EventArgs e)
        {
            RefreshingConfigToClients();
            RefreshItemMassInAllCarts();
        }

        public static void RefreshItemMassInAllCarts()
        {
            if (!RunManagerHelper.IsInsideValidLevel()) return;
            if (!SemiFunc.IsMasterClientOrSingleplayer()) return;
            
            LogWrapper.Warning("[Config Refresh] ⚙️ Mise à jour des Mass pour les Items");

            var trackers = ShrinkTrackerManager.Instance.GetAll().ToList();
            if (!trackers.Any()) return;
            
            float newMass = StaticConfig.Instance.shrinkMassValue;

            foreach (var tracker in trackers)
            {
                if (tracker.IsValidShrinkableItem())
                {
                    if (StaticConfig.Instance.shouldChangingMass)
                    {
                        tracker.ApplyMass(newMass);
                        LogWrapper.Warning($"[Config Refresh] ⚙️ La mass de l'objet {tracker.GrabObject.name} est maintenant de {newMass}");
                    }
                    else
                    {
                        tracker.RestoreMass();
                        LogWrapper.Warning($"[Config Refresh] ⚙️ La mass de l'objet {tracker.GrabObject.name} est maintenant de {tracker.InitialMass}");
                    }
                }
            }
        }
    }
}