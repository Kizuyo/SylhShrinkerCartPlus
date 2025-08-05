using SylhShrinkerCartPlus.Manager;
using SylhShrinkerCartPlus.Utils.Shrink;
using SylhShrinkerCartPlus.Utils.Shrink.Config;
using SylhShrinkerCartPlus.Utils.Shrink.Utils.Cheat.Enemy;

namespace SylhShrinkerCartPlus.Utils.Events
{
    public static class ConfigEvents
    {
        public static void Initialize()
        {
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

            ConfigManager.shouldChangingMass.SettingChanged += OnShouldChangingMassSettingChanged;

            // Ajoute d'autres abonnements ici...
        }

        public static void OnInstantKillSettingChanged(object sender, EventArgs e)
        {
            RefreshEnemiesInAllCarts();
        }

        public static void RefreshEnemiesInAllCarts()
        {
            LogWrapper.Warning(
                "[Config Refresh] ⚙️ Mise à jour des ennemis dans les CARTs suite à changement d'option");

            if (!ConfigManager.shouldInstantKillEnemyInCart.Value) return;

            var trackers = ShrinkTrackerManager.Instance.GetTrackersInCart().ToList();
            if (!trackers.Any()) return;

            foreach (var tracker in trackers)
            {
                EnemyExecutionManager.TryMarkForExecution(tracker);
                LogWrapper.Info($"[Config Refresh] 💀 Exécution instantanée forcée de {tracker.GrabObject.name}");
            }
        }

        public static void OnShouldItemBatteryLifeInfiniteSettingChanged(object sender, EventArgs e)
        {
            RefreshItemBatteryInAllCarts();
        }

        public static void RefreshItemBatteryInAllCarts()
        {
            LogWrapper.Warning("[Config Refresh] ⚙️ Mise à jour des Batteries");

            var trackers = ShrinkTrackerManager.Instance.GetAll().ToList();
            if (!trackers.Any()) return;

            foreach (var tracker in trackers)
            {
                ShrinkBatteryUtils.ApplyBatteryLifeAll(tracker);
            }
        }

        public static void OnShouldChangingMassSettingChanged(object sender, EventArgs e)
        {
            RefreshItemMassInAllCarts();
        }

        public static void RefreshItemMassInAllCarts()
        {
            LogWrapper.Warning("[Config Refresh] ⚙️ Mise à jour des Mass pour les Items");

            var trackers = ShrinkTrackerManager.Instance.GetAll().ToList();
            if (!trackers.Any()) return;
            
            float newMass = ConfigManager.shrinkMassValue.Value;

            foreach (var tracker in trackers)
            {
                if (tracker.IsValidShrinkableItem())
                {
                    if (ConfigManager.shouldChangingMass.Value)
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