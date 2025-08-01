using SylhShrinkerCartPlus.Utils;
using UnityEngine;

namespace SylhShrinkerCartPlus.Models
{
    public class StandardValuableCategoryResolver : ICategoryResolver
    {
        private static readonly Dictionary<Func<LevelValuables, List<GameObject>>, ValuableCategoryEnum> _mappings =
            new()
            {
                { v => v.tiny, ValuableCategoryEnum.Tiny },
                { v => v.small, ValuableCategoryEnum.Small },
                { v => v.medium, ValuableCategoryEnum.Medium },
                { v => v.big, ValuableCategoryEnum.Big },
                { v => v.wide, ValuableCategoryEnum.Wide },
                { v => v.tall, ValuableCategoryEnum.Tall },
                { v => v.veryTall, ValuableCategoryEnum.VeryTall },
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
