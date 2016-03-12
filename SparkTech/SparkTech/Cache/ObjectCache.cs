namespace SparkTech.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;

    using ObjectTeam = SparkTech.Enumerations.ObjectTeam;

    /// <summary>
    /// An alternative to the <see cref="ObjectManager"/> and <see cref="GameObjects"/> classes
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "These fields won't work with any other naming")]
    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "These fields are actually being accessed by reflection")]
    public static class ObjectCache
    {
        #region Public Methods And Fields

        /// <summary>
        /// Saves the <see cref="E:Player"/> instance
        /// </summary>
        public static readonly Obj_AI_Hero Player;

        /// <summary>
        /// Gets the clean list cointaining just the enemy minions (no ghost wards, clones or any other crap just lane minions)
        /// </summary>
        public static readonly List<Obj_AI_Minion> EnemyMinions;

        /// <summary>
        /// Gets the list containing just the wards
        /// </summary>
        public static readonly List<Obj_AI_Minion> Wards;

        /// <summary>
        /// Gets the list containing the attackable objects (e.g. shaco's boxes)
        /// </summary>
        public static readonly List<Obj_AI_Minion> AttackableObjects;

        /// <summary>
        /// Gets the GameObjects of the specified type and team
        /// </summary>
        /// <typeparam name="TGameObject">The type param specifying which <see cref="GameObject"/> type to obtain</typeparam>
        /// <param name="team">The specified object team</param>
        /// <returns></returns>
        public static List<TGameObject> Get<TGameObject>(ObjectTeam team = ObjectTeam.Any) where TGameObject : GameObject
        {
            // If the left side is null, the tpye param not supported by the cache
            var container = GetContainer<TGameObject>() ?? GameObjectList.OfType<TGameObject>().ToList();
            var predicate = GetPredicate<TGameObject>(team);

            return container.FindAll(predicate);
        }

        #endregion

        #region Delegates And Handlers

        /// <summary>
        /// The <see cref="ListAction{TGameObject}"/> delegate
        /// </summary>
        /// <typeparam name="TGameObject">The output <see cref="GameObject"/> type</typeparam>
        /// <param name="list">The <see cref="List{T}"/> to take action on</param>
        private delegate void ListAction<TGameObject>(List<TGameObject> list) where TGameObject : GameObject;

        /// <summary>
        /// Gets the executor function
        /// </summary>
        /// <typeparam name="TGameObject">The output <see cref="GameObject"/> type</typeparam>
        /// <param name="object">The sender</param>
        /// <param name="new">Determines whether to add or remove item</param>
        /// <returns></returns>
        private static ListAction<TGameObject> GetExecutor<TGameObject>(TGameObject @object, bool @new) where TGameObject : GameObject
        {
            if (@object == null)
            {
                return list => { };
            }

            if (@new)
            {
                return list => list.Add(@object);
            }

            return list => list.Remove(@object);
        } 
        
        #endregion

        #region Containers

        /// <summary>
        /// Contains all the <see cref="GameObject"/> instances
        /// </summary>
        private static readonly List<GameObject> GameObjectList;

        /// <summary>
        /// Contains all the <see cref="AttackableUnit"/> instances
        /// </summary>
        private static readonly List<AttackableUnit> AttackableUnitList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Base"/> instances
        /// </summary>
        private static readonly List<Obj_AI_Base> Obj_AI_BaseList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Turret"/> instances
        /// </summary>
        private static readonly List<Obj_AI_Turret> Obj_AI_TurretList;

        /// <summary>
        /// Contains all the <see cref="Obj_HQ"/> instances
        /// </summary>
        private static readonly List<Obj_HQ> Obj_HQList;

        /// <summary>
        /// Contains all the <see cref="Obj_BarracksDampener"/> instances
        /// </summary>
        private static readonly List<Obj_BarracksDampener> Obj_BarracksDampenerList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Minion"/> instances
        /// </summary>
        private static readonly List<Obj_AI_Minion> Obj_AI_MinionList;

        /// <summary>
        /// Contains all the <see cref="Obj_AI_Hero"/> instances
        /// </summary>
        private static readonly List<Obj_AI_Hero> Obj_AI_HeroList;

        #endregion

        /// <summary>
        /// Gets the team predicate
        /// </summary>
        /// <typeparam name="TGameObject">The output <see cref="GameObject"/> type</typeparam>
        /// <param name="team">The specified team</param>
        /// <returns></returns>
        private static Predicate<TGameObject> GetPredicate<TGameObject>(ObjectTeam team) where TGameObject : GameObject
        {
            switch (team)
            {
                case ObjectTeam.Any:
                    return obj => true;
                case ObjectTeam.Enemy:
                    return obj => obj.IsEnemy;
                case ObjectTeam.Ally:
                    return obj => obj.IsAlly;
                case ObjectTeam.Neutral:
                    return obj => obj.Team == GameObjectTeam.Neutral;
                case ObjectTeam.NotAlly:
                    return obj => !obj.IsAlly;
                case ObjectTeam.NotAllyForEnemy:
                    return obj => !obj.IsEnemy;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
            }
        }

        /// <summary>
        /// Gets the <see cref="List{T}"/> container for the specified type parameter
        /// </summary>
        /// <typeparam name="TGameObject">The output <see cref="GameObject"/> type</typeparam>
        /// <returns></returns>
        private static List<TGameObject> GetContainer<TGameObject>() where TGameObject : GameObject
        {
            var field = typeof(ObjectCache).GetField($"{typeof(TGameObject).Name}List");

            return (List<TGameObject>)field?.GetValue(null);
        }

        /// <summary>
        /// The get operation from the native GameObjects stack.
        /// </summary>
        /// <typeparam name="TGameObject">The requested <see cref="GameObject"/> type.</typeparam>
        /// <returns>The <see cref="List{T}"/> containing the requested type.</returns>
        private static List<TGameObject> GetNative<TGameObject>() where TGameObject : GameObject, new() => ObjectManager.Get<TGameObject>().ToList();

        /// <summary>
        /// Initializes static members of the <see cref="ObjectCache"/> class
        /// </summary>
        static ObjectCache()
        {
            GameObjectList = GetNative<GameObject>();
            AttackableUnitList = GetNative<AttackableUnit>();
            Obj_AI_BaseList = GetNative<Obj_AI_Base>();
            Obj_AI_TurretList = GetNative<Obj_AI_Turret>();
            Obj_HQList = GetNative<Obj_HQ>();
            Obj_BarracksDampenerList = GetNative<Obj_BarracksDampener>();
            Obj_AI_MinionList = GetNative<Obj_AI_Minion>();
            Obj_AI_HeroList = GetNative<Obj_AI_Hero>();

            EnemyMinions = Obj_AI_MinionList.FindAll(minion => minion.IsEnemy && minion.IsMinion());
            AttackableObjects = Obj_AI_MinionList.FindAll(minion => minion.IsPet());
            Wards = Obj_AI_MinionList.FindAll(minion => minion.GetMinionType().HasFlag(MinionTypes.Ward));

            Player = (Obj_AI_Hero)GameObjectList.Single(obj => obj.IsMe);

            GameObject.OnCreate += (sender, args) => Process(sender, true);
            GameObject.OnDelete += (sender, args) => Process(sender, false);
        }

        /// <summary>
        /// Processes a <see cref="GameObject"/>
        /// </summary>
        /// <param name="object">The <see cref="GameObject"/> to be processed</param>
        /// <param name="new">Determines whether to add an item</param>
        private static void Process(GameObject @object, bool @new)
        {
            GetExecutor(@object, @new)(GameObjectList);
            GetExecutor(@object as AttackableUnit, @new)(AttackableUnitList);
            GetExecutor(@object as Obj_AI_Base, @new)(Obj_AI_BaseList);
            GetExecutor(@object as Obj_AI_Hero, @new)(Obj_AI_HeroList);
            GetExecutor(@object as Obj_AI_Turret, @new)(Obj_AI_TurretList);
            GetExecutor(@object as Obj_HQ, @new)(Obj_HQList);
            GetExecutor(@object as Obj_BarracksDampener, @new)(Obj_BarracksDampenerList);

            var minion = @object as Obj_AI_Minion;

            if (minion == null)
            {
                return;
            }

            GetExecutor(minion, @new)(Obj_AI_MinionList);

            var flags = minion.GetMinionType();

            if (flags.HasFlag(MinionTypes.Melee) || flags.HasFlag(MinionTypes.Ranged))
            {
                if (minion.IsEnemy)
                {
                    GetExecutor(minion, @new)(EnemyMinions);
                }
            }
            else if (flags.HasFlag(MinionTypes.Ward))
            {
                GetExecutor(minion, @new)(Wards);
            }
            else if (minion.IsPet())
            {
                GetExecutor(minion, @new)(AttackableObjects);
            }
        }
    }
}