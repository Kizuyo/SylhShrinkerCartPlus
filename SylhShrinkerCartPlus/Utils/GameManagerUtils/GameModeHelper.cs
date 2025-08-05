namespace SylhShrinkerCartPlus.Utils.GameManagerUtils
{
    public static class GameModeHelper
    {
        public static bool IsSinglePlayer => GameManager.instance.gameMode == 0;

        public static bool IsMultiplayer => GameManager.instance.gameMode == 1;
    }
}
