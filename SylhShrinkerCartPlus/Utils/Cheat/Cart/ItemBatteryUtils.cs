using UnityEngine;

namespace SylhShrinkerCartPlus.Utils
{
    public static class ItemBatteryUtils
    {
        private const int DefaultBatteryLife = 9999;

        private static void TryFallbackBattery(PhysGrabObject obj, int batteryLife)
        {
            if (obj.TryGetComponent<ItemBattery>(out var battery))
            {
                LogWrapper.Warning($"[FallbackBattery] Using fallback ItemBattery on '{obj.name}' (missing expected component)");
                SetBatteryLife(battery, batteryLife);
            }
        }
        
        public static void SetBatteryLife(ItemBattery itemBattery, int batteryLife = DefaultBatteryLife)
        {
            if (itemBattery == null) return;

            itemBattery.batteryDrainRate = 0f;
            itemBattery.SetBatteryLife(batteryLife);
        }

        public static void SetMeleeBatteryLife(PhysGrabObject obj, int batteryLife = DefaultBatteryLife)
        {
            if (obj.TryGetComponent<ItemMelee>(out var melee))
            {
                melee.durabilityDrainOnEnemiesAndPVP = 0f;
                SetBatteryLife(melee.GetComponent<ItemBattery>(), batteryLife);
            }
            else
            {
                TryFallbackBattery(obj, batteryLife);
            }
        }

        public static void SetGunBatteryLife(PhysGrabObject obj, int batteryLife = DefaultBatteryLife)
        {
            if (obj.TryGetComponent<ItemGun>(out var gun))
            {
                gun.batteryDrain = 0f;
                SetBatteryLife(gun.GetComponent<ItemBattery>(), batteryLife);
            }
            else
            {
                TryFallbackBattery(obj, batteryLife);
            }
        }

        public static void SetDroneBatteryLife(PhysGrabObject obj, int batteryLife = DefaultBatteryLife)
        {
            if (obj.TryGetComponent<ItemDrone>(out var drone))
            {
                SetBatteryLife(drone.GetComponent<ItemBattery>(), batteryLife);
            }
            else
            {
                TryFallbackBattery(obj, batteryLife);
            }
        }
    }
}