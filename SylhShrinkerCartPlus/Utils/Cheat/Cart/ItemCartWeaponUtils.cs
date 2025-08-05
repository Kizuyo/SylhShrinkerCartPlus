using UnityEngine;

namespace SylhShrinkerCartPlus.Utils.Cheat.Cart
{
    public static class ItemCartWeaponUtils
    {
        public static bool TryGetCartWeapon(PhysGrabObject obj, out Component? component)
        {
            if (obj == null)
            {
                component = null;
                return false;
            }

            var weaponTypes = new[] { typeof(ItemCartCannon), typeof(ItemCartLaser) };

            foreach (var type in weaponTypes)
            {
                Component weapon = obj.GetComponent(type);
                if (weapon != null)
                {
                    component = weapon;
                    return true;
                }
            }

            component = null;
            return false;
        }
        
        public static bool TryGetCartWeaponBattery(PhysGrabObject obj, out ItemBattery? battery)
        {
            var weaponTypes = new[]
            {
                typeof(ItemCartCannon),
                typeof(ItemCartLaser),
            };
            
            if (obj == null)
            {
                battery = null;
                return false;
            }
            
            foreach (var type in weaponTypes)
            {
                Component weapon = obj.GetComponent(type);
                if (weapon == null) continue;

                ItemBattery itemBattery = weapon.GetComponent<ItemBattery>();
                if (itemBattery == null) continue;

                battery = itemBattery;
                return true;
            }

            battery = null;
            return false;
        }
    }
}