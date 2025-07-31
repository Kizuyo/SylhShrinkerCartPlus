using UnityEngine;

namespace SylhShrinkerCartPlus.Utils
{
    public static class ItemCartWeaponUtils
    {
        public static bool TryChangeCartWeaponBatteryLife(PhysGrabObject obj)
        {
            if (obj == null)
                return false;

            var weaponTypes = new[]
            {
                typeof(ItemCartCannon),
                typeof(ItemCartLaser),
            };

            foreach (var type in weaponTypes)
            {
                Component weapon = obj.GetComponent(type);
                if (weapon == null) continue;

                ItemBattery itemBattery = weapon.GetComponent<ItemBattery>();
                if (itemBattery == null) return false;

                ItemBatteryUtils.SetBatteryLife(itemBattery);
                return true;
            }

            return false;
        }
    }
}