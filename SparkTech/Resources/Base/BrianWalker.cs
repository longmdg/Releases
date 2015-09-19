#region Credits

/*

                                                                      HUGE CREDITS TO BRIAN FOR

                                                                       - Coding that good shit
                                                              - Letting me use it and tune to my needs
                                                                We (or at least I) love you man :)
                                                  Give him a 5 - star rating in his profile, he totally deserved that!
                                                            https://www.joduska.me/forum/user/2586-brian/

*/

#endregion

namespace SparkTech.Resources.Base
{
    using System;
    using System.Diagnostics.CodeAnalysis; // TODO .NET 4.6 pls
    using System.Linq;
    using LeagueSharp;
    using LeagueSharp.Common;
    using SharpDX;

    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    internal class BrianWalker
    {
        private static bool CanMove { get { return missileLaunched || Utils.GameTimeTickCount + Game.Ping / 2 >= lastAttack + Player.AttackCastDelay * 1000 + GetCurrentWindupTime; } }
        private static int GetCurrentWindupTime { get { return config.Item("ST_Misc_ExtraWindUp").GetValue<Slider>().Value; } }
        private static bool IsAllowedToAttack { get { return ((CurrentMode != Mode.Combo && CurrentMode != Mode.Harass && CurrentMode != Mode.LaneClear) || config.Item("ST_" + CurrentMode + "_Attack").IsActive()) && (CurrentMode != Mode.LastHit || config.Item("ST_LastHit_Attack").IsActive()) && (Attack && !config.Item("ST_Misc_AllAttackDisabled").IsActive()); } }
        private static bool IsAllowedToMove { get { return ((CurrentMode != Mode.Combo && CurrentMode != Mode.Harass && CurrentMode != Mode.LaneClear) || config.Item("ST_" + CurrentMode + "_Move").IsActive()) && (Move && !config.Item("ST_Misc_AllMovementDisabled").IsActive()) && (CurrentMode != Mode.LastHit || config.Item("ST_LastHit_Move").IsActive()); } }
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private static Menu config;
        private static bool disableNextAttack, missileLaunched;
        private static int lastAttack, lastMove;
        private static AttackableUnit lastTarget;
        private static Obj_AI_Minion prevMinion;
        private static readonly Spell MovePrediction = new Spell(SpellSlot.Unknown, GetAutoAttackRange());
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public static bool Attack { get; set; }
        public static bool Move { get; set; }
        public static bool CanAttack { get { return Utils.GameTimeTickCount + Game.Ping / 2 + 25 >= lastAttack + Player.AttackDelay * 1000; } }
        public static Mode CurrentMode { get { return config.Item("ST_Combo_Key").IsActive() ? Mode.Combo : (config.Item("ST_Clear_Key").IsActive() ? Mode.LaneClear : (config.Item("ST_Harass_Key").IsActive() ? Mode.Harass : (config.Item("ST_LastHit_Key").IsActive() ? Mode.LastHit : (config.Item("ST_Flee_Key").IsActive() ? Mode.Flee : Mode.None)))); } }
        public static Obj_AI_Hero ForcedTarget = null;

        public enum Mode
        {
            Combo, Harass, LaneClear, LastHit, Flee, None
        }

        public static Obj_AI_Hero GetBestHeroTarget
        {
            get
            {
                Obj_AI_Hero killableObj = null;
                var hitsToKill = double.MaxValue;
                foreach (var obj in HeroManager.Enemies.Where(i => InAutoAttackRange(i)))
                {
                    var killHits = obj.Health / Player.GetAutoAttackDamage(obj, true);
                    if (killableObj != null && (killHits >= hitsToKill || obj.HasBuffOfType(BuffType.Invulnerability)))
                        continue;
                    killableObj = obj;
                    hitsToKill = killHits;
                }
                return hitsToKill < 4 ? killableObj : TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
            }
        }

        private static float GetAutoAttackRange(Obj_AI_Base source, AttackableUnit target)
        {
            return source.AttackRange + source.BoundingRadius + (target.IsValidTarget() ? target.BoundingRadius : 0);
        }

        // TODO: Improve Lane Clear logic (1/2)
        private static bool ShouldWait
        {
            get
            {
                return ObjectManager.Get<Obj_AI_Minion>().Any(i => InAutoAttackRange(i) && i.Team != GameObjectTeam.Neutral && HealthPrediction.GetHealthPrediction(i, (int)(Player.AttackDelay * 1000 * 2), 0) <= Player.GetAutoAttackDamage(i, true));
            }
        }

        public static float GetAutoAttackRange(AttackableUnit target = null)
        {
            return GetAutoAttackRange(Player, target);
        }

        public static bool InAutoAttackRange(AttackableUnit target, float extraRange = 0, Vector3 from = new Vector3())
        {
            return target.IsValidTarget(GetAutoAttackRange(target) + extraRange, true, from);
        }

        public static AttackableUnit GetPossibleTarget
        {
            get
            {
                if (!config.Item("ST_Misc_PriorityFarm").IsActive() && (CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear))
                {
                    var hero = GetBestHeroTarget;
                    if (hero.IsValidTarget())
                        return hero;
                }
                if (CurrentMode == Mode.Harass || CurrentMode == Mode.LaneClear || CurrentMode == Mode.LastHit)
                    foreach (var obj in ObjectManager.Get<Obj_AI_Minion>().Where(i => InAutoAttackRange(i) && i.Team != GameObjectTeam.Neutral && (MinionManager.IsMinion(i, true) || Helper.IsPet(i)) && i.Health < 2 * Player.TotalAttackDamage).OrderByDescending(i => i.CharData.BaseSkinName.Contains("Siege")).ThenBy(i => i.CharData.BaseSkinName.Contains("Super")).ThenBy(i => i.Health).ThenByDescending(i => i.MaxHealth))
                    {
                        var time = (int)(Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + (int)(Player.Distance(obj) / Orbwalking.GetMyProjectileSpeed() * 1000);
                        var hpPred = HealthPrediction.GetHealthPrediction(obj, time, 0);
                        if (hpPred < 1) FireOnNonKillableMinion(obj);
                        if (hpPred > 0 && hpPred <= Player.GetAutoAttackDamage(obj, true))
                            return obj;
                    }
                if (InAutoAttackRange(ForcedTarget))
                    return ForcedTarget;
                if (CurrentMode == Mode.LaneClear)
                {
                    foreach (var obj in ObjectManager.Get<Obj_AI_Turret>().Where(obj => InAutoAttackRange(obj) && obj.IsValidTarget())) return obj;
                    foreach (var obj in ObjectManager.Get<Obj_BarracksDampener>().Where(obj => InAutoAttackRange(obj) && obj.IsValidTarget())) return obj;
                    foreach (var obj in ObjectManager.Get<Obj_HQ>().Where(obj => InAutoAttackRange(obj) && obj.IsValidTarget())) return obj;
                }
                if (CurrentMode != Mode.LastHit)
                {
                    var hero = GetBestHeroTarget;
                    if (hero.IsValidTarget())
                        return hero;
                }
                if (CurrentMode == Mode.LaneClear || CurrentMode == Mode.Harass)
                {
                    var mob = ObjectManager.Get<Obj_AI_Minion>().Where(i => InAutoAttackRange(i) && i.Team == GameObjectTeam.Neutral && i.CharData.BaseSkinName != "gangplankbarrel").MaxOrDefault(i => i.MaxHealth);
                    if (mob != null)
                        return mob;
                    
                }
                if (CurrentMode != Mode.LaneClear || ShouldWait)
                    return null;

                // TODO: Improve Lane Clear logic (2/2)

                if (InAutoAttackRange(prevMinion))
                {
                    var hpPred = HealthPrediction.LaneClearHealthPrediction(prevMinion, (int)(Player.AttackDelay * 1000 * 2), 0);
                    if (hpPred >= 2 * Player.GetAutoAttackDamage(prevMinion, true) || Math.Abs(hpPred - prevMinion.Health) < float.Epsilon)
                        return prevMinion;
                }
                var minion = (from obj in ObjectManager.Get<Obj_AI_Minion>().Where(i => InAutoAttackRange(i) && i.CharData.BaseSkinName != "gangplankbarrel") let hpPred = HealthPrediction.GetHealthPrediction(obj, (int)(Player.AttackDelay * 1000 * 2), 0) where hpPred >= 2 * Player.GetAutoAttackDamage(obj, true) || Math.Abs(hpPred - obj.Health) < float.Epsilon select obj).MaxOrDefault(i => i.Health);
                if (minion != null)
                    prevMinion = minion;
                return minion;
            }
        }

        #region Init

        internal static void Init(Menu mainMenu)
        {
            config = mainMenu;
            var stMenu = new Menu("BrianWalker", "ST");
            var modeMenu = new Menu("Mode", "Mode");
            {
                var mCombo = new Menu("Combo", "ST_Combo");
                mCombo.AddItem(new MenuItem("ST_Combo_Key", "Key").SetValue(new KeyBind(32, KeyBindType.Press)));
                mCombo.AddItem(new MenuItem("ST_Combo_MeleeMagnet", "Melee Movement Magnet").SetValue(true));
                mCombo.AddItem(new MenuItem("ST_Combo_Move", "Movement").SetValue(true));
                mCombo.AddItem(new MenuItem("ST_Combo_Attack", "Attack").SetValue(true));
                modeMenu.AddSubMenu(mCombo);

                var mHarass = new Menu("Harass", "ST_Harass");
                mHarass.AddItem(new MenuItem("ST_Harass_Key", "Key").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                mHarass.AddItem(new MenuItem("ST_Harass_Move", "Movement").SetValue(true));
                mHarass.AddItem(new MenuItem("ST_Harass_Attack", "Attack").SetValue(true));
                mHarass.AddItem(new MenuItem("ST_Harass_LastHit", "Last Hit Minions").SetValue(true));
                modeMenu.AddSubMenu(mHarass);

                var mLaneClear = new Menu("Lane Clear", "ST_Clear");
                mLaneClear.AddItem(new MenuItem("ST_Clear_Key", "Key").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                mLaneClear.AddItem(new MenuItem("ST_Clear_Move", "Movement").SetValue(true));
                mLaneClear.AddItem(new MenuItem("ST_Clear_Attack", "Attack").SetValue(true));
                modeMenu.AddSubMenu(mLaneClear);

                var mLastHit = new Menu("Last Hit", "ST_LastHit");
                mLastHit.AddItem(new MenuItem("ST_LastHit_Key", "Key").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
                mLastHit.AddItem(new MenuItem("ST_LastHit_Move", "Movement").SetValue(true));
                mLastHit.AddItem(new MenuItem("ST_LastHit_Attack", "Attack").SetValue(true));
                modeMenu.AddSubMenu(mLastHit);

                var mFlee = new Menu("Flee", "ST_Flee");
                mFlee.AddItem(new MenuItem("ST_Flee_Key", "Key").SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
                modeMenu.AddSubMenu(mFlee);

                stMenu.AddSubMenu(modeMenu);
            }
            var mMisc = new Menu("Miscallenous", "ST_Misc");
            mMisc.AddItem(new MenuItem("ST_Misc_HoldZone", "Hold Zone").SetValue(new Slider(0, 0, 250)));
            mMisc.AddItem(new MenuItem("ST_Misc_MoveDelay", "Movement Delay").SetValue(new Slider(30, 0, 250)));
            mMisc.AddItem(new MenuItem("ST_Misc_ExtraWindUp", "Extra WindUp Time").SetValue(new Slider(80, 0, 200)));
            mMisc.AddItem(new MenuItem("ST_Misc_PriorityFarm", "Priorize LastHit Over Harass").SetValue(true));
            mMisc.AddItem(new MenuItem("ST_Misc_AllMovementDisabled", "Disable All Movement").SetValue(false));
            mMisc.AddItem(new MenuItem("ST_Misc_AllAttackDisabled", "Disable All Attack").SetValue(false));
            stMenu.AddSubMenu(mMisc);
            /*
            var drawMenu = new Menu("Draw", "Draw");
            drawMenu.AddItem(new MenuItem("ST_Draw_AARange", "Player AA Range").SetValue(new Circle(false, Color.FloralWhite)));
            drawMenu.AddItem(new MenuItem("ST_Draw_AARangeEnemy", "Enemy AA Range").SetValue(new Circle(false, Color.Pink)));
            drawMenu.AddItem(new MenuItem("ST_Draw_HoldZone", "Hold Zone").SetValue(new Circle(false, Color.FloralWhite)));
            STMenu.AddSubMenu(drawMenu);
            */
            config.AddSubMenu(stMenu);
            MovePrediction.SetTargetted(Player.BasicAttack.SpellCastTime, Player.BasicAttack.MissileSpeed);
            Attack = true;
            Move = true;

            GameObject.OnCreate += BrianWalker.GameObject_OnCreate;
            Spellbook.OnStopCast += BrianWalker.Spellbook_OnStopCast;
            Obj_AI_Base.OnProcessSpellCast += BrianWalker.Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += BrianWalker.Game_OnUpdate;

        #endregion

            // TODO: Move that 'scope'.

            /*
            Drawing.OnDraw += args =>
                {
                    if (Player.IsDead)
                    {
                        return;
                    }
                    if (config.Item("ST_Draw_AARange").IsActive())
                    {
                        Render.Circle.DrawCircle(Player.Position, GetAutoAttackRange(), config.Item("ST_Draw_AARange").GetValue<Circle>().Color);
                    }
                    if (config.Item("ST_Draw_AARangeEnemy").IsActive())
                    {
                        foreach (var obj in HeroManager.Enemies.Where(i => i.IsValidTarget(1000)))
                        {
                            Render.Circle.DrawCircle(obj.Position, GetAutoAttackRange(obj, Player), config.Item("ST_Draw_AARangeEnemy").GetValue<Circle>().Color);
                        }
                    }
                    if (config.Item("ST_Draw_HoldZone").IsActive())
                    {
                        Render.Circle.DrawCircle(Player.Position, config.Item("ST_Misc_HoldZone").GetValue<Slider>().Value, config.Item("ST_Draw_HoldZone").GetValue<Circle>().Color);
                    }
                };
                
            */
        }

        internal static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
                return;
            var missile = (MissileClient)sender;
            if (!missile.SpellCaster.IsMe || !missile.SpellCaster.IsRanged || !missile.SData.IsAutoAttack())
                return;
            missileLaunched = true;
            FireAfterAttack((AttackableUnit)missile.Target);
        }

        internal static void Spellbook_OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            if (sender.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
                lastAttack = 0;
        }

        internal static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.Target.IsValid<AttackableUnit>() && args.SData.IsAutoAttack())
            {
                lastAttack = Utils.GameTimeTickCount - Game.Ping / 2;
                missileLaunched = false;
                var target = (AttackableUnit)args.Target;
                if (!lastTarget.IsValidTarget() || target.NetworkId != lastTarget.NetworkId)
                {
                    FireOnTargetSwitch(target);
                    lastTarget = target;
                }
                if (sender.IsMelee) Utility.DelayAction.Add((int)(sender.AttackCastDelay * 1000 + 40), () => FireAfterAttack(target));
                    FireOnAttack(target);
            }
            if (Orbwalking.IsAutoAttackReset(args.SData.Name))
                lastAttack = 0;
        }

        // TODO: Rethink those checks.
        internal static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || CurrentMode == Mode.None || MenuGUI.IsChatOpen || Player.IsRecalling()|| Player.IsCastingInterruptableSpell(true))
            {
                return;
            }
            Orbwalk(CurrentMode == Mode.Flee ? null : GetPossibleTarget);

        }

        #region Orbwalk <=> MoveTo

        // TODO: Fix the stuttering?
        public static void MoveTo(Vector3 pos)
        {
            if (Utils.GameTimeTickCount - lastMove < config.Item("ST_Misc_MoveDelay").GetValue<Slider>().Value)
            return;
            lastMove = Utils.GameTimeTickCount;
            if (Player.Distance(pos, true) < Math.Pow(Player.BoundingRadius + config.Item("ST_Misc_HoldZone").GetValue<Slider>().Value, 2))
                return;
            Player.IssueOrder(GameObjectOrder.MoveTo, Player.ServerPosition.Extend(pos, (Random.NextFloat(0.6f, 1) + 0.2f) * 400));
        }

        public static void Orbwalk(AttackableUnit target)
        {
            if (target.IsValidTarget() && CanAttack && IsAllowedToAttack)
            {
                disableNextAttack = false;
                FireBeforeAttack(target);
                if (!disableNextAttack && (CurrentMode != Mode.Harass || !target.IsValid<Obj_AI_Minion>() || config.Item("OW_Harass_LastHit").IsActive()))
                {
                    lastAttack = Utils.GameTimeTickCount + Game.Ping + 100 - (int)(Player.AttackCastDelay * 1000);
                    missileLaunched = false;
                    if (Player.Distance(target, true) > Math.Pow(GetAutoAttackRange(target) - 65, 2) && !Player.IsMelee)
                    lastAttack = Utils.GameTimeTickCount + Game.Ping + 400 - (int)(Player.AttackCastDelay * 1000);
                    if (!Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                    {
                        Comms.Print("debug", true); // TODO
                    }
                    lastTarget = target;
                    return;
                }
            }
            if (!CanMove || !IsAllowedToMove)
                return;
            if (config.Item("ST_Combo_MeleeMagnet").IsActive() && CurrentMode == Mode.Combo && Player.IsMelee && Player.AttackRange < 200 && InAutoAttackRange(target) && target.IsValid<Obj_AI_Hero>() && ((Obj_AI_Hero) target).Distance(Game.CursorPos) < 300)
            {
                MovePrediction.Delay = Player.BasicAttack.SpellCastTime;
                MovePrediction.Speed = Player.BasicAttack.MissileSpeed;
                MoveTo(MovePrediction.GetPrediction((Obj_AI_Hero) target).UnitPosition);
            }
            else
                MoveTo(Game.CursorPos);
        }

        #endregion

        #region Event - related

        public delegate void AfterAttackEvenH(AttackableUnit target);
        public delegate void BeforeAttackEvenH(BeforeAttackEventArgs args);
        public delegate void OnAttackEvenH(AttackableUnit target);
        public delegate void OnNonKillableMinionH(AttackableUnit minion);
        public delegate void OnTargetChangeH(AttackableUnit oldTarget, AttackableUnit newTarget);
        public static event AfterAttackEvenH AfterAttack;
        public static event BeforeAttackEvenH BeforeAttack;
        public static event OnAttackEvenH OnAttack;
        public static event OnNonKillableMinionH OnNonKillableMinion;
        public static event OnTargetChangeH OnTargetChange;

        private static void FireAfterAttack(AttackableUnit target)
        {
            if (AfterAttack != null && target.IsValidTarget())
            AfterAttack(target);
        }

        private static void FireBeforeAttack(AttackableUnit target)
        {
            if (BeforeAttack != null)
            {
                if (target.IsValidTarget())
                BeforeAttack(new BeforeAttackEventArgs
                                     {
                                         Target = target
                                     });
            }
            else
            disableNextAttack = false;
        }

        private static void FireOnAttack(AttackableUnit target)
        {
            if (OnAttack != null && target.IsValidTarget())
            OnAttack(target);
        }

        private static void FireOnNonKillableMinion(AttackableUnit minion)
        {
            if (OnNonKillableMinion != null && minion.IsValidTarget())
            OnNonKillableMinion(minion);
        }

        private static void FireOnTargetSwitch(AttackableUnit newTarget)
        {
            if (OnTargetChange != null)
            OnTargetChange(lastTarget, newTarget);
        }

        public class BeforeAttackEventArgs
        {
            public AttackableUnit Target;
            private bool process = true;
            public bool Process
            {
                get
                {
                    return process;
                }
                set
                {
                    disableNextAttack = !value;
                    process = value;
                }
            }
        }

        #endregion

    }
}