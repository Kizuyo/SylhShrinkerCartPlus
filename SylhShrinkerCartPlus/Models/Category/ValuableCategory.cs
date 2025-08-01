namespace SylhShrinkerCartPlus.Models
{
    public abstract class ValuableCategoryBase
    {
        public abstract string DisplayName { get; }
    }

    public sealed class ValuableCategory : ValuableCategoryBase
    {
        public ValuableCategoryEnum Category { get; }

        public ValuableCategory(ValuableCategoryEnum category)
        {
            Category = category;
        }

        public override string DisplayName => Category.ToString();
    }

    public sealed class EnemyValuableCategory : ValuableCategoryBase
    {
        public EnemyValuableCategoryEnum Category { get; }

        public EnemyValuableCategory(EnemyValuableCategoryEnum category)
        {
            Category = category;
        }

        public override string DisplayName => Category.ToString();
    }

    public enum ValuableCategoryEnum
    {
        Tiny,
        Small,
        Medium,
        Big,
        Wide,
        Tall,
        VeryTall,
    }

    public enum EnemyValuableCategoryEnum
    {
        Small,
        Medium,
        Big,
    }
}