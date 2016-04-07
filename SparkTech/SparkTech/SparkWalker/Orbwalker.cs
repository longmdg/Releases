namespace SparkTech.SparkWalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers.Damages;

    using SharpDX;

    using SparkTech.Cache;
    using SparkTech.Enumerations;
    using SparkTech.EventData;
    using SparkTech.Utils;

    using Key = System.Windows.Forms.Keys;
    using Menu = LeagueSharp.SDK.Core.UI.IMenu.Menu;
    using Drawings = LeagueSharp.Drawing;
    using Color = System.Drawing.Color;
    using SharpColor = SharpDX.Color;

    /// <summary>
    /// An alternative to the <see cref="LeagueSharp.SDK.Orbwalker"/> class.
    /// </summary>
    public static class Orbwalker
    {
        #region Attackable Objects

        /// <summary>
        /// The <see cref="ObjectInfo"/> nested class. 
        /// <para>Warning: It's messy but it actually works</para>
        /// </summary>
        private class ObjectInfo
        {
            /// <summary>
            /// The display name of the object
            /// </summary>
            internal readonly string DisplayName;

            /// <summary>
            /// The <see cref="Func{TResult}"/> whether to attack
            /// </summary>
            internal readonly Func<Mode, bool> Attack;

            /// <summary>
            /// The indication whether the item should be added to menu
            /// </summary>
            internal readonly bool AddToMenu;

            /// <summary>
            /// The indication whether to enable the menu item by default
            /// </summary>
            internal readonly bool AttackByDefault;

            /// <summary>
            /// Returns a specified <see cref="ObjectInfo"/> instance
            /// </summary>
            /// <param name="displayName">The display name</param>
            /// <param name="attackByDef">Attack by default</param>
            /// <returns></returns>
            internal static ObjectInfo Create(string displayName, bool attackByDef = true)
            {
                return new ObjectInfo(
                    true,
                    attackByDef,
                    mode => Menu[$"st_orb_modes_{mode.ToShort()}_targeting_objects_{displayName.ToMenuUse()}"].GetValue<MenuBool>().Value,
                    displayName);
            }

            /// <summary>
            /// The invulnerable <see cref="ObjectInfo"/> instance
            /// </summary>
            internal static readonly ObjectInfo Invulnerable = new ObjectInfo(false, false, mode => false, "Invulnerable");

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectInfo"/> nested class
            /// </summary>
            /// <param name="addToMenu">Add to menu</param>
            /// <param name="attackByDef">Attack by default</param>
            /// <param name="attack">Attack</param>
            /// <param name="displayName">Display name</param>
            private ObjectInfo(bool addToMenu, bool attackByDef, Func<Mode, bool> attack, string displayName)
            {
                AddToMenu = addToMenu;

                AttackByDefault = attackByDef;

                Attack = attack;

                DisplayName = displayName;
            }
        }

        /// <summary>
        /// The <see cref="Dictionary{TKey,TValue}"/> holding <see cref="ObjectInfo"/> data
        /// </summary>
        private static readonly Dictionary<string, ObjectInfo> AttackDictionary = new Dictionary<string, ObjectInfo>
                                                                                      {
                                                                                          {
                                                                                              "zyrathornplant",
                                                                                              ObjectInfo.Create("Zyra's thorn plant")
                                                                                          },
                                                                                          {
                                                                                              "zyragraspingplant",
                                                                                              ObjectInfo.Create("Zyra's grasping plant")
                                                                                          },
                                                                                          {
                                                                                              "shacobox",
                                                                                              ObjectInfo.Create("Shaco's box")
                                                                                          },
                                                                                          {
                                                                                              "yorickdecayedghoul",
                                                                                              ObjectInfo.Create("Yorick's decayed ghoul")
                                                                                          },
                                                                                          {
                                                                                              "yorickravenousghoul",
                                                                                              ObjectInfo.Create("Yorick's ravenous ghoul")
                                                                                          },
                                                                                          {
                                                                                              "yorickspectralghoul",
                                                                                              ObjectInfo.Create("Yorick's spectral ghoul")
                                                                                          },
                                                                                          {
                                                                                              "heimertyellow",
                                                                                              ObjectInfo.Create("Heimer's yellow turret")
                                                                                          },
                                                                                          {
                                                                                              "heimertblue",
                                                                                              ObjectInfo.Create("Heimer's blue turret")
                                                                                          },
                                                                                          {
                                                                                              "malzaharvoidling",
                                                                                              ObjectInfo.Create("Malzahar's voidling")
                                                                                          },
                                                                                          {
                                                                                              "annietibbers",
                                                                                              ObjectInfo.Create("Annie's tibbers", false)
                                                                                          },
                                                                                          {
                                                                                              "teemomushroom",
                                                                                              ObjectInfo.Create("Teemo's mushroom")
                                                                                          },
                                                                                          {
                                                                                              "elisespiderling",
                                                                                              ObjectInfo.Create("Elise's spiderling")
                                                                                          },
                                                                                          {
                                                                                              "gangplankbarrel",
                                                                                              ObjectInfo.Create("Gangplank's barrel", false)
                                                                                          },
                                                                                          {
                                                                                              "leblanc",
                                                                                              ObjectInfo.Create("LeBlanc's clone", false)
                                                                                          },
                                                                                          {
                                                                                              "shaco",
                                                                                              ObjectInfo.Create("Shaco's clone", false)
                                                                                          },
                                                                                          {
                                                                                              "monkeyking",
                                                                                              ObjectInfo.Create("Wukong's clone", false)
                                                                                          },
                                                                                          {
                                                                                              "jarvanivstandard",
                                                                                              ObjectInfo.Invulnerable
                                                                                          }
                                                                                      };

        #endregion

        #region Configuration

        /// <summary>
        /// Contains the mode's configuration data
        /// </summary>
        private class ModeConfig
        {
            /// <summary>
            /// The key for mode to be activated
            /// </summary>
            internal readonly Key Key;

            /// <summary>
            /// An array of <see cref="UnitType"/> the mode should be enabled by default for
            /// </summary>
            internal readonly UnitType[] UnitsEnabled;

            /// <summary>
            /// Initializes a new instance of the <see cref="ModeConfig"/> class
            /// </summary>
            /// <param name="key">The key for mode to be activated</param>
            /// <param name="unitsEnabled">An array of <see cref="UnitType"/> the mode should be enabled by default for</param>
            internal ModeConfig(Key key, params UnitType[] unitsEnabled)
            {
                Key = key;

                UnitsEnabled = new UnitType[EnumCache<UnitType>.Count];

                for (int i = 0; i < EnumCache<UnitType>.Count; i++)
                {
                    UnitsEnabled[i] = i < unitsEnabled.Length ? unitsEnabled[i] : UnitType.None;
                }
            }
        }

        /// <summary>
        /// Contains the configuration for each of the modes
        /// </summary>
        private static readonly Dictionary<Mode, ModeConfig> ModeConfiguration = new Dictionary<Mode, ModeConfig>
                                                                                     {
                                                                                         {
                                                                                             Mode.Combo,
                                                                                             new ModeConfig(
                                                                                             Key.Space,
                                                                                             UnitType.Champion)
                                                                                         },
                                                                                         {
                                                                                             Mode.LaneClear,
                                                                                             new ModeConfig(
                                                                                             Key.V,
                                                                                             UnitType.LaneMinion,
                                                                                             UnitType.Structure,
                                                                                             UnitType.Object,
                                                                                             UnitType.Champion,
                                                                                             UnitType.LaneClearMinion,
                                                                                             UnitType.JungleMinion)
                                                                                         },
                                                                                         {
                                                                                             Mode.Harass,
                                                                                             new ModeConfig(
                                                                                             Key.C,
                                                                                             UnitType.LaneMinion,
                                                                                             UnitType.Champion)
                                                                                         },
                                                                                         {
                                                                                             Mode.Freeze,
                                                                                             new ModeConfig(
                                                                                             Key.A,
                                                                                             UnitType.LaneMinion)
                                                                                         },
                                                                                         {
                                                                                             Mode.LastHit,
                                                                                             new ModeConfig(
                                                                                             Key.X,
                                                                                             UnitType.LaneMinion)
                                                                                         },
                                                                                         {
                                                                                             Mode.Flee,
                                                                                             new ModeConfig(Key.Z)
                                                                                         }
                                                                                     };

        #endregion

        #region Variables

        /// <summary>
        /// Gets the <see cref="Obj_AI_Hero"/> instance of the <see cref="E:Player"/>
        /// </summary>
        private static Obj_AI_Hero Player => ObjectCache.Player;

        /// <summary>
        /// The orbwalker's <see cref="LeagueSharp.SDK.Core.UI.IMenu.Menu"/> instance
        /// </summary>
        private static readonly Menu Menu = Core.Menu.Add(new Menu("st_orb", "SparkWalker [ALPHA]"));

        /// <summary>
        /// The <see cref="System.Random"/> instance
        /// </summary>
        private static readonly Random Random = new Random(Variables.TickCount);

        /// <summary>
        /// Determines whether the <see cref="E:Player"/> can move
        /// </summary>
        private static bool movementFlag;

        /// <summary>
        /// The current ping
        /// </summary>
        private static int Ping { get; set; }

        /// <summary>
        /// The current position of the <see cref="E:Player"/>
        /// </summary>
        private static Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the orbwalking point
        /// </summary>
        public static Vector3 OrbwalkingPoint { get; set; }

        /// <summary>
        /// Gets the extra holdzone radius
        /// </summary>
        private static float ExtraHoldZone => Menu["st_orb_problems_stutter"].GetValue<MenuBool>().Value ? 100f : 20f;

        /// <summary>
        /// The backing field for <see cref="E:ScanRange"/>
        /// </summary>
        private static Func<Obj_AI_Minion, float> scanRangeBacking;

        /// <summary>
        /// Gets or sets the <see cref="E:UnkillableMinions"/> scan range
        /// </summary>
        public static Func<Obj_AI_Minion, float> ScanRange
        {
            get
            {
                return scanRangeBacking ?? AutoAttack.GetRealAutoAttackRange;
            }
            set
            {
                scanRangeBacking = value;
            }
        }

        /// <summary>
        /// Gets or sets the submenu name
        /// </summary>
        public static string MenuName
        {
            get
            {
                return Menu.DisplayName;
            }
            set
            {
                Menu.DisplayName = value;
            }
        }

        /// <summary>
        /// The last target attacked by the <see cref="E:Player"/>, used with the <see cref="E:PlayerTargetSwitch"/> event
        /// </summary>
        private static AttackableUnit switchTarget;

        /// <summary>
        /// Gets the current tick count
        /// </summary>
        private static int TickCount { get; set; }

        /// <summary>
        /// Gets the current farm delay
        /// </summary>
        private static int FarmDelay => Menu["st_orb_problems_missingcs"].GetValue<MenuBool>().Value ? Core.ChampionName == "Azir" ? 125 : 70 : 0;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the player autoattacks
        /// </summary>
        public static event EventDataHandler<PlayerAttackEventArgs> PlayerAttack;

        /// <summary>
        /// Fired before the orbwalker - generated attack, useful for cancelling it
        /// </summary>
        public static event EventDataHandler<BeforePlayerUnhumanAttackEventArgs> BeforePlayerUnhumanAttack;

        /// <summary>
        /// Fired after the windup is done
        /// </summary>
        public static event EventDataHandler<AfterPlayerAttackEventArgs> AfterPlayerAttack;

        /// <summary>
        /// Fired when the Player switches targets
        /// </summary>
        public static event EventDataHandler<PlayerTargetSwitchEventArgs> PlayerTargetSwitch;

        /// <summary>
        /// Fired when you aren't able to execute minions using just basic attacks
        /// </summary>
        public static event EventDataHandler<UnkillableMinionsEventArgs> UnkillableMinions;

        #endregion

        #region Enable Handlers

        /// <summary>
        /// Gets or sets the drawings
        /// </summary>
        public static bool Drawing { get; set; } = true;

        /// <summary>
        /// Gets or sets attacks
        /// </summary>
        public static bool Attacks { get; set; } = true;

        /// <summary>
        /// Gets or sets the movement
        /// </summary>
        public static bool Movement { get; set; } = true;

        /// <summary>
        /// Determines whether the <see cref="Orbwalker"/> is enabled
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return Attacks || Movement;
            }
            set
            {
                Attacks = value;
                Movement = value;
            }
        }

        #endregion

        #region Timers

        /// <summary>
        /// An array of buff names getting which resets the auto attack timer
        /// </summary>
        private static readonly List<string> AttackResettingBuffs = new List<string> { "poppypassivebuff", "sonapassiveready" };

        /// <summary>
        /// The backing field for <see cref="E:LastAttackTick"/>
        /// </summary>
        private static int lastAttackTick;

        /// <summary>
        /// Gets the previous attack tick
        /// </summary>
        private static int previousAttackTick;

        /// <summary>
        /// Gets the last autoattack tick
        /// </summary>
        public static int LastAttackTick
        {
            get
            {
                return lastAttackTick;
            }
            private set
            {
                lastAttackTick = value;

                previousAttackTick = previousAttackTick != value
                                         ? value
                                         : value - (int)(Player.AttackDelay * 1000f - 100f + Ping / 2f);
            }
        }

        /// <summary>
        /// Gets the last movement tick
        /// </summary>
        public static int LastMovementTick { get; private set; }

        /// <summary>
        /// Determines tha the orders should be blockes until a determined tick
        /// </summary>
        private static int BlockOrders { get; set; }

        /// <summary>
        /// Resets the specified timers 
        /// </summary>
        public static void ResetTimer(bool resetMoveFlag = false)
        {
            if (resetMoveFlag)
            {
                movementFlag = true;
            }

            LastAttackTick = previousAttackTick;
        }

        #endregion

        #region Permissions

        /// <summary>
        /// Indicates whether attacks are allowed for the specified <see cref="Mode"/>
        /// </summary>
        /// <param name="mode">The orbwalking mode</param>
        /// <returns></returns>
        private static bool HasAttackPermissions(Mode mode)
        {
            return Attacks && mode != Mode.None && Menu["st_orb_attacks"].GetValue<MenuBool>().Value
                   && Menu[$"st_orb_modes_{mode.ToShort()}_attack"].GetValue<MenuBool>().Value;
        }

        /// <summary>
        /// Indicates whether movement is allowed for the specified <see cref="Mode"/>
        /// </summary>
        /// <param name="mode">The orbwalking mode</param>
        /// <returns></returns>
        private static bool HasMovementPermissions(Mode mode)
        {
            return Movement && mode != Mode.None && Menu["st_orb_movement"].GetValue<MenuBool>().Value
                   && Menu[$"st_orb_modes_{mode.ToShort()}_movement"].GetValue<MenuBool>().Value
                   && !Menu["st_orb_keybind_blockmov"].GetValue<MenuKeyBind>().Active;
        }

        #endregion

        #region Mode Handlers

        /// <summary>
        /// Gets or sets the default custom mode
        /// </summary>
        public static Mode DefaultCustomMode;

        /// <summary>
        /// Saves the value of a custom mode
        /// </summary>
        private static Mode? customMode;

        /// <summary>
        /// Returns the current <see cref="Enumerations.Mode"/> and as well as allows you to override the settings
        /// </summary>
        public static Mode Mode
        {
            get
            {
                return customMode ?? EnumCache<Mode>.Values.Find(mode => Menu["st_orb_modes"][$"st_orb_modes_{mode.ToShort()}"][$"st_orb_modes_{mode.ToShort()}_key"].GetValue<MenuKeyBind>().Active);
            }
            set
            {
                customMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the default mode
        /// </summary>
        public static bool CustomMode
        {
            get
            {
                return customMode.HasValue;
            }
            set
            {
                customMode = value ? (Mode?)DefaultCustomMode : null;
            }
        }

        #endregion

        #region Structure

        /// <summary>
        /// Initializes the core features of the <see cref="Orbwalker"/>
        /// </summary>
        static Orbwalker()
        {
            var modesMenu = Menu.Add(new Menu("st_orb_modes", "Modes"));
            {
                foreach (var config in ModeConfiguration)
                {
                    var header = $"st_orb_modes_{config.Key.ToShort()}";

                    var modeMenu = modesMenu.Add(new Menu(header, config.Key.ToString()));
                    {
                        header += "_targeting";

                        var targetingMenu = modeMenu.Add(new Menu(header, "Targeting"));
                        {
                            var champMenu = targetingMenu.Add(new Menu(header + "_champion", "Champions"));
                            {
                                champMenu.Add(new MenuBool(header + "_champion_ignoreshields", "Ignore shields"));
                                champMenu.Add(new MenuBool(header + "_champion_ignorets", "Ignore TS on targets easy to kill", true));
                                champMenu.Add(new MenuSlider(header + "_champion_attacks", "^ Max killable attacks for this to trigger", 2, 1, 5));

                                var blacklistMenu = champMenu.Add(new Menu(header + "_champion_blacklist", "Blacklist"));
                                {
                                    blacklistMenu.Add(new MenuSeparator(header + "_champion_blacklist_info", "Disable attacking for the following champions:"));

                                    foreach (var hero in ObjectCache.GetNative<Obj_AI_Hero>().FindAll(hero => hero.IsEnemy))
                                    {
                                        blacklistMenu.Add(new MenuBool(header + "_champion_blacklist" + hero.NetworkId, $"{hero.ChampionName()} ({hero.Name})"));
                                    }
                                }
                            }

                            var objectMenu = targetingMenu.Add(new Menu(header + "_targeting_objects", "Objects"));
                            {
                                objectMenu.Add(new MenuSeparator(header + "_targeting_objects_info", "Attack the following objects:"));
                                objectMenu.AddSeparator();
                                objectMenu.Add(new MenuSeparator(header + "_targeting_objects_wards", "Wards"));

                                foreach (var info in AttackDictionary.Select(pair => pair.Value).Where(info => info.AddToMenu))
                                {
                                    objectMenu.Add(new MenuBool($"{header}_targeting_objects_{info.DisplayName.ToMenuUse()}", info.DisplayName, info.AttackByDefault));
                                }
                            }

                            var structureMenu = targetingMenu.Add(new Menu(header + "_structure", "Structures"));
                            {
                                structureMenu.Add(new MenuBool($"{header}_structures_nexus", "Attack the nexus", true));
                                structureMenu.Add(new MenuBool($"{header}_structures_inhibitor", "Attack inhibitors", true));
                                structureMenu.Add(new MenuBool($"{header}_structures_turret", "Attack turrets", true));
                            }

                            var jungleMenu = targetingMenu.Add(new Menu(header + "_jungle", "Jungle"));
                            {
                                jungleMenu.Add(new MenuBool(header + "_jungle_smallfirst", "Prioritize small minions"));
                            }

                            if (config.Key == Mode.Freeze)
                            {
                                var freezeMenu = targetingMenu.Add(new Menu(header + "_freeze", "Freeze"));
                                {
                                    freezeMenu.Add(new MenuSlider(header + "_freeze_maxhealth", "Health to freeze minions at", 20, 5, 50));
                                }
                            }

                            targetingMenu.AddSeparator();

                            for (int i = 0; i < EnumCache<UnitType>.Count; ++i)
                            {
                                targetingMenu.Add(new MenuList<UnitType>($"{header}_priority_{i}", i != 0 ? i != EnumCache<UnitType>.Count - 1 ? $"Priority {i}" : $"Priority {i} (last)" : $"Priority {i} (first)", EnumCache<UnitType>.Values) { SelectedValue = config.Value.UnitsEnabled[i] });
                            }
                        }

                        header = header.Replace("_targeting", "");

                        modeMenu.Add(new MenuBool(header + "_magnet", "Magnet to champion targets (melee only)", config.Key == Mode.Combo));
                        modeMenu.Add(new MenuBool(header + "_attacks", "Enable attacks", true));
                        modeMenu.Add(new MenuBool(header + "_movement", "Enable movement", true));
                        modeMenu.Add(new MenuKeyBind(header + "_key", $"{config.Key} active!", config.Value.Key, KeyBindType.Press));
                    }
                }

                modesMenu.Add(new MenuKeyBind("st_orb_key_movblock", "Movement block", Key.P, KeyBindType.Press));
            }

            var drawMenu = Menu.Add(new Menu("st_orb_draw", "Drawings"));
            {
                var rangeMenu = drawMenu.Add(new Menu("st_orb_draw_ranges", "Attack ranges"));
                {
                    var adc = rangeMenu.Add(new MenuBool("st_orb_draw_ranges_adc", "Turn on by default for enemy ADC", true));

                    Action<Obj_AI_Hero> addToMenu = hero =>
                    {
                        var id = hero.NetworkId;

                        var heroMenu = rangeMenu.Add(new Menu($"st_orb_draw_ranges_{id}", $"{hero.ChampionName()} ({hero.Name})"));
                        {
                            heroMenu.Add(new MenuSlider($"st_orb_draw_ranges_radius_{id}", "Radius to activate (0 stands for unlimited)", 1500, 0, 5000));
                            heroMenu.Add(new MenuColor($"st_orb_draw_ranges_range_{id}", "Draw range", hero.IsEnemy ? SharpColor.Red : SharpColor.Blue) { Active = adc.Value && hero.IsADC() });
                            heroMenu.Add(new MenuColor($"st_orb_draw_ranges_holdzone_{id}", "Draw HoldZone", SharpColor.White) { Active = false });
                        }
                    };

                    var heroes = ObjectCache.GetNative<Obj_AI_Hero>();

                    rangeMenu.AddSeparator();
                    rangeMenu.AddSeparator("== ALLIES ==");
                    rangeMenu.AddSeparator();

                    heroes.FindAll(hero => hero.IsAlly).ForEach(addToMenu);

                    rangeMenu.AddSeparator();
                    rangeMenu.AddSeparator("== ENEMIES ==");
                    rangeMenu.AddSeparator();

                    heroes.FindAll(hero => hero.IsEnemy).ForEach(addToMenu);
                }

                var minionMenu = drawMenu.Add(new Menu("st_orb_draw_minions", "Minions"));
                {
                    minionMenu.Add(new MenuColor("st_orb_draw_minions_killable", "Draw killable minions", SharpColor.AliceBlue));
                }
            }

            var problemMenu = Menu.Add(new Menu("st_orb_problems", "Problems"));
            {
                problemMenu.Add(new MenuBool("st_orb_problems_stutter", "I'm stuttering"));
                problemMenu.Add(new MenuBool("st_orb_problems_missingcs", "The lasthits are badly timed"));
                problemMenu.Add(new MenuBool("st_orb_problems_holdzone", "The HoldZone isn't big enough"));
            }

            Menu.Add(new MenuBool("st_orb_attacks", "Enable any attacks", true));
            Menu.Add(new MenuBool("st_orb_movement", "Enable any movement", true));
            Menu.Add(new MenuList<HumanizerMode>("st_orb_humanizer", "Humanizer mode", EnumCache<HumanizerMode>.Values) { SelectedValue = HumanizerMode.Normal });

            Game.OnUpdate += delegate
                {
                    TickCount = Variables.TickCount;
                    Ping = Game.Ping;
                    Position = Player.ServerPosition.ToVector2();

                    if (BlockOrders >= TickCount)
                    {
                        return;
                    }

                    if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen || Player.IsDead || Player.IsCastingInterruptableSpell(true))
                    {
                        return;
                    }

                    var position = Vector3.Zero;
                    var mode = Mode;
                    var target = GetTarget(mode);

                    if (target != null && InAttackRange(target))
                    {
                        if (HasAttackPermissions(mode) && CanAttack())
                        {
                            var args = new BeforePlayerUnhumanAttackEventArgs(target);

                            try
                            {
                                BeforePlayerUnhumanAttack?.Invoke(args);
                            }
                            catch (Exception ex)
                            {
                                ex.Catch();
                            }

                            if (!args.CancelAttack && Player.IssueOrder(GameObjectOrder.AutoAttack, target))
                            {
                                if (Player.CanCancelAutoAttack())
                                {
                                    movementFlag = false;
                                }
                            }
                        }
                        else if (Menu[$"st_orb_modes_{mode.ToShort()}_magnet"] && Player.IsMelee)
                        {
                            var hero = target as Obj_AI_Hero;

                            if (hero.InAutoAttackRange())
                            {
                                position = LeagueSharp.SDK.Movement.GetPrediction(hero, Player.BasicAttack.SpellCastTime, 0f, Player.BasicAttack.MissileSpeed).UnitPosition;
                            }
                        }
                    }

                    if (!HasMovementPermissions(mode) || !CanMove())
                    {
                        return;
                    }

                    if (position.IsZero)
                    {
                        position = Point();
                    }

                    if (Player.Path.Length > 0)
                    {
                        if (Position.Distance(position) < Player.BoundingRadius + ExtraHoldZone && Player.IssueOrder(GameObjectOrder.Stop, Player.ServerPosition))
                        {
                            TickUp();
                            LastMovementTick = TickCount;
                        }
                    }
                    else if (Player.IssueOrder(GameObjectOrder.MoveTo, Player.ServerPosition + (position.ToVector2() - Position).Normalized().ToVector3() * 200f))
                    {
                        TickUp();
                        LastMovementTick = TickCount;
                    }

                    if (UnkillableMinions == null)
                    {
                        return;
                    }

                    var minions =
                        ObjectCache.GetMinions(
                            ObjectTeam.Enemy,
                            MinionType.Minion,
                            minion =>
                            Vector2.DistanceSquared(Position, minion.ServerPosition.ToVector2())
                            <= (float)Math.Pow(ScanRange(minion), 2))
                            .FindAll(minion => Health.GetPrediction(minion, ProjectileTime(minion), FarmDelay) <= 0);

                    if (minions.Count > 0)
                    {
                        try
                        {
                            UnkillableMinions(new UnkillableMinionsEventArgs(minions));
                        }
                        catch (Exception ex)
                        {
                            ex.Catch();
                        }
                    }
                };

            Obj_AI_Base.OnDoCast += (sender, args) =>
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }

                    var name = args.SData.Name;

                    if (AutoAttack.IsAutoAttack(name))
                    {
                        try
                        {
                            AfterPlayerAttack?.Invoke(new AfterPlayerAttackEventArgs((AttackableUnit)args.Target));
                        }
                        catch (Exception ex)
                        {
                            ex.Catch();
                        }
                    }
                    else if (AutoAttack.IsAutoAttackReset(name))
                    {
                        ResetTimer();
                    }
                };

            Obj_AI_Base.OnPlayAnimation += (sender, args) =>
            {
                if (!sender.IsMe)
                {
                    return;
                }

                var animation = args.Animation.ToLower();

                if (animation.StartsWith("attack") || animation == "crit")
                {
                    DelayAction.Add(Player.AttackCastDelay * 1000f - 100f + Ping / 2f, () => movementFlag = true);
                }
            };

            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
                {
                    if (!sender.IsMe || !AutoAttack.IsAutoAttack(args.SData.Name))
                    {
                        return;
                    }

                    var target = (AttackableUnit)args.Target;

                    LastAttackTick = TickCount - Ping / 2;
                    TickUp();

                    try
                    {
                        PlayerAttack?.Invoke(new PlayerAttackEventArgs(target));
                    }
                    catch (Exception ex)
                    {
                        ex.Catch();
                    }

                    if (PlayerTargetSwitch != null && (!switchTarget.Compare(target) || !switchTarget.IsValidTarget() && target.IsValidTarget()))
                    {
                        try
                        {
                            PlayerTargetSwitch(new PlayerTargetSwitchEventArgs(switchTarget, target));
                        }
                        catch (Exception ex)
                        {
                            ex.Catch();
                        }
                    }

                    switchTarget = target;
                };

            Spellbook.OnStopCast += (spellbook, args) =>
            {
                if (spellbook.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
                {
                    ResetTimer(true);
                }
            };

            Obj_AI_Base.OnBuffAdd += (sender, args) =>
            {
                if (sender.IsMe && AttackResettingBuffs.Contains(args.Buff.DisplayName.ToLower()))
                {
                    ResetTimer();
                }
            };
            
            Drawings.OnDraw += delegate
            {
                return;

                foreach (var hero in ObjectCache.Get<Obj_AI_Hero>())
                {
                    var id = hero.NetworkId;

                    var radius = Menu[$"st_orb_draw_ranges_radius_{id}"].GetValue<MenuSlider>().Value;
                    var range = Menu[$"st_orb_draw_ranges_range_{id}"].GetValue<MenuColor>();
                    var holdZone = Menu[$"st_orb_draw_ranges_holdzone_{id}"].GetValue<MenuColor>();

                    if (!holdZone.Active && !range.Active)
                    {
                        continue;
                    }

                    if (!hero.IsValidTarget(radius == 0 ? float.MaxValue : radius, false))
                    {
                        continue;
                    }

                    if (range.Active)
                    {
                        Drawings.DrawCircle(hero.ServerPosition, hero.AttackRange, range.Color.ToSystemColor());
                    }

                    if (!holdZone.Active)
                    {
                        continue;
                    }

                    var holdzone = hero.BoundingRadius;

                    if (hero.IsMe)
                    {
                        holdzone += ExtraHoldZone;
                    }

                    Drawings.DrawCircle(hero.ServerPosition, holdzone, holdZone.Color.ToSystemColor());
                }

                var drawKillable = Menu["st_orb_draw_minions_killable"].GetValue<MenuColor>();

                if (!drawKillable.Active)
                {
                    return;
                }

                var color = drawKillable.Color;

                foreach (var minion in ObjectCache.GetMinions())
                {
                    var alpha = (int)(minion.Health / AttackDamage(minion) * 255f);

                    if (alpha < 255 && alpha > 0)
                    {
                        Drawings.DrawCircle(
                            minion.Position,
                            minion.BoundingRadius,
                            Color.FromArgb(alpha, color.R, color.G, color.B));
                    }
                }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a value indicating whether the player is able to auto-attack
        /// </summary>
        /// <returns></returns>
        public static bool CanAttack()
        {
            if (Player.IsDashing())
            {
                return false;
            }

            var delay = Player.AttackDelay * 1000f;

            if (Core.ChampionName == "Graves")
            {
                if (!Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return false;
                }

                delay *= 1.0740297f;
                delay -= 716.2381256f;
            }
            else if (Core.ChampionName == "Jhin" && Player.HasBuff("JhinPassiveReload"))
            {
                return false;
            }

            return TickCount - LastAttackTick + Ping / 2f + 25f >= delay;
        }

        /// <summary>
        /// Returns a value indicating whether the player is able to move
        /// </summary>
        /// <returns></returns>
        public static bool CanMove()
        {
            if (Core.ChampionName == "Rengar" && Player.Buffs.Any(buff => buff.Name.StartsWith("rengarq", StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }

            if (!Player.CanCancelAutoAttack() || movementFlag)
            {
                return true;
            }

            var windup = Windup.Get(Ping, Menu["st_orb_problem_stutter"].GetValue<MenuBool>().Value);

            return TickCount - LastAttackTick + Ping / 2f - Player.AttackCastDelay * 1000f >= windup;
        }

        /// <summary>
        /// Counts the current missiles on a target
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static int GetAggroCount(GameObject unit)
        {
            var id = unit.NetworkId;

            return ObjectCache.GetNative<MissileClient>().Count(missile => missile.Target.NetworkId == id);
        }

        /// <summary>
        /// Increases the humanizer tick
        /// </summary>
        private static void TickUp()
        {
            var mode = Menu["st_orb_humanizer"].GetValue<MenuList<HumanizerMode>>().SelectedValue;

            if (mode == HumanizerMode.Off)
            {
                return;
            }

            var array = EnumCache<HumanizerMode>.Description(mode).Split('-');

            BlockOrders = TickCount + Random.Next(int.Parse(array[0]), int.Parse(array[1]));
        }

        /// <summary>
        /// Returns a list of enemy target types this mode can attack
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static HashSet<UnitType> GetTargetSelection(Mode mode)
        {
            var set = new HashSet<UnitType>();
            var smode = mode.ToShort();

            for (int i = 0; i < EnumCache<UnitType>.Count; i++)
            {
                set.Add(Menu[$"st_orb_modes_{smode}_priority_{i}"].GetValue<MenuList<UnitType>>().SelectedValue);
            }

            return set;
        } 

        /// <summary>
        /// Determines whether the specified unit is in the autoattack range
        /// </summary>
        /// <param name="unit">The requested unit</param>
        /// <returns></returns>
        public static bool InAttackRange(AttackableUnit unit)
        {
            var @base = unit as Obj_AI_Base;

            return @base != null
                       ? InAttackRange(new HitData(@base))
                       : Vector2.DistanceSquared(Position, unit.Position.ToVector2())
                         <= Math.Pow(unit.GetRealAutoAttackRange(), 2);
        }

        /// <summary>
        /// Determines whether the specified unit is in the autoattack range
        /// </summary>
        /// <param name="data">The provided hit data</param>
        /// <returns></returns>
        public static bool InAttackRange(HitData data)
        {
            return data.PlayerInRange || data.SoldiersInRange.Count > 0;
        }

        /// <summary>
        /// Gets the <see cref="E:Player"/>'s projectile time to a unit
        /// </summary>
        /// <param name="unit">The requested unit</param>
        /// <returns></returns>
        private static int ProjectileTime(Obj_AI_Base unit)
        {
            var delay = Player.AttackCastDelay * 1000f - 100f + Ping / 2f;

            if (Player.IsMelee || Core.ChampionName == "Azir" || Core.ChampionName == "Velkoz" || Core.ChampionName == "Viktor" && Player.HasBuff("ViktorPowerTransferReturn"))
            {
                return (int)delay;
            }

            return (int)(delay + Vector2.Distance(Position, unit.ServerPosition.ToVector2()) * 1000f / Player.BasicAttack.MissileSpeed);
        }

        /// <summary>
        /// Returns the damage the <see cref="E:Player"/> can currently deal to a unit
        /// </summary>
        /// <param name="unit">The requested unit</param>
        /// <returns></returns>
        private static float AttackDamage(Obj_AI_Base unit)
        {
            return AttackDamage(new HitData(unit));
        }

        /// <summary>
        /// Returns the damage the <see cref="E:Player"/> can currently deal to a unit
        /// </summary>
        /// <param name="data">The requested data</param>
        /// <returns></returns>
        private static float AttackDamage(HitData data)
        {
            var damage = data.PlayerInRange ? Player.GetAutoAttackDamage(data.Unit) : 0d;

            if (data.SoldiersInRange.Count > 0)
            {
                damage += Player.GetSpellDamage(data.Unit, SpellSlot.W) * (1d + 0.25d * (data.SoldiersInRange.Count - 1));
            }

            return (float)damage;
        }

        private static string ToShort(this Mode mode)
        {
            switch (mode)
            {
                case Mode.Combo:
                    return "combo";
                case Mode.LaneClear:
                    return "lc";
                case Mode.None:
                    return "none";
                case Mode.Freeze:
                    return "freeze";
                case Mode.Flee:
                    return "flee";
                case Mode.LastHit:
                    return "lh";
                case Mode.Harass:
                    return "harass";
                default:
                    return "unknown";
            }
        }

        /// <summary>
        /// Gets the current orbwalking point
        /// </summary>
        private static Vector3 Point()
        {
            if (Core.ChampionName == "Draven")
            {

            }

            return Game.CursorPos;
        }

        #endregion

        #region Targeting

        /// <summary>
        /// Gets the best target for the specified mode
        /// </summary>
        /// <param name="mode">The specified <see cref="Enumerations.Mode"/></param>
        /// <returns><see cref="AttackableUnit"/></returns>
        public static AttackableUnit GetTarget(Mode mode)
        {
            var processed = new HashSet<UnitType>();
            var smode = mode.ToShort();

            for (int i = 0; i < EnumCache<UnitType>.Count; i++)
            {
                var next = Menu[$"st_orb_modes_{smode}_priority_{i}"].GetValue<MenuList<UnitType>>().SelectedValue;

                if (!processed.Add(next))
                {
                    continue;
                }

                TargetData data;

                switch (next)
                {
                    case UnitType.Champion:
                        data = GetHero(mode);
                        break;
                    case UnitType.Structure:
                        data = GetStructure(mode);
                        break;
                    case UnitType.Object:
                        data = GetAttackableObject(mode);
                        break;
                    case UnitType.LaneMinion:
                        data = GetKillableMinion(mode);
                        break;
                    case UnitType.LaneClearMinion:
                        data = GetBalanceMinion(mode, processed.Add(UnitType.LaneMinion));
                        break;
                    case UnitType.JungleMinion:
                        data = GetJungleMinion(mode);
                        break;
                    case UnitType.None:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(next), next, null);
                }

                if (data.ShouldWait)
                {
                    break;
                }

                if (data.Target != null)
                {
                    return data.Target;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the best attackable object for the specified mode
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <returns></returns>
        private static TargetData GetAttackableObject(Mode mode)
        {
            var flags = MinionType.Pet;

            if (Menu[$"st_orb_modes_{mode.ToShort()}_targeting_objects_wards"].GetValue<MenuBool>().Value)
            {
                flags |= MinionType.Ward;
            }

            ObjectInfo info;

            return new TargetData(ObjectCache.GetMinions(ObjectTeam.Neutral | ObjectTeam.Enemy, flags, InAttackRange)
                    .ConvertAll(minion => new
                                          {
                                              Minion = minion,
                                              minion.CharData,
                                              IsWard = minion.GetMinionType().HasFlag(MinionTypes.Ward)
                                          })
                    .FindAll(arg => arg.IsWard || AttackDictionary.TryGetValue(arg.CharData.BaseSkinName.ToLower(), out info) && info.Attack(mode))
                    .OrderBy(arg => arg.IsWard)
                    .ThenByDescending(arg => arg.Minion.Health)
                    .ThenByDescending(arg => arg.Minion.DistanceToPlayer())
                    .FirstOrDefault()?
                    .Minion);
        }

        /// <summary>
        /// Gets the <see cref="TargetData"/> of the best killable minion
        /// </summary>
        /// <param name="mode">The requested mode</param>
        /// <returns></returns>
        private static TargetData GetKillableMinion(Mode mode)
        {
            var max = mode == Mode.Freeze ? Menu["st_orb_modes_freeze_targeting_freeze_maxhealth"].GetValue<MenuSlider>().Value : int.MaxValue;

            var data = ObjectCache.GetMinions(ObjectTeam.Enemy, MinionType.Minion, InAttackRange).ConvertAll(minion =>
                        new
                        {
                            Minion = minion,
                            Damage = Math.Min(AttackDamage(minion), max),
                            Prediction = HealthWrapper.GetPrediction(minion, ProjectileTime(minion), FarmDelay),
                            Type = minion.GetMinionType()
                        });

            var killable = data.FindAll(arg => arg.Prediction <= arg.Damage);

            return killable.Count > 0
                       ? new TargetData(
                             killable.OrderByDescending(
                                 arg =>
                                 {
                                     var weight = arg.Prediction / (GetAggroCount(arg.Minion) / 2.4f + 1f);

                                     if (arg.Prediction < 0f)
                                         weight *= 2.8f;
                                     if (HealthWrapper.HasTurretAggro(arg.Minion))
                                         weight /= 2.4f;
                                     if (arg.Type.HasFlag(MinionTypes.Siege))
                                         weight /= 1.8f;
                                     if (arg.Type.HasFlag(MinionTypes.Melee))
                                         weight /= 1.2f;

                                     return weight;
                                 }).First().Minion)
                       : new TargetData(
                             data.Exists(
                                 arg =>
                                 HealthWrapper.GetPrediction(
                                     arg.Minion,
                                     (int)Math.Round(Player.AttackDelay * 2000f),
                                     FarmDelay,
                                     HealthPredictionType.Simulated) < arg.Damage));
        }

        /// <summary>
        /// Searches for a structure for the specified mode
        /// </summary>
        /// <param name="mode">The requested mode</param>
        /// <returns></returns>
        private static TargetData GetStructure(Mode mode)
        {
            var smode = mode.ToShort();
            var collection = new List<AttackableUnit>(2);

            if (Menu[$"st_orb_modes_{smode}_targeting_structures_turret"].GetValue<MenuBool>().Value)
                collection.AddRange(ObjectCache.Get<Obj_AI_Turret>(ObjectTeam.Enemy, InAttackRange));

            if (Menu[$"st_orb_modes_{smode}_targeting_structures_inhibitor"].GetValue<MenuBool>().Value)
                collection.AddRange(ObjectCache.Get<Obj_BarracksDampener>(ObjectTeam.Enemy, InAttackRange));

            if (Menu[$"st_orb_modes_{smode}_targeting_structures_nexus"].GetValue<MenuBool>().Value)
                collection.AddRange(ObjectCache.Get<Obj_HQ>(ObjectTeam.Enemy, InAttackRange));

            return new TargetData(collection.Count == 0 ? null : collection[0]);
        }

        /// <summary>
        /// Gets the hero for the specified mode
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <returns></returns>
        private static TargetData GetHero(Mode mode)
        {
            var header = $"st_orb_modes_{mode.ToShort()}_targeting_champion";
            var ignoreShields = Menu[header + "_ignoreshields"].GetValue<MenuBool>().Value;
            var enemies = ObjectCache.Get<Obj_AI_Hero>(ObjectTeam.Enemy);

            if (Menu[header + "_ignorets"].GetValue<MenuBool>().Value)
            {
                var maxIgnored = Menu[header + "_attacks"].GetValue<MenuSlider>().Value;

                var killable = enemies
                        .ConvertAll(hero => new { Damage = AttackDamage(hero), Hero = hero })
                        .FindAll(arg => arg.Damage > 0f && !Invulnerable.Check(arg.Hero, DamageType.Physical, ignoreShields, arg.Damage))
                        .ConvertAll(arg => new { arg.Hero, arg.Damage, KillHits = arg.Hero.Health / arg.Damage })
                        .FindAll(arg => (int)Math.Ceiling(arg.KillHits) <= maxIgnored)
                        .OrderByDescending(arg => arg.KillHits)
                        .FirstOrDefault()?.Hero;

                if (killable != null)
                {
                    return new TargetData(killable);
                }
            }

            return new TargetData(Variables.TargetSelector.GetTarget(
                float.MaxValue,
                DamageType.Physical,
                ignoreShields,
                Player.ServerPosition,
                enemies.FindAll(enemy => !InAttackRange(enemy) || Menu[$"{header}_blacklist_{enemy.NetworkId}"].GetValue<MenuBool>().Value)));
        }

        /// <summary>
        /// Gets a lane balance minion
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <param name="checkNormal">Determines whether killable minions should be considered</param>
        /// <returns></returns>
        private static TargetData GetBalanceMinion(Mode mode, bool checkNormal)
        {
            if (checkNormal)
            {
                var normal = GetKillableMinion(mode);

                if (normal.ShouldWait || normal.Target != null)
                {
                    return normal;
                }
            }
            
            return new TargetData(ObjectCache.GetMinions(ObjectTeam.Enemy, MinionType.Minion, InAttackRange)
                        .FindAll(minion =>
                        {
                            var pred = HealthWrapper.GetPrediction(
                                minion,
                                (int)Math.Round(Player.AttackDelay * 2000f - 200f + Ping),
                                FarmDelay);

                            return Math.Abs(pred - minion.Health) < float.Epsilon || pred >= AttackDamage(minion) * 2f;
                        })
                        .OrderBy(minion => minion.MaxHealth)
                        .ThenByDescending(minion => minion.Health)
                        .ThenByDescending(minion => minion.DistanceToPlayer())
                        .FirstOrDefault());
        }

        /// <summary>
        /// Gets the jungle minion
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <returns></returns>
        private static TargetData GetJungleMinion(Mode mode)
        {
            var smallPrio = Menu[$"st_orb_modes_{mode.ToShort()}_jungle_smallfirst"].GetValue<MenuBool>().Value;

            return new TargetData(ObjectCache.GetMinions(ObjectTeam.Neutral, MinionType.Jungle, InAttackRange)
                        .ConvertAll(minion => new
                                              {
                                                  Minion = minion,
                                                  Type = minion.GetJungleType()
                                              })
                        .FindAll(arg => arg.Type != JungleType.Unknown)
                        .OrderBy(arg => arg.Type == JungleType.Small == smallPrio)
                        .ThenBy(arg => arg.Type)
                        .ThenByDescending(arg => arg.Minion.Health)
                        .ThenByDescending(arg => arg.Minion.DistanceToPlayer())
                        .FirstOrDefault()?
                        .Minion);
        }

        #endregion
    }
}