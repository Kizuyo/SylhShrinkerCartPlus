namespace SylhShrinkerCartPlus.Utils
{
    public class NameUtils
    {
        public static string CleanName(string name)
        {
            return name.Replace("Valuable ", "").Replace("(Clone)", "").Trim();
        }
        
        public static string CleanCloneSuffix(string name)
        {
            return name.Replace("(Clone)", "").Trim();
        }
        
        public static bool ContainsIgnoreCase(
            string source, 
            string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase
            )
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static bool TryParseEnemyValuable(
            string name,
            out string baseName,
            out string type
        )
        {
            baseName = null;
            type = null;

            // Clean the name first
            string cleanName = CleanName(name);

            if (!cleanName.StartsWith("Enemy")) return false;

            var parts = cleanName.Split('-');
            if (parts.Length < 2) return false;

            baseName = parts[0].Trim(); // "Enemy"
            type = parts[1].Trim(); // "Big", "Medium", or "Small"

            return true;
        }
    }
}

