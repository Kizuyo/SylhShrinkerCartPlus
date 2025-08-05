using SylhShrinkerCartPlus.Resolver.Valuable;

namespace SylhShrinkerCartPlus.Resolver.Valuable
{
    public static class CategoryResolverRegistry
    {
        private static readonly List<ICategoryResolver> _resolvers = new();

        public static void Register(ICategoryResolver resolver)
        {
            _resolvers.Add(resolver);
        }

        public static ValuableCategoryBase? Resolve(PhysGrabObject item)
        {
            foreach (var resolver in _resolvers)
            {
                var result = resolver.ResolveCategory(item);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}