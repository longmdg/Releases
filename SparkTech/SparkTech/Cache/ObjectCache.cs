namespace SparkTech.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp;

    // For recognising the minion types
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    using SparkTech.Enumerations;

    /// <summary>
    /// An alternative to the <see cref="ObjectManager"/> and <see cref="GameObjects"/> classes
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "These fields won't work with any other naming")]
    public static class ObjectCache
    {
        #region Public Methods

        /// <summary>
        /// Saves the <see cref="E:Player"/> instance
        /// </summary>
        public static readonly Obj_AI_Hero Player;

        /// <summary>
        /// Gets the GameObjects of the specified type and team
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject"/> type</typeparam>
        /// <param name="team">The specified object team</param>
        /// <param name="range">The range to take objects from</param>
        /// <param name="moreChecks">Determines whether to obtain invisible or non-targetable units as well</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static List<TGameObject> Get<TGameObject>(ObjectTeam team = ObjectTeam.Ally | ObjectTeam.Enemy | ObjectTeam.Neutral, float range = float.MaxValue, bool moreChecks = true) where TGameObject : GameObject
        {
            Predicate<TGameObject> inrange = o => true;
            FieldInfo field;
            var name = typeof(TGameObject).Name + "List";

            if (!float.IsPositiveInfinity(range *= range))
            {
                var from = Player.ServerPosition;
                inrange = o => Vector3.DistanceSquared((o as Obj_AI_Base)?.ServerPosition ?? o.Position, from) <= range;
            }

            if (FieldData.TryGetValue(name, out field))
            {
                return Selector((List<TGameObject>)field.GetValue(null), team, moreChecks, o => o != null && o.IsValid && inrange(o));
            }

            field = GetField(name);
            var container = GameObjectList.OfType<GameObject, TGameObject>().FindAll(o => o.IsValid);

            // ReSharper disable once InvertIf
            if (field != null)
            {
                field.SetValue(null, container);
                FieldData.Add(name, field);
            }

            return Selector(container, team, moreChecks, inrange);
        }

        /// <summary>
        /// Gets the minions of the specified type and team
        /// </summary>
        /// <param name="type">The minion type flags</param>
        /// <param name="team">The requested team</param>
        /// <param name="range">The range</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetMinions(ObjectTeam team = ObjectTeam.Ally | ObjectTeam.Enemy, MinionType type = MinionType.Minion, float range = float.MaxValue)
        {
            var container = new List<Obj_AI_Minion>(Obj_AI_MinionList.Count);

            if (type.HasFlag(MinionType.Minion))
                container.AddRange(Minions);
            if (type.HasFlag(MinionType.Ward))
                container.AddRange(Wards);
            if (type.HasFlag(MinionType.Pet))
                container.AddRange(Pets);
            if (type.HasFlag(MinionType.Jungle))
                container.AddRange(JungleMinions);
            if (type.HasFlag(MinionType.Other))
                container.AddRange(OtherMinions);

            Predicate<Obj_AI_Base> match;

            if (!float.IsPositiveInfinity(range *= range))
            {
                var from = Player.ServerPosition;
                match = o => o != null && o.IsValid && Vector3.DistanceSquared(from, o.ServerPosition) <= range;
            }
            else
            {
                match = o => o != null && o.IsValid;
            }

            return Selector(container, team, true, match);
        }

        /// <summary>
        /// Gets minions in a similiar way to <see cref="E:MinionManager"/>
        /// </summary>
        /// <param name="from">The from</param>
        /// <param name="range">The range</param>
        /// <param name="team">The team</param>
        /// <param name="type">The minion type</param>
        /// <returns></returns>
        public static List<Obj_AI_Base> GetMinions(Vector3 from, float range, ObjectTeam team = ObjectTeam.Enemy | ObjectTeam.Ally, MinionType type = MinionType.Minion)
        {
            var container = new List<Obj_AI_Base>(Obj_AI_MinionList.Count);

            if (type.HasFlag(MinionType.Minion))
                container.AddRange(Minions);
            if (type.HasFlag(MinionType.Ward))
                container.AddRange(Wards);
            if (type.HasFlag(MinionType.Pet))
                container.AddRange(Pets);
            if (type.HasFlag(MinionType.Jungle))
                container.AddRange(JungleMinions);
            if (type.HasFlag(MinionType.Other))
                container.AddRange(OtherMinions);

            if (from.IsZero)
                from = Player.ServerPosition;

            Predicate<Obj_AI_Base> match;

            if (!float.IsPositiveInfinity(range *= range))
            {
                match = o => o != null && o.IsValid && Vector3.DistanceSquared(from, o.ServerPosition) <= range;
            }
            else
            {
                match = o => o != null && o.IsValid;
            }

            return Selector(container, team, true, match);
        }

        /// <summary>
        /// Gets the <see cref="ObjectTeam"/> representation of the current object
        /// </summary>
        /// <param name="object">The <see cref="GameObject"/> to be inspected</param>
        /// <returns></returns>
        public static ObjectTeam Team(this GameObject @object)
        {
            var team = @object.Team;
            return TeamDictionary.Single(pair => pair.Value == team).Key;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a matched list
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject"/> type</typeparam>
        /// <param name="container">THe original list</param>
        /// <param name="team">The provided team flags</param>
        /// <param name="moreChecks">Determines whether to perform additional checks</param>
        /// <param name="check">The additional predicate</param>
        /// <returns></returns>
        private static List<TGameObject> Selector<TGameObject>(List<TGameObject> container, ObjectTeam team, bool moreChecks, Predicate<TGameObject> check) where TGameObject : GameObject
        {
            var teams = TeamDictionary.Where(pair => team.HasFlag(pair.Key)).Select(pair => pair.Value).ToList();

            container.RemoveAll(o =>
            {
                if (check != null && !check(o))
                {
                    return true;
                }

                var side = o.Team;

                if (!teams.Contains(side))
                {
                    return true;
                }

                if (!moreChecks)
                {
                    return false;
                }

                if (!o.IsVisible || o.IsDead)
                {
                    return true;
                }

                var attackable = o as AttackableUnit;

                if (attackable == null)
                {
                    return false;
                }

                if (attackable.IsInvulnerable || side != AlliedTeam && !attackable.IsTargetable)
                {
                    return true;
                }

                var unit = attackable as Obj_AI_Base;

                return unit != null && unit.HealthPercent <= 10 && unit.HasBuff("kindredrnodeathbuff");
            });

            container.TrimExcess();
            return container;
        }

        /// <summary>
        /// Gets the specified field
        /// </summary>
        /// <param name="fieldName">The name of the searched field</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static FieldInfo GetField(string fieldName)
        {
            return typeof(ObjectCache).GetField(fieldName, BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// The allied team to the <see cref="E:Player"/>
        /// </summary>
        private static readonly GameObjectTeam AlliedTeam;

        /// <summary>
        /// Initializes static members of the <see cref="ObjectCache"/> class
        /// </summary>
        static ObjectCache()
        {
            GameObjectList = ObjectManager.Get<GameObject>().ToList();
            Obj_AI_MinionList = GameObjectList.OfType<Obj_AI_Minion>().ToList();

            FieldData = new Dictionary<string, FieldInfo>
                        {
                            { "GameObjectList", GetField("GameObjectList") },
                            { "Obj_AI_MinionList", GetField("Obj_AI_MinionList") }
                        };

            Minions = Obj_AI_MinionList.FindAll(minion =>
            {
                var type = minion.GetMinionType();
                return type.HasFlag(MinionTypes.Melee) || type.HasFlag(MinionTypes.Ranged);
            });
            Pets = Obj_AI_MinionList.FindAll(minion => minion.IsPet());
            Wards = Obj_AI_MinionList.FindAll(minion => minion.GetMinionType().HasFlag(MinionTypes.Ward));
            JungleMinions = Obj_AI_MinionList.FindAll(minion => minion.GetJungleType() != JungleType.Unknown);
            OtherMinions = Obj_AI_MinionList.FindAll(o =>
                    {
                        var type = o.GetMinionType();
                        return !type.HasFlag(MinionTypes.Melee) && !type.HasFlag(MinionTypes.Ranged) && !type.HasFlag(MinionTypes.Ward) && o.GetJungleType() == JungleType.Unknown && !o.IsPet();
                    });

            Player = (Obj_AI_Hero)GameObjectList.Single(o => o.IsMe);

            AlliedTeam = Player.Team;

            TeamDictionary = new Dictionary<ObjectTeam, GameObjectTeam>(Enum.GetNames(typeof(ObjectTeam)).Length)
                             {
                                 { ObjectTeam.Ally, AlliedTeam },
                                 { ObjectTeam.Enemy, AlliedTeam == GameObjectTeam.Order ? GameObjectTeam.Chaos : GameObjectTeam.Order },
                                 { ObjectTeam.Neutral, GameObjectTeam.Neutral },
                                 { ObjectTeam.Unknown, GameObjectTeam.Unknown }
                             };

            GameObject.OnCreate += (sender, args) => Process(sender, true);

            Game.OnUpdate += delegate
            {
                var item = GameObjectList.Find(o => o == null || !o.IsValid);

                if (item != null)
                {
                    Process(item, false);
                }
            };
        }

        /// <summary>
        /// Adds or removes a <see cref="GameObject"/> from the appropiate lists
        /// </summary>
        /// <param name="object">The <see cref="GameObject"/> to be processed</param>
        /// <param name="new">Determines whether to add or remove an item</param>
        private static void Process(GameObject @object, bool @new)
        {
            GetExecutor(@object, @new)(GameObjectList);
            GetExecutor(@object as Obj_GeneralParticleEmitter, @new)(Obj_GeneralParticleEmitterList);

            var attackable = @object as AttackableUnit;
            if (attackable == null) return;

            GetExecutor(attackable, @new)(AttackableUnitList);
            GetExecutor(attackable as Obj_HQ, @new)(Obj_HQList);
            GetExecutor(attackable as Obj_BarracksDampener, @new)(Obj_BarracksDampenerList);

            var @base = attackable as Obj_AI_Base;
            if (@base == null) return;

            GetExecutor(@base, @new)(Obj_AI_BaseList);
            GetExecutor(@base as Obj_AI_Hero, @new)(Obj_AI_HeroList);
            GetExecutor(@base as Obj_AI_Turret, @new)(Obj_AI_TurretList);

            var minion = attackable as Obj_AI_Minion;
            if (minion == null) return;

            GetExecutor(minion, @new)(Obj_AI_MinionList);

            if (@new)
            {
                var type = minion.GetMinionType();

                GetExecutor(minion, true)(
                    !type.HasFlag(MinionTypes.Melee) && !type.HasFlag(MinionTypes.Ranged)
                        ? !type.HasFlag(MinionTypes.Ward)
                              ? minion.GetJungleType() == JungleType.Unknown
                                    ? !minion.IsPet()
                                        ? OtherMinions
                                        : Pets
                                    : JungleMinions
                              : Wards
                        : Minions);
            }
            else
            {
                GetExecutor(minion, false)(Minions);
                GetExecutor(minion, false)(Wards);
                GetExecutor(minion, false)(Pets);
                GetExecutor(minion, false)(OtherMinions);
                GetExecutor(minion, false)(JungleMinions);
            }
        }

        #endregion

        #region Containers

        /// <summary>
        /// The team relation dictionary
        /// </summary>
        private static readonly Dictionary<ObjectTeam, GameObjectTeam> TeamDictionary;

        /// <summary>
        /// Contains the <see cref="FieldInfo"/> instances
        /// </summary>
        private static readonly Dictionary<string, FieldInfo> FieldData;

        /// <summary>
        /// Contains all the <see cref="GameObject"/> instances
        /// </summary>
        private static readonly List<GameObject> GameObjectList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Minion"/> instances
        /// </summary>
        private static readonly List<Obj_AI_Minion> Obj_AI_MinionList;

        /// <summary>
        /// Gets the clean list cointaining just the minions (no ghost wards, clones or any other crap just lane minions)
        /// </summary>
        private static readonly List<Obj_AI_Minion> Minions;

        /// <summary>
        /// Gets the list containing just the wards
        /// </summary>
        private static readonly List<Obj_AI_Minion> Wards;

        /// <summary>
        /// Gets the list containing the attackable objects (e.g. shaco's boxes)
        /// </summary>
        private static readonly List<Obj_AI_Minion> Pets;

        /// <summary>
        /// Gets the list containing the jungle creeps
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleMinions;

        /// <summary>
        /// Gets the list containing the unknown minion type (e.g. trundle walls)
        /// </summary>
        private static readonly List<Obj_AI_Minion> OtherMinions;

#pragma warning disable 649

        /// <summary>
        /// Contains all the <see cref="AttackableUnit"/> instances
        /// </summary>
        private static List<AttackableUnit> AttackableUnitList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Base"/> instances
        /// </summary>
        private static List<Obj_AI_Base> Obj_AI_BaseList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Turret"/> instances
        /// </summary>
        private static List<Obj_AI_Turret> Obj_AI_TurretList;

        /// <summary>
        /// Contains all the <see cref="Obj_HQ"/> instances
        /// </summary>
        private static List<Obj_HQ> Obj_HQList;

        /// <summary>
        /// Contains all the <see cref="Obj_BarracksDampener"/> instances
        /// </summary>
        private static List<Obj_BarracksDampener> Obj_BarracksDampenerList;

        /// <summary>
        /// Contains all the <see cref="Obj_GeneralParticleEmitter"/> instances
        /// </summary>
        private static List<Obj_GeneralParticleEmitter> Obj_GeneralParticleEmitterList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Hero"/> instances
        /// </summary>
        private static List<Obj_AI_Hero> Obj_AI_HeroList;

#pragma warning restore 649

        #endregion

        #region Handlers

        /// <summary>
        /// The <see cref="ListAction{TGameObject}"/> delegate
        /// </summary>
        /// <typeparam name="TGameObject">The <see cref="GameObject"/> to take action on</typeparam>
        /// <param name="list">The <see cref="List{T}"/> to take action on</param>
        private delegate void ListAction<TGameObject>(List<TGameObject> list) where TGameObject : GameObject;

        /// <summary>
        /// Gets the executor function
        /// </summary>
        /// <typeparam name="TItem">The <see cref="GameObject"/> type to take action on</typeparam>
        /// <param name="object">The sender</param>
        /// <param name="new">Determines whether to add or remove item</param>
        /// <returns></returns>
        private static ListAction<TItem> GetExecutor<TItem>(TItem @object, bool @new) where TItem : GameObject
        {
            if (@new)
            {
                return list => list?.Add(@object);
            }

            return list => list?.RemoveAll(o => o == null || !o.IsValid);
        }

        #endregion
    }
}