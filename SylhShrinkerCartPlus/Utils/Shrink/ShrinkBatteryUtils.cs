using SylhShrinkerCartPlus.Components;
using SylhShrinkerCartPlus.Config;
using SylhShrinkerCartPlus.Utils.Cheat.Cart;
using UnityEngine;

namespace SylhShrinkerCartPlus.Utils.Shrink
{
    public static class ShrinkBatteryUtils
    {
        public static int BigBatteryLife = 9999;

        public static void ApplyBatteryLifeAll(ShrinkableTracker tracker)
        {
            ApplyCartWeaponBattery(tracker);

            ApplyBatteryLogic<ItemMelee>(tracker, StaticConfig.Instance.shouldItemMeleeBatteryLifeInfinite);
            ApplyBatteryLogic<ItemGun>(tracker, StaticConfig.Instance.shouldItemGunBatteryLifeInfinite);
            ApplyBatteryLogic<ItemDrone>(tracker, StaticConfig.Instance.shouldItemDroneBatteryLifeInfinite);

            ApplyGenericBattery(tracker);
        }

        private static void ApplyCartWeaponBattery(ShrinkableTracker tracker)
        {
            if (!ItemCartWeaponUtils.TryGetCartWeaponBattery(
                    tracker.GrabObject,
                    out var battery
                ) || battery == null)
                return;

            if (StaticConfig.Instance.shouldCartWeaponBatteryLifeInfinite)
            {
                SetBatteryLife(battery, BigBatteryLife);
                tracker.CanResetBattery = true;
            }
            else if (tracker.CanResetBattery)
            {
                SetBatteryLife(battery, (int)tracker.BatteryLife);
                tracker.CanResetBattery = false;
            }
        }

        private static void ApplyBatteryLogic<T>(ShrinkableTracker tracker, bool shouldForceInfinite)
            where T : Component
        {
            if (!tracker.TryGetComponent<T>(out var _)) return;
            if (!tracker.TryGetComponent<ItemBattery>(out var battery)) return;

            if (shouldForceInfinite)
            {
                SetBatteryLife(battery, BigBatteryLife);
                tracker.CanResetBattery = true;
            }
            else if (tracker.CanResetBattery)
            {
                SetBatteryLife(battery, (int)tracker.BatteryLife);
                tracker.CanResetBattery = false;
            }
        }

        private static void ApplyGenericBattery(ShrinkableTracker tracker)
        {
            // Exclusion des types déjà gérés
            if (ItemCartWeaponUtils.TryGetCartWeapon(tracker.GrabObject, out _)) return;
            if (tracker.GetComponent<ItemMelee>() != null || tracker.GetComponent<ItemGun>() != null ||
                tracker.GetComponent<ItemDrone>() != null) return;
            if (tracker.GetComponent<ItemGrenade>()) return;
            
            if (!tracker.TryGetComponent<ItemBattery>(out var battery)) return;

            if (StaticConfig.Instance.shouldItemGenericBatteryLifeInfinite)
            {
                SetBatteryLife(battery, BigBatteryLife);
                tracker.CanResetBattery = true;
            }
            else if (tracker.CanResetBattery)
            {
                SetBatteryLife(battery, (int)tracker.BatteryLife);
                tracker.CanResetBattery = false;
            }
        }

        public static void SetBatteryLife(ItemBattery battery, int amount = 9999)
        {
            battery.batteryLife = amount;
            battery.SetBatteryLife(amount);
        }
    }
}