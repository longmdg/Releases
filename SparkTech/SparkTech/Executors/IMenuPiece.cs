namespace SparkTech.Executors
{
    using LeagueSharp.SDK.Core.UI.IMenu;

    /// <summary>
    /// The devired class will add a submenu to the core menu
    /// </summary>
    internal interface IMenuPiece
    {
        /// <summary>
        /// The submenu to be added to the root
        /// </summary>
        Menu Piece { get; }
    }
}