using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace LastHit_Assistant
{
    class Program
    {
        private static readonly string[] UnsupportedChamps =
        {
            "Azir", "Kalista"
        };

        private static readonly string[] NoAttacks =
        {
            "jarvanivcataclysmattack", "monkeykingdoubleattack",
            "shyvanadoubleattack", "shyvanadoubleattackdragon",
            "zyragraspingplantattack", "zyragraspingplantattack2",
            "zyragraspingplantattackfire", "zyragraspingplantattack2fire",
            "viktorpowertransfer", "sivirwattackbounce",
            "elisespiderlingbasicattack", "heimertyellowbasicattack",
            "heimertyellowbasicattack2", "heimertbluebasicattack",
            "annietibbersbasicattack", "annietibbersbasicattack2",
            "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
            "yorickspectralghoulbasicattack", "malzaharvoidlingbasicattack",
            "malzaharvoidlingbasicattack2", "malzaharvoidlingbasicattack3"
        };

        private static readonly string[] Attacks =
        {
            "caitlynheadshotmissile", "frostarrow", "garenslash2",
            "kennenmegaproc", "lucianpassiveattack", "masteryidoublestrike",
            "quinnwenhanced", "renektonexecute", "renektonsuperexecute",
            "rengarnewpassivebuffdash", "trundleq", "xenzhaothrust",
            "xenzhaothrust2", "xenzhaothrust3", "viktorqbuff"
        };

        private static Menu config;
        private static int lastAaTick;
        private static int lastMovement;

        private static void Main()
        {
            if (Game.Mode == GameMode.Running)
            {
                OnLoad(new EventArgs());
                return;
            }
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
        
        private static void OnLoad(EventArgs args)
        {
            if (UnsupportedChamps.Contains(ObjectManager.Player.ChampionName))
            {
                Game.PrintChat("[ST] LastHit Assistant : " + ObjectManager.Player.ChampionName + " is bugged ATM and has been disabled. I'm sorry :c");
             // SadMemes();
                return;
            }
            config = new Menu("[ST] LastHit Assistant", "ST_LHA", true);

            var menuDrawing = new Menu("Drawings", "LastHit_Draw");
            menuDrawing.AddItem(new MenuItem("LastHit_Draw_AARange", "AA Range").SetValue(new Circle(false, Color.FromArgb(255, 255, 0, 255))));
            menuDrawing.AddItem(new MenuItem("LastHit_Draw_MinionHPBar", "Your AA Damage on Minions' Health Bars").SetValue(new Circle(true, Color.Black)));
            menuDrawing.AddItem(new MenuItem("LastHit_Draw_Lasthit", "Minion Last Hit Circle").SetValue(new Circle(true, Color.Lime)));
            menuDrawing.AddItem(new MenuItem("LastHit_Draw_nearKill", "Minion Near Kill Circle").SetValue(new Circle(true, Color.Gold)));
            config.AddSubMenu(menuDrawing);

            var menuMisc = new Menu("Miscallenous", "LastHit_Misc");
            menuMisc.AddItem(new MenuItem("LastHit_Misc_Holdzone", "Hold Position").SetValue(new Slider(0, 100, 0)));
            menuMisc.AddItem(new MenuItem("LastHit_Misc_Farmdelay", "Farm Delay").SetValue(new Slider(0, 100, 0)));
            menuMisc.AddItem(new MenuItem("LastHit_Misc_ExtraWindUp", "Extra Winduptime").SetValue(new Slider(35, 200, 0)));
            menuMisc.AddItem(new MenuItem("LastHit_Misc_AutoWindUp", "Autoset Windup").SetValue(false));
            menuMisc.AddItem(new MenuItem("LastHit_Misc_Humanizer", "Humanizer/Movement Delay").SetValue(new Slider(50, 150, 0)));
            menuMisc.AddItem(new MenuItem("LastHit_Misc_Path", "Send Short Move Commands").SetValue(true));
            config.AddItem(new MenuItem("LastHit_Move", "Movement").SetValue(true));
            config.AddSubMenu(menuMisc);

            config.AddItem(new MenuItem("LastHit_Key", "Active").SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            config.AddToMainMenu();
        }

        private static void OnUpdate(EventArgs args)
        {
            CheckAutoWindUp();
            if (!config.Item("LastHit_Key").GetValue<KeyBind>().Active || ObjectManager.Player.IsCastingInterruptableSpell(true) || MenuGUI.IsChatOpen || ObjectManager.Player.IsDead)
            {
                return;
            }
            var target = GetPossibleTarget();
            Orbwalk(Game.CursorPos, target);
        }

        private static void OnDraw(EventArgs args)
        {
            if (config.Item("LastHit_Draw_AARange").GetValue<Circle>().Active && !ObjectManager.Player.IsDead)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, GetAutoAttackRange(), config.Item("LastHit_Draw_AARange").GetValue<Circle>().Color);
            }
            if (!config.Item("LastHit_Draw_MinionHPBar").GetValue<Circle>().Active && !config.Item("LastHit_Draw_Lasthit").GetValue<Circle>().Active && !config.Item("LastHit_Draw_nearKill").GetValue<Circle>().Active)
            {
                return;
            }
            foreach (var minion in MinionManager.GetMinions(ObjectManager.Player.Position, 2500))
            {
                if (!minion.IsValidTarget(2500))
                {
                    continue;
                }
                var attackToKill = Math.Ceiling(minion.MaxHealth / ObjectManager.Player.GetAutoAttackDamage(minion, true));
                var hpBarPosition = minion.HPBarPosition;
                var barWidth = minion.IsMelee ? 75 : 80;
                if (minion.HasBuff("turretshield"))
                {
                    barWidth = 70;
                }
                var barDistance = (float)(barWidth / attackToKill);
                if (config.Item("LastHit_Draw_Lasthit").GetValue<Circle>().Active && minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion, true))
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius, config.Item("LastHit_Draw_Lasthit").GetValue<Circle>().Color);
                }
                else if (config.Item("LastHit_Draw_nearKill").GetValue<Circle>().Active && minion.Health <= ObjectManager.Player.GetAutoAttackDamage(minion, true) * 2)
                {
                    Render.Circle.DrawCircle(minion.Position, minion.BoundingRadius, config.Item("LastHit_Draw_nearKill").GetValue<Circle>().Color);
                }
                if (!config.Item("LastHit_Draw_MinionHPBar").GetValue<Circle>().Active)
                {
                    continue;
                }
                for (var i = 1; i < attackToKill; i++)
                {
                    var startposition = hpBarPosition.X + 45 + barDistance * i;
                    Drawing.DrawLine(new Vector2(startposition, hpBarPosition.Y + 18), new Vector2(startposition, hpBarPosition.Y + 23), 1, config.Item("LastHit_Draw_MinionHPBar").GetValue<Circle>().Color);
                }
            }
        }

        private static void Orbwalk(Vector3 goalPosition, GameObject target)
        {
            if (target != null && CanAttack() && config.Item("LastHit_Key").GetValue<KeyBind>().Active)
            {
                if (ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target))
                {
                    lastAaTick = Environment.TickCount + Game.Ping / 2;
                }
            }
            if (!CanMove() || !IsAllowedToMove())
            {
                return;
            }
            MoveTo(goalPosition);

        }

        private static void MoveTo(Vector3 position, float holdAreaRadius = -1)
        {
            var delay = config.Item("LastHit_Misc_Humanizer").GetValue<Slider>().Value;
            if (Environment.TickCount - lastMovement < delay || !CanMove())
            {
                return;
            }
            lastMovement = Environment.TickCount;
            if (holdAreaRadius < 0)
            {
                holdAreaRadius = config.Item("LastHit_Misc_Holdzone").GetValue<Slider>().Value;
            }
            if (ObjectManager.Player.ServerPosition.Distance(position) < holdAreaRadius)
            {
                if (ObjectManager.Player.Path.Length > 1)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.HoldPosition, ObjectManager.Player.ServerPosition);
                }
                return;
            }
            if (config.Item("LastHit_Misc_Path").GetValue<bool>())
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, ObjectManager.Player.ServerPosition + 200 * (position.To2D() - ObjectManager.Player.ServerPosition.To2D()).Normalized().To3D());
            }
            else
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, position);
            }
        }

        private static bool IsAllowedToMove()
        {
            return config.Item("LastHit_Key").GetValue<KeyBind>().Active && config.Item("LastHit_Move").GetValue<bool>();
        }

        private static void OnProcessSpell(Obj_AI_Base unit, GameObjectProcessSpellCastEventArgs spell)
        {
            if (!IsAutoAttack(spell.SData.Name) || !unit.IsMe)
            {
                return;
            }
            lastAaTick = Environment.TickCount;
        }

        private static Obj_AI_Base GetPossibleTarget()
        {
            return (from minion in ObjectManager.Get<Obj_AI_Minion>() where minion.IsValidTarget() && InAutoAttackRange(minion) let t = (int)(ObjectManager.Player.AttackCastDelay * 1000) - 100 + Game.Ping / 2 + 1000 * (int)ObjectManager.Player.Distance(minion) / (int)MyProjectileSpeed() let predHealth = HealthPrediction.GetHealthPrediction(minion, t, config.Item("LastHit_Misc_Farmdelay").GetValue<Slider>().Value) where minion.Team != GameObjectTeam.Neutral && predHealth > 0 && predHealth <= ObjectManager.Player.GetAutoAttackDamage(minion, true) select minion).FirstOrDefault();
        }

        private static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower())) || Attacks.Contains(name.ToLower());
        }

        private static bool CanAttack()
        {
            if (lastAaTick <= Environment.TickCount)
            {
                return Environment.TickCount + Game.Ping / 2 + 25 >= lastAaTick + ObjectManager.Player.AttackDelay * 1000;
            }
            return false;
        }

        private static bool CanMove()
        {
            var extraWindup = config.Item("LastHit_Misc_ExtraWindUp").GetValue<Slider>().Value;
            if (lastAaTick <= Environment.TickCount)
            {
                return Environment.TickCount + Game.Ping / 2 >= lastAaTick + ObjectManager.Player.AttackCastDelay * 1000 + extraWindup;
            }
            return false;
        }

        private static float MyProjectileSpeed()
        {
            return (ObjectManager.Player.CombatType == GameObjectCombatType.Melee) ? float.MaxValue : ObjectManager.Player.BasicAttack.MissileSpeed;
        }

        private static void CheckAutoWindUp()
        {
            if (!config.Item("LastHit_Misc_AutoWindUp").GetValue<bool>())
            {
                return;
            }
            var additional = 0;
            if (Game.Ping >= 100)
            {
                additional = Game.Ping / 100 * 10;
            }
            else if (Game.Ping > 40 && Game.Ping < 100)
            {
                additional = Game.Ping / 100 * 20;
            }
            else if (Game.Ping <= 40) additional = +20;
            var windUp = Game.Ping + additional;
            if (windUp < 40)
            {
                windUp = 40;
            }
            config.Item("LastHit_Misc_ExtraWindUp").SetValue(windUp < 200 ? new Slider(windUp, 200, 0) : new Slider(200, 200, 0));
        }

        private static float GetAutoAttackRange(Obj_AI_Base source = null, GameObject target = null)
        {
            if (source == null)
            {
                source = ObjectManager.Player;
            }
            var ret = source.AttackRange + ObjectManager.Player.BoundingRadius;
            if (target != null)
            {
                ret += target.BoundingRadius;
            }
            return ret;
        }

        private static bool InAutoAttackRange(Obj_AI_Base target)
        {
            if (target == null)
            {
                return false;
            }
            var myRange = GetAutoAttackRange(ObjectManager.Player, target);
            return Vector2.DistanceSquared(target.ServerPosition.To2D(), ObjectManager.Player.ServerPosition.To2D()) <= myRange * myRange;
        }
    }
}