namespace SparkTech.Base
{
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu;

    using SparkTech.Executors;

    using static System.String;

    public class SparkSpell : Spell
    {
        static SparkSpell()
        {
            
        }

        private static Obj_AI_Hero Player => GameObjects.Player;

        private new static bool IsSkillshot(SpellType type)
        {
            var skillshotTypes = new[]
                                 {
                                     SpellType.SkillshotArc, SpellType.SkillshotCircle, SpellType.SkillshotCone,
                                     SpellType.SkillshotLine, SpellType.SkillshotMissileArc,
                                     SpellType.SkillshotMissileCircle, SpellType.SkillshotMissileCone,
                                     SpellType.SkillshotMissileLine, SpellType.SkillshotRing, SpellType.Vector,
                                     SpellType.SkillshotArc
                                 };

            return skillshotTypes.Contains(type);
        }

        private static Menu menu;

        /// <summary>
        /// The submenu to be added to the root
        /// </summary>
        Menu IMenuPiece.Piece
        {
            get
            {
                if (menu == null)
                {
                    menu = new Menu("st_core_spell", Empty);

                    foreach (
                        var entry in
                            SpellDatabase.Spells.Where(
                                entry => entry.ChampionName == Player.ChampionName && IsSkillshot(entry.SpellType)))
                    {

                    }

                    //    SpellSlot[] slots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

                    //    foreach (var slot in slots.Where(x => Player.Spellbook.GetSpell(x).SData))
                    //    {
                    //        var inst = Player.Spellbook.GetSpell(slot);


                    //    }
                }

                return menu;
            }
        }

        public SparkSpell(SpellSlot slot, bool loadFromGame, HitChance hitChance = HitChance.Medium) : base(slot, loadFromGame, hitChance)
        {

        }

        public SparkSpell(SpellSlot slot, float range = float.MaxValue) : base(slot, range)
        {

        }
    }
}