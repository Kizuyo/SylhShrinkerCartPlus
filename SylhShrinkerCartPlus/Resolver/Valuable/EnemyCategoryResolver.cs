using SylhShrinkerCartPlus.Utils;

namespace SylhShrinkerCartPlus.Resolver.Valuable
{
    public class EnemyCategoryResolver : ICategoryResolver
    {
        private static readonly Dictionary<string, EnemyValuableCategoryEnum> _mappings =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "Small", EnemyValuableCategoryEnum.Small },
                { "Medium", EnemyValuableCategoryEnum.Medium },
                { "Big", EnemyValuableCategoryEnum.Big },
                { "Berserker", EnemyValuableCategoryEnum.Berserker }
            };

        public ValuableCategoryBase? ResolveCategory(PhysGrabObject item)
        {
            string itemName = NameUtils.CleanName(item.name);

            if (!NameUtils.TryParseEnemyValuable(itemName, out _, out var enemyType))
                return null;

            return _mappings.TryGetValue(enemyType, out var category)
                ? new EnemyValuableCategory(category)
                : null;
        }
    }

}