namespace SparkTech.SparkWalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers.Damages;

    using SharpDX;

    using SparkTech.Cache;
    using SparkTech.Enumerations;
    using SparkTech.EventData;
    using SparkTech.Helpers;

    using Key = System.Windows.Forms.Keys;

    /// <summary>
    /// An alternative to the <see cref="LeagueSharp.SDK.Orbwalker"/> class.
    /// </summary>
    public static class Orbwalker
    {
        #region Attackable Objects

        /// <summary>
        /// Holds the <see cref="E:CharData.Name"/> of objects that should never be attacked
        /// </summary>
        private static readonly string[] BlackListedNames = { "wardcorpse", "beacon" };

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
                    mode => Menu[$"st_orb_modes_{mode.ToString().ToLower()}_targeting_objects_{displayName.ToMenuUse()}"].GetValue<MenuBool>().Value,
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
                                                                                             UnitType.FreezeMinion)
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
        /// Gets the <see cref="E:Player"/>'s champion name
        /// </summary>
        private static string ChampionName => Core.ChampionName;

        /// <summary>
        /// The orbwalker's <see cref="LeagueSharp.SDK.Core.UI.IMenu.Menu"/> instance
        /// </summary>
        private static readonly Menu Menu = new Menu("st_orb", "SparkWalker");

        /// <summary>
        /// The <see cref="System.Random"/> instance
        /// </summary>
        private static readonly Random Random = new Random(TickCount);

        /// <summary>
        /// Determines whether the <see cref="E:Player"/> can move
        /// </summary>
        private static bool movementFlag;

        /// <summary>
        /// The current tick limiter
        /// </summary>
        private static int limiter;

        /// <summary>
        /// The current ping
        /// </summary>
        private static int ping;

        private static Vector2 myPos;

        /// <summary>
        /// The backing field for <see cref="E:OrbwalkingPoint"/>
        /// </summary>
        private static Vector3 oPoint = Vector3.Zero;

        /// <summary>
        /// Gets or sets the orbwalking point
        /// </summary>
        public static Vector3 OrbwalkingPoint
        {
            get
            {
                return oPoint.IsZero ? Game.CursorPos : oPoint;
            }
            set
            {
                oPoint = value;
            }
        }

        /// <summary>
        /// The backing field for <see cref="E:ScanRange"/>
        /// </summary>
        private static Func<Obj_AI_Minion, float> scanRange;

        /// <summary>
        /// Gets or sets the <see cref="E:UnkillableMinions"/> scan range
        /// </summary>
        public static Func<Obj_AI_Minion, float> ScanRange
        {
            get
            {
                return scanRange ?? (minion => 1200f);
            }
            set
            {
                scanRange = value;
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
        /// The <see cref="E:LaneClearWaitTimeMod"/>
        /// </summary>
        private const float LaneClearWaitTimeMod = 2f;

        /// <summary>
        /// The last target attacked by the <see cref="E:Player"/>, used with the <see cref="E:PlayerTargetSwitch"/> event
        /// </summary>
        private static AttackableUnit lastTarget;

        /// <summary>
        /// Gets the current tick count
        /// </summary>
        private static int TickCount => Variables.TickCount;

        /// <summary>
        /// Gets the current farm delay
        /// </summary>
        private static int FarmDelay
        {
            get
            {
                var delay = Menu["st_orb_misc_adv_farmdelay"].GetValue<MenuSlider>().Value;

                if (ChampionName == "Azir")
                {
                    delay += 125;
                }

                return delay;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the player autoattacks
        /// </summary>
        public static event EventDataHandler<PlayerAttackEventArgs> PlayerAttack = delegate
        {
            movementFlag = false;
        };

        /// <summary>
        /// Fired before the orbwalker - generated attack, useful for cancelling it
        /// </summary>
        public static event EventDataHandler<BeforePlayerUnhumanAttackEventArgs> BeforePlayerUnhumanAttack;

        /// <summary>
        /// Fired after the windup is done
        /// </summary>
        public static event EventDataHandler<AfterPlayerAttackEventArgs> AfterPlayerAttack = delegate
            {
                LastAttackTick = TickCount;

                movementFlag = true;
            };

        /// <summary>
        /// Fired when the Player switches targets
        /// </summary>
        public static event EventDataHandler<PlayerTargetSwitchEventArgs> PlayerTargetSwitch;

        /// <summary>
        /// Fired when you aren't able to execute minions using just basic attacks
        /// </summary>
        public static event EventDataHandler<UnkillableMinionsEventArgs> UnkillableMinions;

        /// <summary>
        /// Fired when there are too many minions around for the orbwalker
        /// </summary>
        public static event EventDataHandler<SpellFarmSuggestedEventArgs> SpellFarmSuggested;

        #endregion

        #region Enable Handlers

        /// <summary>
        /// Determines whether the orbwalker has already been added
        /// </summary>
        public static bool Initialized { get; private set; }

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
                return Initialized && (Attacks || Movement);
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
        private static readonly string[] AttackResettingBuffs = { "poppypassivebuff", "sonapassiveready" };

        /// <summary>
        /// Gets the last autoattack tick
        /// </summary>
        public static int LastAttackTick { get; private set; }

        /// <summary>
        /// Gets the last movement tick
        /// </summary>
        public static int LastMovementTick { get; private set; }

        /// <summary>
        /// Resets the specified timers 
        /// </summary>
        public static void ResetTimer(bool resetMoveFlag = false, bool resetAttack = true)
        {
            if (resetMoveFlag)
            {
                movementFlag = true;
            }

            if (resetAttack)
            {
                LastAttackTick = 0;
            }
        }

        #endregion

        #region Permissions

        /// <summary>
        /// Indicates whether attacks are allowed for the specified <see cref="Mode"/>
        /// </summary>
        /// <param name="lmode">The orbwalking mode</param>
        /// <returns></returns>
        private static bool HasAttackPermissions(Mode lmode)
        {
            return Attacks && lmode != Mode.None && Menu["st_orb_attacks"].GetValue<MenuBool>().Value
                   && Menu[$"st_orb_modes_{lmode.ToString().ToLower()}_attack"].GetValue<MenuBool>().Value;
        }

        /// <summary>
        /// Indicates whether movement is allowed for the specified <see cref="Mode"/>
        /// </summary>
        /// <param name="lmode">The orbwalking mode</param>
        /// <returns></returns>
        private static bool HasMovementPermissions(Mode lmode)
        {
            return Movement && lmode != Mode.None && Menu["st_orb_movement"].GetValue<MenuBool>().Value
                   && Menu[$"st_orb_modes_{lmode.ToString().ToLower()}_movement"].GetValue<MenuBool>().Value
                   && !Menu["st_orb_keybind_blockmov"].GetValue<MenuKeyBind>().Active;
        }

        #endregion

        #region Mode Handlers

        /// <summary>
        /// Saves the value of a custom mode
        /// </summary>
        private static Mode? cmode;

        /// <summary>
        /// Returns the current <see cref="Enumerations.Mode"/> and as well as allows you to override the settings
        /// </summary>
        public static Mode Mode
        {
            get
            {
                return cmode ?? EnumCache<Mode>.Values.Find(mode => Menu[$"st_orb_modes_{mode.ToString().ToLower()}_key"].GetValue<MenuKeyBind>().Active);
            }
            set
            {
                cmode = value;
            }
        }

        /// <summary>
        /// Gets or sets the default mode
        /// </summary>
        public static bool CustomMode
        {
            get
            {
                return cmode.HasValue;
            }
            set
            {
                if (value)
                {
                    cmode = Mode.None;
                }
                else
                {
                    cmode = null;
                }
            }
        }

        #endregion

        #region CanAttack / CanMove

        /// <summary>
        /// Returns a value indicating whether the player is able to auto-attack
        /// </summary>
        /// <returns><see cref="bool"/></returns>
        public static bool CanAttack()
        {

        }

        /// <summary>
        /// Returns a value indicating whether the player is able to move
        /// </summary>
        /// <returns><see cref="bool"/></returns>
        public static bool CanMove()
        {
            if (!Player.CanCancelAutoAttack())
            {
                return true;
            }

            if (movementFlag)
            {
                return true;
            }
        }

        #endregion

        #region Menu Provider

        /// <summary>
        /// Initializes <see cref="Orbwalker"/> to a menu
        /// </summary>
        /// <param name="menuToAttachTo">The menu to attach to</param>
        public static void Initialize(Menu menuToAttachTo)
        {
            if (Initialized)
            {
                return;
            }

            Initialized = true;

            menuToAttachTo.Add(Menu);

            var modesMenu = Menu.Add(new Menu("st_orb_modes", "Modes"));
            {
                foreach (var config in ModeConfiguration)
                {
                    var header = $"{modesMenu.Name}_{config.Key.ToString().ToLower()}";
                    var modeMenu = modesMenu.Add(new Menu(header, config.Key.ToString()));
                    {
                        var targetingMenu = modeMenu.Add(new Menu(header + "_targeting", "Targeting"));
                        {
                            var champMenu = targetingMenu.Add(new Menu(header + "_targeting_champion", "Champions"));
                            {
                                
                            }

                            var objectMenu = targetingMenu.Add(new Menu(header + "_targeting_objects", "Objects"));
                            {
                                objectMenu.Add(new MenuSeparator(header + "_targeting_objects_info", "Attack the following objects:"));

                                foreach (var info in AttackDictionary.Select(pair => pair.Value).Where(info => info.AddToMenu))
                                {
                                    objectMenu.Add(new MenuBool($"{header}_targeting_objects_{info.DisplayName.ToMenuUse()}", info.DisplayName, info.AttackByDefault));
                                }
                            }
                        }
                        
                        modeMenu.Add(new MenuKeyBind(header + "_active", $"{config.Key} active", config.Value.Key, KeyBindType.Press));
                    }
                }

                foreach (var config in ModeConfiguration) {
                    ExtendMenu(
                    modeMenu.Add(
                        new Menu($"{modeMenu.Name}_{config.Key.ToString().ToLower()}", config.Key.ToString())),
                    config.Value.Key,
                    config.Value.UnitsEnabled);
                }

                /*
private static void ExtendMenu(Menu premenu, Key key, params UnitType[] defValues)
{
var header = premenu.Name + "_targeting";
var menu = premenu.Add(new Menu(header, "Targeting"));

var championMenu = menu.Add(new Menu(header + "_champion", "Champions"));
{
championMenu.Add(new MenuBool(header + "_champion_invulnerable", "Attack invulnerable heroes"));

var blacklistMenu = championMenu.Add(new Menu(header + "_champion_blacklist", "Blacklist"));
{
    blacklistMenu.Add(new MenuSeparator(header + "_champion_blacklist_info", "Disable attacking for the following champions:"));

    foreach (var hero in GameObjects.EnemyHeroes)
    {
        blacklistMenu.Add(new MenuBool(header + "_champion_blacklist_" + hero.NetworkId, hero.ChampionName()));
    }
}
}

var structureMenu = menu.Add(new Menu(header + "_structure", "Structures"));
{
var structureNames = new[] { "nexus", "inhibitor", "turret" };

foreach (var name in structureNames)
{
    structureMenu.Add(new MenuBool($"{header}_structure_{name}", $"Attack {name}", true));
}
}

var freezeMenu = menu.Add(new Menu(header + "_freeze", "Freeze"));
{
freezeMenu.Add(new MenuBool(header + "_freeze_aggro", "", true));
freezeMenu.Add(new MenuSlider(header + "_freeze_maxhealth", "", 30, 0, 50));
}

var objectsMenu = Menu.Add(new SubMenu("st_orb_objects", "Objects"));
{
objectsMenu.Add(new MenuSeparator("st_orb_objects_info", "Attack the following objects:"));

foreach (var info in AttackDictionary.Values.Where(objectInfo => objectInfo.AddToMenu))
{
    objectsMenu.Add(new MenuBool($"st_orb_objects_{info.DisplayName.ToMenuUse()}", $" - {info.DisplayName}", info.AttackByDefault));
}
}

for (var i = EnumCache<UnitType>.Count; i > 0; i--)
{
var defValue = defValues.Length >= i ? defValues[i] : UnitType.None;

menu.Add(new MenuList<UnitType>($"{header}_priority_{i}", "") { SelectedValue = defValue });
}

premenu.Add(new MenuBool(premenu.Name + "_attacks", "Enable attacks", true));
premenu.Add(new MenuBool(premenu.Name + "_movement", "Enable movement", true));
}
*/

                /*
                private static void AddTargettingMenu(Menu submenu, Mode lmode)
                {
                    var mycore = $"st_orb_modes_{lmode.ToString().ToLower()}_targetting";
                    var core = new Menu("Targetting", mycore);
                    var names = UnitRange.Where(x => !unit.ToLower().Contains("minion")).Concat(new[] { "Lasthit Minion", "Laneclear Minion" }).ToArray();
                    submenu.AddSubMenu(core);
                    core.AddItem(
                        new MenuItem($"{mycore}_info", "Lower value <=> Higher priority").SetTooltip(
                            "Will attempt to seek targets in order set by You. You can't break a mode just by changing these settings but setting them in an unthoughtful way may cause unexpected behaviour"));

                    for (int i = 0; i < names.Length; i++)
                    {
                        var prioDisp = i == 1 ? "Top priority" : $"Priority {i}: ";

                        core.AddItem(
                            new MenuItem($"{mycore}_priority_{i}", prioDisp).SetValue(
                                new StringList(names.Select(Helper.SpaceString).ToArray(), GetIndex(i, lmode))));
                    }

                    core.AddSeparator();

                    core.AddItem(new MenuItem($"{mycore}_junglepriob", "Prioritize bigger minions").SetValue(true));

                    core.AddItem(
                        new MenuItem(Helper.SeparatorText, "Note: ").SetTooltip(
                            "To tune champion picking, please optimize Your Target Selector settings."));
                }
                */
            }

            var drawMenu = Menu.Add(new Menu("st_orb_draw", "Drawings"));
            {
                var rangeMenu = drawMenu.Add(new Menu("st_orb_draw_ranges", "Ranges"));
                {
                    var myRangeMenu = rangeMenu.Add(new Menu("st_orb_draw_ranges_me", "Me"));
                    {

                    }

                    var allysubmenu = rangeMenu.Add(new Menu("st_orb_draw_ranges_allies", "Allies"));
                    var enemysubmenu = rangeMenu.Add(new Menu("st_orb_draw_ranges_enemies", "Enemies"));

                    var multiplicator = rangeMenu.Add(new MenuSlider("st_orb_draw_ranges_multiplicator", "Default multiplicator", 2, 0, 10));
                    var adcRanges = rangeMenu.Add(new MenuBool("st_orb_draw_ranges_adc", "Turn on by default for enemy ADC", true));

                    foreach (var hero in GameObjects.Heroes.Where(hero => !hero.IsMe))
                    {
                        var id = hero.NetworkId;

                        var heroMenu = (hero.IsEnemy ? enemysubmenu : allysubmenu).Add(new Menu($"st_orb_draw_ranges_{id}", hero.ChampionName()));
                        {
                            heroMenu.Add(new MenuSlider($"st_orb_draw_ranges_radius_{id}", "Radius to activate", (int)((hero.AttackRange + hero.BoundingRadius) * multiplicator.GetValue<MenuSlider>().Value), 0, 5000));
                            heroMenu.Add(new MenuColor($"st_orb_draw_ranges_range_{id}", "Range", hero.IsEnemy ? Color.Red : Color.Blue) { Active = hero.IsADC() && adcRanges.GetValue<MenuBool>().Value });
                            heroMenu.Add(new MenuColor($"st_orb_draw_ranges_holdzone_{id}", "HoldZone", Color.White) { Active = false });
                        }
                    }
                }

                var minionMenu = drawMenu.Add(new Menu("st_orb_draw_minions", "Minions"));
                {

                }
            }

            var miscMenu = Menu.Add(new Menu("Miscallenous", "st_orb_misc"));
            {
                miscMenu.Add(new MenuItem("st_orb_misc_windup", "Windup").SetValue(sliderWindup));
                miscMenu.Add(new MenuItem("st_orb_misc_windup_autoset", "^ Autoset").SetValue(true));
                miscMenu.Add(new MenuItem("st_orb_misc_farmdelay", "Farm Delay").SetValue(new Slider(70, 0, 150)));
                miscMenu.Add(new MenuItem("st_orb_misc_movdelay", "Movement Delay").SetValue(new Slider(40, 0, 150)));
                miscMenu.Add(new MenuItem("st_orb_misc_missilecheck", "Missile Check").SetValue(true));
            }

            var advMenu = new Menu("Advanced", "st_orb_adv");
            {
                var varMenu = new Menu("Variables", "st_orb_adv_var");
                {
                    advMenu.AddItem(new MenuItem("st_orb_adv_var_canattackmod", "CanAttackMod").SetValue(new Slider(25, 0, 50)).SetTooltip("Default value: 25"));
                    advMenu.AddItem(new MenuItem("st_orb_adv_var_laneclearwaittimemod", "LaneClearWaitTimeMod").SetValue(new Slider(2, 0, 5)).SetTooltip("Default value: 2"));
                    advMenu.AddItem(new MenuItem("st_orb_adv_var_ondocast_delay", "Delay OnDoCast").SetValue(true));
                    advMenu.AddItem(new MenuItem("st_orb_adv_var_ondocast_value", "OnDoCast Delay Value").SetValue(new Slider(30)));
                    advMenu.AddItem(new MenuItem("st_orb_adv_var_ondocast_autoset", "^ Autoset").SetValue(true).SetTooltip("OnDoCast will be delayed or not automatically"));
                }
                advMenu.AddSubMenu(varMenu);
                advMenu.Add(new MenuBool("st_orb_adv_ignoreshields", "Ignore shield checks on champs (TODO: Review this option)")).SetTooltip("Will avoid autoattacking shielded champions"))
                ;
            }
            Menu.AddSubMenu(advMenu);

            var keyMenu = Menu.Add(new Menu("Keybinds", "st_orb_key"));
            {
                keyMenu.Add(new MenuKeyBind("st_orb_key_movblock", "Movement block", Key.P, KeyBindType.Press));
                keyMenu.AddItem(new MenuItem("st_orb_key_combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
                keyMenu.AddItem(new MenuItem("st_orb_key_combo2", "Combo alternate").SetValue(new KeyBind('/', KeyBindType.Press)));
                keyMenu.Add(new MenuKeyBind("st_orb_modes_harras_key", "Keybind", Key.C, KeyBindType.Press));
                keyMenu.Add(new MenuKeyBind("st_orb_modes_laneclear_key", "Keybind", Key.V, KeyBindType.Press));
                lasthitMenu.AddItem(new MenuItem("st_orb_modes_lasthit_key", "Keybind").SetValue(new KeyBind('X', KeyBindType.Press)));
                keyMenu.AddItem(new MenuItem("st_orb_key_flee", "Flee Mode").SetValue(new KeyBind('Z', KeyBindType.Press)));
            }

            Menu.AddItem(new MenuItem("st_orb_attacks", "Attacks").SetValue(true));
            Menu.AddItem(new MenuItem("st_orb_movement", "Movement").SetValue(true));

            Game.OnUpdate += delegate
            {
                try
                {
                    Game.OnUpdate += delegate
                    {
                        if (Menu["st_orb_misc_ticklimiter"].GetValue<MenuSlider>().Value + limiter >= Environment.TickCount)
                        {
                            return;
                        }

                        limiter = Environment.TickCount;

                        if (Menu["st_orb_misc_windup_autoset"].GetValue<MenuBool>().Value)
                        {
                            var ping = Game.Ping;
                            var windUp = ping - 20;

                            if (ping >= 100)
                            {
                                windUp += ping / 20;
                            }
                            else if (ping > 40 && ping < 100)
                            {
                                windUp += ping / 10;
                            }
                            else if (ping <= 40)
                            {
                                windUp += 20;
                            }
                            if (windUp < 40)
                            {
                                windUp = 40;
                            }

                            Menu["windup"].GetValue<MenuSlider>().Value = windUp;
                        }

                        if (Menu.Item("st_orb_adv_var_ondocast_autoset").GetValue<bool>())
                        {
                            Menu.Item("st_orb_adv_var_ondocast_value").SetValue(Game.Ping <= 30);
                        }

                        InnerOrbwalk(Game.CursorPos);
                    };

                    var unkillable = GameObjects.EnemyMinions.Where(minion => minion.IsValidTarget(ScanRange(minion)) && minion.IsMinion() && Health.GetPrediction(minion, (int)minion.GetTimeToHit(), FarmDelay) <= 0);

                    var minions = unkillable.ToList();

                    if (minions.Any())
                    {
                        UnkillableMinions?.Invoke(new UnkillableMinionsEventArgs(minions));
                    }
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            };

            Obj_AI_Base.OnDoCast += (sender, args) =>
                {
                    try
                    {
                        if (!sender.IsMe)
                        {
                            return;
                        }

                        var spellName = args.SData.Name;

                        if (AutoAttack.IsAutoAttack(spellName))
                        {
                            AfterPlayerAttack(new AfterPlayerAttackEventArgs((AttackableUnit)args.Target));
                        }

                        if (AutoAttack.IsAutoAttackReset(spellName))
                        {
                            ResetTimer();
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                    }
                };

            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                try
                {
                    if (!sender.IsMe)
                    {
                        return;
                    }

                    var data = args.SData;
                    var spellName = data.Name;

                    if (AutoAttack.IsAutoAttackReset(spellName) && Math.Abs(data.SpellCastTime) < float.Epsilon)
                    {
                        ResetTimer();
                    }

                    if (!AutoAttack.IsAutoAttack(spellName))
                    {
                        return;
                    }

                    LastAttackTick = TickCount - ping / 2;

                    var target = (AttackableUnit)args.Target;

                    PlayerAttack(new PlayerAttackEventArgs(target));

                    if (PlayerTargetSwitch != null && (!lastTarget.IsValidTarget() || !lastTarget.Compare(target)))
                    {
                        PlayerTargetSwitch(new PlayerTargetSwitchEventArgs(lastTarget, target));
                    }

                    lastTarget = target;
                }
                catch (Exception ex)
                {
                    ex.Catch();
                }
            };

            Spellbook.OnStopCast += (spellbook, args) =>
            {
                try
                {
                    if (spellbook?.Owner?.IsMe == true && args.StopAnimation && args.DestroyMissile)
                    {
                        ResetTimer(true);
                    }
                }
                catch (Exception ex)
                {
                    ex.Catch();
                }

            };

            Obj_AI_Base.OnBuffAdd += (sender, args) =>
            {
                try
                {
                    if (sender.IsMe && AttackResettingBuffs.Contains(args.Buff.DisplayName.ToLower()))
                    {
                        ResetTimer();
                    }
                }
                catch (Exception ex)
                {
                    ex.Catch();
                }
            };

            /*
            Obj_AI_Base.OnNewPath += (sender, args) =>
            {
                if (sender.IsMe && !args.IsDash)
                {
                    LastMovementTick = Environment.TickCount;
                }
            };
            */

            // Waiting for the global PlaySharp library :^)
            LeagueSharp.Drawing.OnDraw += delegate
            {
                foreach (var hero in GameObjects.Heroes)
                {
                    try
                    {
                        if (hero.IsMe)
                        {
                            if (Player.IsDead)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            var id = hero.NetworkId;

                            var radius = Menu[$"st_orb_draw_ranges_radius_{id}"].GetValue<MenuSlider>().Value;
                            var itemRange = Menu[$"st_orb_draw_ranges_range_{id}"].GetValue<MenuColor>();
                            var itemHoldZone = Menu[$"st_orb_draw_ranges_holdzone_{id}"].GetValue<MenuColor>();

                            var rangeActive = itemRange.Active;
                            var holdZoneActive = itemHoldZone.Active;

                            if ((!rangeActive && !holdZoneActive) || !hero.IsValidTarget(radius != 0 ? radius : float.MaxValue, false))
                            {
                                continue;
                            }

                            if (rangeActive)
                            {
                                LeagueSharp.Drawing.DrawCircle(hero.Position, hero.AttackRange, itemRange.Color.ToSystemColor());
                            }

                            if (holdZoneActive)
                            {
                                LeagueSharp.Drawing.DrawCircle(hero.Position, hero.BoundingRadius, itemHoldZone.Color.ToSystemColor());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Catch();
                    }
                }
            };
        }

        #endregion

        /// <summary>
        /// Gets the informations about the best target for the specified mode
        /// </summary>
        /// <param name="unitType">The specified unit type</param>
        /// <param name="lmode">The specified orbwalking mode</param>
        /// <returns><see cref="TargetData"/></returns>
        public static TargetData GetTargetData(UnitType unitType, Mode lmode)
        {
            // TODO: Check whichever of them needs a ShouldWait
            switch (unitType)
            {
                case UnitType.Champion:
                    return new TargetData(GetHero(lmode));
                case UnitType.Structure:
                    return new TargetData(GetStructure(lmode));
                case UnitType.Object:
                    return new TargetData(GetAttackableObjects(lmode).FirstOrDefault());
                case UnitType.FreezeMinion:
                case UnitType.LaneMinion:
                    return new TargetData(GetMinionData(lmode).Select(tuple => tuple.Item1).FirstOrDefault() ?? GetTurretBalanceMinions(lmode).FirstOrDefault());
                case UnitType.LaneClearMinion:

                    
                case UnitType.JungleMinion:

                    
                case UnitType.None:
                    return new TargetData(null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        /// <summary>
        /// Gets the target for the specified mode
        /// </summary>
        /// <param name="lmode">The specified <see cref="Mode"/></param>
        /// <returns><see cref="AttackableUnit"/></returns>
        public static AttackableUnit GetTarget(Mode lmode)
        {
            var smode = lmode.ToString().ToLower();

            for (int i = 0; i < EnumCache<UnitType>.Count; i++)
            {
                var data = GetTargetData(Menu[$"st_orb_modes_{smode}_priority_{i}"].GetValue<MenuList<UnitType>>().SelectedValue, lmode);

                if (data.Target != null)
                {
                    return data.Target;
                }

                if (data.ShouldWait)
                {
                    return null;
                }
            }

            return null;
        }

        public static HitData GetHitData(AttackableUnit unit)
        {
            var range = Player.AttackRange + Player.BoundingRadius + unit.BoundingRadius;

            var enemyPos = ((unit as Obj_AI_Base)?.ServerPosition ?? unit.Position).ToVector2();

            if (ChampionName == "Caitlyn")
            {
                var @base = unit as Obj_AI_Base;

                if (@base != null && @base.HasBuff("caitlynyordletrapinternal"))
                {
                    range += 650f;
                }
            }

            var byplayer = Vector2.DistanceSquared(enemyPos, myPos) <= range * range;

            if (ChampionName != "Azir")
            {
                return byplayer ? HitData.Yes : HitData.No;
            }

            //TODO: Soldiers
            return new HitData(byplayer);
        }

        public static bool InAttackRange(AttackableUnit unit)
        {
            var data = GetHitData(unit);
            var count = data.SoldiersInRange.Count;

            if (data.PlayerInRange)
            {
                count++;
            }

            return count > 0;
        }

        private static int GetProjectileTime(AttackableUnit unit)
        {
            
        }

        private static float GetAttackDamage(Obj_AI_Base unit)
        {
            
        }

        /// <summary>
        /// Autoattacks and moves to a position
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="target">The target</param>
        /// <param name="moveMagnet">The movement magnet</param>
        public static void Orbwalk(Vector3 position, AttackableUnit target = null, bool moveMagnet = false)
        {
            
        }

        /// <summary>
        /// Moves champion to the specified position
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="target">The target</param>
        /// <param name="moveMagnet">The movement magnet</param>
        public static void Reposition(Vector3 position, AttackableUnit target = null, bool moveMagnet = false)
        {
            
        }

        private static bool ShouldWait(Func<Obj_AI_Minion, double> maxHealth = null)
        {
            return GameObjects.EnemyMinions.Any(minion =>
            {
                if (!minion.InAutoAttackRange() || minion.IsZombie)
                {
                    return false;
                }

                var time = Math.Max((int)Math.Round(Player.AttackDelay * 1000 * LaneClearWaitTimeMod), 0);
                var simulation = Health.GetPrediction(minion, time, FarmDelay, HealthPredictionType.Simulated);
                var damage = maxHealth?.Invoke(minion) ?? Player.GetAutoAttackDamage(minion);

                return simulation < damage;
            });
        }

        private static bool CanInnerMove(bool justMaths = false)
        {
            if (!Player.CanCancelAutoAttack())
            {
                return true;
            }

            if (!justMaths && movementFlag)
            {
                return true;
            }

            if (LastAttackTick > Environment.TickCount)
            {
                return false;
            }

            var windup = Menu["st_orb_misc_windup"].GetValue<MenuSlider>().Value;

            if (ChampionName == "Rengar" && Player.Buffs.Any(buff =>
            {
                var name = buff.Name.ToLower();
                return name == "rengarqbase" || name == "rengarqemp";
            }))
            {
                windup += 200;
            }

            return ResponsiveTickCount >= LastAttackTick + Player.AttackCastDelay * 1000 + windup;
        }

        private static bool CanInnerAttack()
        {
            if (!Player.CanAttack || Player.IsWindingUp || Player.IsDashing())
            {
                return false;
            }

            var delay = Player.AttackDelay * 1000d;

            // ReSharper disable once InvertIf
            if (ChampionName == "Graves")
            {
                if (!Player.HasBuff("GravesBasicAttackAmmo1"))
                {
                    return false;
                }

                // Blame other devs :s
                delay = delay * 1.07 - 716.23;
            }

            return ResponsiveTickCount + Menu["st_orb_adv_var_canattackmod"].GetValue<MenuSlider>().Value >= LastAttackTick + delay;
        }

        private static void InnerOrbwalk(Vector3 position, AttackableUnit target = null, bool moveMagnet = false)
        {
            try
            {
                if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen || Player.IsDead || Player.Spellbook.IsAutoAttacking && !CanCancelAttack)
                {
                    return;
                }

                var shouldWait = false;

                if (!target.IsValidTarget())
                {
                    var data = GetTargetinn(GetInnerMode());

                    target = data.Item1;
                    shouldWait = data.Item2;
                }

                if (!shouldWait && target.InAutoAttackRange() && CanInnerAttack() && HasAttackPermissions())
                {
                    var args = new BeforePlayerUnhumanAttackEventArgs(target);

                    BeforePlayerUnhumanAttack?.Invoke(args);

                    if (!args.CancelAttack)
                    {
                        if (Player.IssueOrder(GameObjectOrder.AutoAttack, target))
                        {
                            lastAttackTick = ResponsiveTickCount;
                        }

                        return;
                    }
                }

                Reposition(position, target, moveMagnet);
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        private static void InnerMoveTo(Vector3 position, AttackableUnit target = null, bool moveMagnet = false)
        {
            try
            {
                if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen || Player.IsDead || Player.Spellbook.IsCastingSpell || !Player.CanMove || Player.Spellbook.IsChanneling || Player.Spellbook.IsCharging || Player.IsDashing())
                {
                    return;
                }

                if (!HasMovementPermissions() || !CanInnerMove())
                {
                    return;
                }

                if (Player.Path.Length != 0)
                {
                    if (Player.ServerPosition.Distance(position) < Menu.Item("st_orb_misc_holdpos").GetValue<Slider>().Value)
                    {
                        Player.IssueOrder(Menu.Item("st_orb_adv_stopbymoving").GetValue<bool>() ? GameObjectOrder.MoveTo : GameObjectOrder.Stop, Menu.Item("st_orb_adv_stopmovingtoppos").GetValue<bool>() ? Player.Position : Player.ServerPosition);
                    }
                }
                else if (Player.IssueOrder(GameObjectOrder.MoveTo, !Menu.Item("st_orb_misc_shortmc").GetValue<KeyBind>().Active ? position : Player.ServerPosition + 200 * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D()))
                {
                    lastMovementTick = ResponsiveTickCount;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        #region Targeting

        private static Obj_AI_Minion GetMinion(MinionReceiverType type, string smode, byte? maxHealth = null)
        {
            Obj_AI_Minion minion = null;

            switch (type)
            {
                case MinionReceiverType.Lane:
                    minion = (from x in ObjectManager.Get<Obj_AI_Minion>().Where(x => InAutoAttackRange(x) && x.IsMinion() && x.Team == EnemyTeam).OrderBy(x => x.GetMinionType().HasFlag(MinionTypes.Siege)).ThenBy(x => x.IsMelee).ThenByDescending(x => x.MaxHealth) let pred = Health.GetPrediction(x, (int)Player.AttackCastDelay * 1000 - 100 + Game.Ping / 2 + 1000 * Math.Max(0, (int)(Player.Distance(x) - Player.BoundingRadius)) / (int)Player.GetProjectileSpeed(), FarmDelay) where pred > 0 && pred <= (maxHealth ?? Player.GetAutoAttackDamage(x)) select x).FirstOrDefault();
                    break;
                case MinionReceiverType.LaneExtra:
                    if (!ShouldWait(maxHealth))
                    {
                        //     minion =
                    }
                    break;
                case MinionReceiverType.Jungle:
                    var jungle = ObjectManager.Get<Obj_AI_Minion>().Where(x => InAutoAttackRange(x) && x.Team == GameObjectTeam.Neutral && x.Health <= Player.GetAutoAttackDamage(x));

                    minion = jungle.FirstOrDefault();
                    break;
                case MinionReceiverType.JungleExtra:
                    break;
                case MinionReceiverType.UnderTower:
                    break;
                case MinionReceiverType.UnderTowerExtra:
                    break;
                case MinionReceiverType.Object:
                    minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => InAutoAttackRange(x) && !x.IsMinion() && AttackObject(x));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return minion;
        }

        private static Tuple<AttackableUnit, bool> GetTargetinn(Mode lmode)
        {
            switch (lmode)
            {
                case Mode.Combo:
                case Mode.LaneClear:
                case Mode.Harass:
                case Mode.LastHit:
                case Mode.Freeze:
                    AttackableUnit result = null;
                    var smode = lmode.ToString().ToLower();

                    for (var i = 0; i < Enum.GetNames(typeof(UnitRange)).Length; i++)
                    {
                        if (result != null)
                        {
                            break;
                        }
                        var item = Menu[$"st_orb_modes_{smode}_priority_{i}"].GetValue<MenuList<string>>().SelectedValue;

                        switch (item)
                        {
                            case null:
                                Logging.Write()(LogLevel.Error, new ArgumentNullException(nameof(item)));
                                break;
                            default:
                                Logging.Write()(LogLevel.Error, new ArgumentOutOfRangeException(nameof(item), item, null));
                                break;
                        }
                    }

                    return Tuple.Create(null as AttackableUnit, true);
                case Mode.Flee:
                case Mode.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(lmode), lmode, null);
            }
        }

        /// <summary>
        /// Gets the best attackable object for the specified mode
        /// </summary>
        /// <param name="mode">The specified mode</param>
        /// <returns></returns>
        private static TargetData GetAttackableObject(Mode mode)
        {
            var flags = MinionType.Pet;

            if (Menu[$"st_orb_modes_{mode.ToString().ToLower()}_targeting_objects_wards"].GetValue<MenuBool>().Value)
            {
                flags |= MinionType.Ward;
            }

            ObjectInfo info;

            return new TargetData(ObjectCache.GetMinions(ObjectTeam.Neutral | ObjectTeam.Enemy, flags, InAttackRange)
                    .Select(minion => new { Minion = minion, Name = minion.CharData.BaseSkinName.ToLower(), IsWard = minion.GetMinionType().HasFlag(MinionTypes.Ward) })
                    .Where(arg => arg.IsWard || !BlackListedNames.Contains(arg.Minion.CharData.Name.ToLower()) && AttackDictionary.TryGetValue(arg.Name, out info) && info.Attack(mode))
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
            var max = mode == Mode.Freeze ? Menu[$"st_orb_modes_{mode.ToString().ToLower()}_targeting_freeze_maxhealth"].GetValue<MenuSlider>().Value : int.MaxValue;
            var onlykillable = Menu[$"st_orb_modes_{mode.ToString().ToLower()}_targeting_minions_onlykillable"].GetValue<MenuBool>().Value;

            var data = ObjectCache.GetMinions(ObjectTeam.Enemy, MinionType.Minion, InAttackRange).Select(minion =>
                        new
                        {
                            Minion = minion, Damage = GetAttackDamage(minion),
                            Prediction = HealthWrapper.GetPrediction(minion, GetProjectileTime(minion), FarmDelay),
                            Type = minion.GetMinionType()
                        })
                    .ToList();

            var killable = data.FindAll(arg =>
                    arg.Prediction <= max && (!onlykillable || arg.Prediction > 0f) && arg.Prediction <= GetAttackDamage(arg.Minion))
                    .OrderByDescending(arg =>
                        {
                            var weight = arg.Prediction / HealthWrapper.GetAggroCount(arg.Minion);

                            if (HealthWrapper.HasTurretAggro(arg.Minion)) weight /= 2.4f;
                            if (arg.Type.HasFlag(MinionTypes.Siege)) weight /= 1.8f;
                            if (arg.Type.HasFlag(MinionTypes.Melee)) weight /= 1.25f;

                            return weight;
                        }).FirstOrDefault()?.Minion;

            return killable != null ? new TargetData(killable) : new TargetData(null,
                             data.Exists(arg =>
                                 {
                                     var time = Math.Max((int)Math.Round(Player.AttackDelay * 1000 * LaneClearWaitTimeMod), 0);
                                     var simulation = HealthWrapper.GetPrediction(arg.Minion, time, FarmDelay, true);
                                     var damage = Math.Min(max, arg.Damage);

                                     return simulation < damage;
                                 }));
        }

        /// <summary>
        /// Searches for a structure for the specified mode
        /// </summary>
        /// <param name="mode">The requested mode</param>
        /// <returns></returns>
        private static TargetData GetStructure(Mode mode)
        {
            var smode = mode.ToString().ToLower();
            var collection = new List<AttackableUnit>(2);

            if (Menu[$"st_orb_modes_{smode}_targeting_structures_turret"].GetValue<MenuBool>().Value)
                collection.AddRange(ObjectCache.Get<Obj_AI_Turret>(ObjectTeam.Enemy, InAttackRange));

            if (Menu[$"st_orb_modes_{smode}_targeting_structures_inhibitor"].GetValue<MenuBool>().Value)
                collection.AddRange(ObjectCache.Get<Obj_BarracksDampener>(ObjectTeam.Enemy, InAttackRange));

            if (Menu[$"st_orb_modes_{smode}_targeting_structures_nexus"].GetValue<MenuBool>().Value)
                collection.AddRange(ObjectCache.Get<Obj_HQ>(ObjectTeam.Enemy, InAttackRange));

            return new TargetData(collection.Count > 0 ? collection[0] : null);
        }

        private static IEnumerable<Obj_AI_Minion> GetTurretBalanceMinions(Mode mode)
        {
            var turret = GameObjects.AllyTurrets.FirstOrDefault(t => t.DistanceToPlayer() < 1500);

            if (turret == null)
            {
                yield break;
            }

            var turretRange = turret.AttackRange;

            var reachable =
                GameObjects.EnemyMinions.Where(
                    minion =>
                    minion.IsValidTarget(turretRange, true, turret.Position)
                    && minion.DistanceToPlayer() <= Player.GetRealAutoAttackRange(minion)).ToList();

            if (reachable.Count == 0)
            {
                yield break;
            }

            Func<Obj_AI_Minion, bool> melee = minion =>
            {
                return false;

                //var health = minion.Health;
                //var turretDamage = turret.GetAutoAttackDamage(minion);

                //while (health > 0)
                //{

                //}
            };

            Func<Obj_AI_Minion, bool> ranged = minion =>
            {
                return false;

                //return minion.MaxHealth - minion.Health
                //       < Health.GetPrediction(minion, (int)minion.GetTimeToHit(), FarmDelay)
                //       + Player.GetAutoAttackDamage(minion);
            };

            foreach (
                var minion in
                    reachable.Where(minion => (minion.IsMelee ? melee : ranged)(minion)).OrderBy(Health.HasTurretAggro).ThenBy(Health.HasMinionAggro))
            {
                yield return minion;
            }
        }

        private static Obj_AI_Hero GetHero(Mode lmode)
        {
            var header = $"st_orb_modes_{lmode.ToString().ToLower()}_targeting_hero";
            var ignoreShields = Menu[header + "_ignoreshields"].GetValue<MenuBool>().Value;

            // ReSharper disable once InvertIf
            if (Menu[header + "_ignorets"].GetValue<MenuBool>().Value)
            {
                var maxIgnored = Menu[header + "_attacks"].GetValue<MenuSlider>().Value;

                var killable =
                    ObjectCache.Get<Obj_AI_Hero>(ObjectTeam.Enemy, InAttackRange)
                        .OrderByDescending(enemy => enemy.Health / Player.GetAutoAttackDamage(enemy))
                        .FirstOrDefault(
                            enemy =>
                            !enemy.IsZombie && !Invulnerable.Check(enemy, DamageType.Physical, ignoreShields)
                            && Math.Ceiling(enemy.Health / Player.GetAutoAttackDamage(enemy)) <= maxIgnored);

                if (killable != null)
                {
                    return killable;
                }
            }

            return Variables.TargetSelector.GetTarget(
                float.MaxValue,
                DamageType.Physical,
                ignoreShields,
                Player.ServerPosition,
                ObjectCache.Get<Obj_AI_Hero>(ObjectTeam.Enemy)
                    .FindAll(
                        enemy =>
                        Menu[$"{header}_blacklist_{enemy.NetworkId}"].GetValue<MenuBool>().Value || enemy.IsZombie
                        || !InAttackRange(enemy)));
        }

        #endregion
    }
}