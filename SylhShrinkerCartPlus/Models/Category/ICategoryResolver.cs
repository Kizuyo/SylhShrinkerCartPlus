namespace SylhShrinkerCartPlus.Models
{
    public interface ICategoryResolver
    {
        ValuableCategoryBase? ResolveCategory(PhysGrabObject item);
    }
}