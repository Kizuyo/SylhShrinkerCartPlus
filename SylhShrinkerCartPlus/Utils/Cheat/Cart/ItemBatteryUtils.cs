namespace SylhShrinkerCartPlus.Utils.Cheat.Cart
{
    public static class ItemBatteryUtils
    {
        public static ItemBattery? TryGetBattery(PhysGrabObject item, out ItemBattery? battery)
        {
            if (item == null)
            {
                battery = null;
                return battery;
            }

            if (ItemCartWeaponUtils.TryGetCartWeaponBattery(item, out ItemBattery? canonBattery))
            {
                battery = canonBattery;
                return battery;
            }

            if (item.TryGetComponent<ItemMelee>(out var melee))
            {
                battery = melee.GetComponent<ItemBattery>();
                return battery;
            }

            if (item.TryGetComponent<ItemGun>(out var gun))
            {
                battery = gun.GetComponent<ItemBattery>();
                return battery;
            }

            if (item.TryGetComponent<ItemDrone>(out var drone))
            {
                battery = drone.GetComponent<ItemBattery>();
                return battery;
            }

            if (item.TryGetComponent<ItemBattery>(out var genericBattery))
            {
                battery = genericBattery;
                return battery;
            }

            battery = null;
            return battery;
        }
    }
}