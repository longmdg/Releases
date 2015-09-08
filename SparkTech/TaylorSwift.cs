namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;

    using SharpDX;

    internal class TaylorSwift
    {
        public Render.Sprite Hud;

        public TaylorSwift()
        {
            Hud = new Render.Sprite(Properties.Resources.TaySwift, new Vector2(1, 1));
            Hud.Add();
        }
    }
}