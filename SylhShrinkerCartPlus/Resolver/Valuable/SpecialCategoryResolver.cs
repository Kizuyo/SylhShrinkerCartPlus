namespace SylhShrinkerCartPlus.Resolver.Valuable
{
    public class SpecialCategoryResolver : ICategoryResolver
    {
        public ValuableCategoryBase? ResolveCategory(PhysGrabObject item)
        {
            if (item.GetComponent<SurplusValuable>())
            {
                return new SpecialValuableCategory(SpecialValuableCategoryEnum.SurplusValuable);
            }

            return null;
        }
    }

}