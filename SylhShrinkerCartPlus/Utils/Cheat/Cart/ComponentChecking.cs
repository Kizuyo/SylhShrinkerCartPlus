using UnityEngine;

namespace SylhShrinkerCartPlus.Utils.Cheat.Cart
{
    public static class ComponentChecking
    {
        public static bool IsValid(Component component)
        {
            string itemName = NameUtils.CleanName(component.name);
            
            // Checking Enemy Valuable
            if (NameUtils.TryParseEnemyValuable(itemName, out string name, out var type)) return true;
            
            // Checking Valuable
            if (component.GetComponent<ValuableObject>()) return true;
            
            // Checking valid ItemWeapon
            if (component.GetComponent<ItemMelee>()) return true;
            if (component.GetComponent<ItemGun>()) return true;
            if (component.GetComponent<ItemDrone>()) return true;
            if (component.GetComponent<ItemBattery>()) return true;
            
            // Checking Cart Weapon
            if (component.GetComponent<ItemCartCannon>()) return true;
            if (component.GetComponent<ItemCartLaser>()) return true;

            return false;
        }
    }
}

