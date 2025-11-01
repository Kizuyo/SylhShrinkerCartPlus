using SylhShrinkerCartPlus.Utils;
using UnityEngine;

namespace SylhShrinkerCartPlus.Resolver.Valuable
{
    public class StandardValuableCategoryResolver : ICategoryResolver
    {
        private static readonly Dictionary<Func<LevelValuables, List<GameObject>>, ValuableCategoryEnum> _mappings =
            new()
            {
                // REPO v0.3.0 Compatibility
                // LevelValuables stores PrefabRef objects instead of GameObjects; extract PrefabRef.Prefab so mappings return actual prefab GameObjects
                { v => v.tiny.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.Tiny },
                { v => v.small.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.Small },
                { v => v.medium.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.Medium },
                { v => v.big.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.Big },
                { v => v.wide.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.Wide },
                { v => v.tall.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.Tall },
                { v => v.veryTall.Select(p => p.Prefab).ToList(), ValuableCategoryEnum.VeryTall },
            };

        public ValuableCategoryBase? ResolveCategory(PhysGrabObject item)
        {
            string itemName = NameUtils.CleanName(item.name);

            foreach (var levelValuables in RunManager.instance.levelCurrent.ValuablePresets)
            {
                foreach (var mapping in _mappings)
                {
                    var list = mapping.Key(levelValuables);
                    if (list.Any(v => NameUtils.CleanName(v.name) == itemName))
                    {
                        return new ValuableCategory(mapping.Value);
                    }
                }
            }

            return null;
        }
    }

}
