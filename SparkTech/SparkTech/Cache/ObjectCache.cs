#region License

// Copyright (c) LeagueSharp 2016
// If you want to copy parts of the code, please inform the author and give appropiate credits
// File: ObjectCache.cs
// Release date: 12.03.2016
// Author: Spark
// Contact: "wiktorsharp" on Skype

#endregion

namespace SparkTech.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.MoreLinq;

    using SharpDX;

    using SparkTech.Enumerations;

    /// <summary>
    ///     An alternative to the <see cref="ObjectManager" /> and <see cref="GameObjects" /> classes
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "These fields won't work with any other naming")]
    public static class ObjectCache
    {
        #region Static Fields

        /// <summary>
        ///     Saves the <see cref="E:Player" /> instance
        /// </summary>
        public static readonly Obj_AI_Hero Player;

        /// <summary>
        ///     The allied team to the <see cref="E:Player" />
        /// </summary>
        private static readonly GameObjectTeam AlliedTeam;

        /// <summary>
        ///     Contains the <see cref="FieldInfo" /> instances
        /// </summary>
        private static readonly Dictionary<string, FieldInfo> FieldData;

        /// <summary>
        ///     Contains all the <see cref="GameObject" /> instances
        /// </summary>
        private static readonly List<GameObject> GameObjectList;

        /// <summary>
        ///     Gets the list containing the jungle creeps
        /// </summary>
        private static readonly List<Obj_AI_Minion> JungleMinions;

        /// <summary>
        ///     Gets the clean list cointaining just the minions (no ghost wards, clones or any other crap just lane minions)
        /// </summary>
        private static readonly List<Obj_AI_Minion> Minions;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Minion" /> instances
        /// </summary>
        private static readonly List<Obj_AI_Minion> Obj_AI_MinionList;

        /// <summary>
        ///     Gets the list containing the unknown minion type (e.g. trundle walls)
        /// </summary>
        private static readonly List<Obj_AI_Minion> OtherMinions;

        /// <summary>
        ///     Gets the list containing the attackable objects (e.g. shaco's boxes)
        /// </summary>
        private static readonly List<Obj_AI_Minion> Pets;

        /// <summary>
        ///     The team relation dictionary
        /// </summary>
        private static readonly Dictionary<ObjectTeam, GameObjectTeam> TeamDictionary;

        /// <summary>
        ///     Gets the list containing just the wards
        /// </summary>
        private static readonly List<Obj_AI_Minion> Wards;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="ObjectCache" /> class
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
                                 {
                                     ObjectTeam.Enemy,
                                     AlliedTeam == GameObjectTeam.Order ? GameObjectTeam.Chaos : GameObjectTeam.Order
                                 },
                                 { ObjectTeam.Neutral, GameObjectTeam.Neutral },
                                 { ObjectTeam.Unknown, GameObjectTeam.Unknown }
                             };

            GameObject.OnCreate += (sender, args) => Process(sender, true);

            // I don't find OnDelete reliable when it comes to kepping the lists tidy so I'll use OnUpdate instead
            Game.OnUpdate += delegate
            {
                foreach (var o in GameObjectList.FindAll(o => o == null || !o.IsValid).DistinctBy(o => o?.NetworkId))
                {
                    Process(o, false);
                }
            };
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The <see cref="ListAction{TGameObject}" /> delegate
        /// </summary>
        /// <typeparam name="TGameObject">The <see cref="GameObject" /> to take action on</typeparam>
        /// <param name="list">The <see cref="List{T}" /> to take action on</param>
        private delegate void ListAction<TGameObject>(List<TGameObject> list) where TGameObject : GameObject;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the GameObjects of the specified type and team
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <param name="team">The specified object team</param>
        /// <param name="inrange">The function determining whether this instance is in range</param>
        /// <param name="zombies">Determines whether to obtain zombie units</param>
        /// <param name="moreChecks">
        ///     Determines whether to obtain invisible or non-targetable units as well. This is used mostly
        ///     for the buildings
        /// </param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static List<TGameObject> Get<TGameObject>(ObjectTeam team = ObjectTeam.Ally | ObjectTeam.Enemy | ObjectTeam.Neutral, Predicate<TGameObject> inrange = null, bool zombies = false, bool moreChecks = true) where TGameObject : GameObject
        {
            FieldInfo field;
            var name = typeof(TGameObject).Name + "List";

            // Found it cached
            if (FieldData.TryGetValue(name, out field))
            {
                return Selector((List<TGameObject>)field.GetValue(null), team, moreChecks, true, zombies, inrange);
            }

            field = GetField(name);
            var container = GameObjectList.OfType<GameObject, TGameObject>(o => o.IsValid);

            // ReSharper disable once InvertIf
            // This type of GameObject is supported however cache is being just initialized
            if (field != null)
            {
                field.SetValue(null, container);
                FieldData.Add(name, field);
            }

            return Selector(container, team, moreChecks, false, zombies, inrange);
        }

        /// <summary>
        ///     Gets the minions of the specified type and team
        /// </summary>
        /// <param name="type">The minion type flags</param>
        /// <param name="team">The requested team</param>
        /// <param name="inrange">The function determining whether this instance is in range</param>
        /// <returns></returns>
        public static List<Obj_AI_Minion> GetMinions(ObjectTeam team = ObjectTeam.Ally | ObjectTeam.Enemy, MinionType type = MinionType.Minion, Predicate<Obj_AI_Minion> inrange = null)
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

            return Selector(container, team, true, true, false, inrange);
        }

        /// <summary>
        ///     Gets minions in a similiar way to <see cref="E:MinionManager" />
        /// </summary>
        /// <param name="from">The from</param>
        /// <param name="range">The range to take minions from</param>
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

            var infinity = float.IsPositiveInfinity(range *= range);

            return Selector(container, team, true, true, false, @base => infinity || Vector3.DistanceSquared(@base.ServerPosition, from) <= range);
        }

        /// <summary>
        ///     Gets the <see cref="ObjectTeam" /> representation of the current object
        /// </summary>
        /// <param name="object">The <see cref="GameObject" /> to be inspected</param>
        /// <param name="check">Determines whether to check the instance for validity</param>
        /// <returns></returns>
        public static ObjectTeam Team(this GameObject @object, bool check = false)
        {
            if (check && (@object == null || !@object.IsValid))
                return ObjectTeam.Unknown;
            var team = @object.Team;
            return TeamDictionary.Single(pair => pair.Value == team).Key;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the executor function
        /// </summary>
        /// <typeparam name="TItem">The <see cref="GameObject" /> type to take action on</typeparam>
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

        /// <summary>
        ///     Gets the specified field
        /// </summary>
        /// <param name="fieldName">The name of the searched field</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static FieldInfo GetField(string fieldName)
        {
            return typeof(ObjectCache).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        ///     Adds or removes a <see cref="GameObject" /> from the appropiate lists
        /// </summary>
        /// <param name="object">The <see cref="GameObject" /> to be processed</param>
        /// <param name="new">Determines whether to add or remove an item</param>
        private static void Process(GameObject @object, bool @new)
        {
            GetExecutor(@object, @new)(GameObjectList);
            GetExecutor(@object as Obj_GeneralParticleEmitter, @new)(Obj_GeneralParticleEmitterList);

            var attackable = @object as AttackableUnit;
            if (attackable == null)
            {
                return;
            }

            GetExecutor(attackable, @new)(AttackableUnitList);
            GetExecutor(attackable as Obj_HQ, @new)(Obj_HQList);
            GetExecutor(attackable as Obj_BarracksDampener, @new)(Obj_BarracksDampenerList);

            var @base = attackable as Obj_AI_Base;
            if (@base == null)
            {
                return;
            }

            GetExecutor(@base, @new)(Obj_AI_BaseList);
            GetExecutor(@base as Obj_AI_Hero, @new)(Obj_AI_HeroList);
            GetExecutor(@base as Obj_AI_Turret, @new)(Obj_AI_TurretList);

            var minion = attackable as Obj_AI_Minion;
            if (minion == null)
            {
                return;
            }

            GetExecutor(minion, @new)(Obj_AI_MinionList);

            if (@new)
            {
                var type = minion.GetMinionType();

                GetExecutor(minion, true)(
                    !type.HasFlag(MinionTypes.Melee) && !type.HasFlag(MinionTypes.Ranged)
                        ? !type.HasFlag(MinionTypes.Ward)
                              ? minion.GetJungleType() == JungleType.Unknown
                                    ? !minion.IsPet() ? OtherMinions : Pets
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

        /// <summary>
        ///     Returns a matched list
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject" /> type</typeparam>
        /// <param name="container">The original list</param>
        /// <param name="flags">The provided team flags</param>
        /// <param name="moreChecks">Determines whether to perform additional checks</param>
        /// <param name="check">The additional predicate</param>
        /// <param name="inrange">The function to check if the unit is in range</param>
        /// <param name="zombies">Determines whether to obtain zombie units</param>
        /// <returns></returns>
        private static List<TGameObject> Selector<TGameObject>(List<TGameObject> container, ObjectTeam flags, bool moreChecks, bool check, bool zombies, Predicate<TGameObject> inrange) where TGameObject : GameObject
        {
            var teams = (from pair in TeamDictionary where flags.HasFlag(pair.Key) select pair.Value).ToList();

            container = container.FindAll(o =>
                {
                    if (check && (o == null || !o.IsValid))
                    {
                        return false;
                    }

                    var team = o.Team;

                    if (!teams.Contains(team) || inrange != null && !inrange(o))
                    {
                        return false;
                    }

                    if (!moreChecks)
                    {
                        return true;
                    }

                    if (!o.IsVisible || o.IsDead)
                    {
                        return false;
                    }

                    var attackable = o as AttackableUnit;

                    if (attackable == null)
                    {
                        return true;
                    }

                    if (!zombies && attackable.IsZombie)
                    {
                        return false;
                    }

                    if (attackable.IsInvulnerable || (team != AlliedTeam && !attackable.IsTargetable))
                    {
                        return false;
                    }

                    var unit = o as Obj_AI_Base;

                    return unit == null || unit.HealthPercent > 10f || !unit.HasBuff("kindredrnodeathbuff");
                });

            container.TrimExcess();
            return container;
        }

        #endregion

#pragma warning disable 649

        /// <summary>
        ///     Contains all the <see cref="AttackableUnit" /> instances
        /// </summary>
        private static List<AttackableUnit> AttackableUnitList;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Base" /> instances
        /// </summary>
        private static List<Obj_AI_Base> Obj_AI_BaseList;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Turret" /> instances
        /// </summary>
        private static List<Obj_AI_Turret> Obj_AI_TurretList;

        /// <summary>
        ///     Contains all the <see cref="Obj_HQ" /> instances
        /// </summary>
        private static List<Obj_HQ> Obj_HQList;

        /// <summary>
        ///     Contains all the <see cref="Obj_BarracksDampener" /> instances
        /// </summary>
        private static List<Obj_BarracksDampener> Obj_BarracksDampenerList;

        /// <summary>
        ///     Contains all the <see cref="Obj_GeneralParticleEmitter" /> instances
        /// </summary>
        private static List<Obj_GeneralParticleEmitter> Obj_GeneralParticleEmitterList;

        /// <summary>
        ///     Contains all the <see cref="Obj_AI_Hero" /> instances
        /// </summary>
        private static List<Obj_AI_Hero> Obj_AI_HeroList;

#pragma warning restore 649
    }
}