namespace SparkTech.Utils
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;

    public static class ManaManager
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        private static readonly string[] ResourcelessChampions =
            {
                "Aatrox", "DrMundo", "Mordekaiser", "Vladimir", "Zac",
                "Akali", "Kennen", "LeeSin", "Shen", "Zed", "Garen",
                "Gnar", "Katarina", "RekSai", "Renekton", "Rengar",
                "Riven", "Rumble", "Shyvana", "Tryndamere", "Yasuo"
            };

        private static readonly bool NoMana = ResourcelessChampions.Contains(Player.CharData.BaseSkinName);

        public static bool IsWorthCasting(this Spell spell, Obj_AI_Hero target)
        {
            if (NoMana) return true;

            float myLostManaPercent = Player.ManaPercent - (Player.Mana - spell.Instance.ManaCost) / Player.MaxMana * 100;
            float enemyLostHealthPercent = target.HealthPercent - target.Health - spell.GetDamage(target) / target.MaxHealth * 100;

            return myLostManaPercent < enemyLostHealthPercent;
        }
    }
}