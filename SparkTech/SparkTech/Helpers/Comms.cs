namespace SparkTech.Helpers
{
    using LeagueSharp;
    using LeagueSharp.SDK.Core.UI.IMenu;

    using SparkTech.Executors;

    /// <summary>
    /// The user - developer - assembly communication class
    /// </summary>
    public class Comms : IMenuPiece
    {
        /// <summary>
        /// The submenu to be added to the root
        /// </summary>
        public Menu Piece
        {
            get
            {
                var menu = new Menu("st_core_comms", "Comms");
                {
                    
                }

                return menu;
            }
        }

        /// <summary>
        /// Notifies the message to the user based on his selected settings
        /// </summary>
        /// <param name="text"></param>
        // TODO expand, add menu items, checks, notifications and colors.
        public static void Print(string text)
        {
            Game.PrintChat(text);
        }

        public static void Debug(string memberName, string value)
        {
            if (!string.IsNullOrEmpty(memberName))
            {
                value = $"\"{memberName}\" - {value}";
            }
        }
    }
}