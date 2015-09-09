namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class Extensions
    {
        public Render.Sprite Hud;

        public Extensions()
        {
            Hud = new Render.Sprite(Properties.Resources.TaySwift, new Vector2(1, 1));
            Hud.Add();
        }
    }
}