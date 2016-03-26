namespace SparkTech.Base
{
    using System;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu;

    using SparkTech.Executors;

    public class SparkSpell : Spell //, IMenuPiece
    {
        //static SparkSpell()
        //{
            
        //}

        //private static Obj_AI_Hero Player => GameObjects.Player;

        //private static Menu menu;

        ///// <summary>
        ///// The submenu to be added to the root
        ///// </summary>
        //Menu IMenuPiece.Piece
        //{
        //    get
        //    {
        //        if (menu == null)
        //        {
        //            menu = new Menu("st_core_spell", string.Empty);

        //            foreach (
        //                var entry in
        //                    SpellDatabase.Spells.Where(
        //                        entry => entry.ChampionName == Player.ChampionName && IsSkillshot(entry.SpellType)))
        //            {

        //            }

        //            //    SpellSlot[] slots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        //            //    foreach (var slot in slots.Where(x => Player.Spellbook.GetSpell(x).SData))
        //            //    {
        //            //        var inst = Player.Spellbook.GetSpell(slot);


        //            //    }
        //        }

        //        return menu;
        //    }
        //}

        public SparkSpell(SpellSlot slot, bool loadFromGame, HitChance hitChance = HitChance.Medium) : base(slot, loadFromGame, hitChance)
        {

        }

        public SparkSpell(SpellSlot slot, float range = float.MaxValue) : base(slot, range)
        {

        }
    }
}