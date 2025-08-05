namespace SylhShrinkerCartPlus.Resolver.Valuable
{
    public interface ICategoryResolver
    {
        ValuableCategoryBase? ResolveCategory(PhysGrabObject item);
    }
}