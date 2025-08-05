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

            ConfigManager.shouldValuableSafeInsideCart.SettingChanged += OnShouldValuableSafeInsideCartSettingChanged;
            ConfigManager.shouldValuableStayUnbreakable.SettingChanged += OnShouldValuableStayUnbreakableSettingChanged;
            
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

        public static void OnShouldValuableSafeInsideCartSettingChanged(object sender, EventArgs e)
        {
            RefreshValuablesToBecomeSafeInsideCart();
        }

        public static void RefreshValuablesToBecomeSafeInsideCart()
        {
            LogWrapper.Warning("[Config Refresh] ⚙️ Mise à jour des Items pour qu'ils deviennent safe dans les chariots");

            var trackers = ShrinkTrackerManager.Instance.GetTrackersInCart().ToList();
            if (!trackers.Any()) return;
            
            foreach (var tracker in trackers)
            {
                if (ConfigManager.shouldValuableSafeInsideCart.Value)
                {
                    tracker.MakeUnbreakable();
                    LogWrapper.Warning($"[Config Refresh] ⚙️ L'objet {tracker.GrabObject.name} est maintenant incassable dans un chariot");
                }
                else
                {
                    tracker.MakeBreakable();
                    LogWrapper.Warning($"[Config Refresh] ⚙️ L'objet {tracker.GrabObject.name} n'est plus incassable dans un chariot");
                }
            }
        }

        public static void OnShouldValuableStayUnbreakableSettingChanged(object sender, EventArgs e)
        {
            RefreshValuablesToStaySafeOutsideCart();
        }

        public static void RefreshValuablesToStaySafeOutsideCart()
        {
            LogWrapper.Warning("[Config Refresh] ⚙️ Mise à jour des Items pour qu'ils restent safe en dehors des chariots");
            
            var trackers = ShrinkTrackerManager.Instance.GetAll().ToList();
            if (!trackers.Any()) return;
            
            foreach (var tracker in trackers)
            {
                if (ConfigManager.shouldValuableStayUnbreakable.Value && ConfigManager.shouldValuableSafeInsideCart.Value)
                {
                    tracker.MakeUnbreakable();
                    LogWrapper.Warning($"[Config Refresh] ⚙️ L'objet {tracker.GrabObject.name} est maintenant incassable");
                }

                if (!ConfigManager.shouldValuableStayUnbreakable.Value)
                {
                    if (!tracker.IsInCart()) tracker.MakeBreakable();
                }
            }
        }
    }
}