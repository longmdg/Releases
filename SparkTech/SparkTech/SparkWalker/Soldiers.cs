// Copyright (c) LeagueSharp 2016
// If you want to copy parts of the code, please inform the author and give appropiate credits
// File: SoldierTracker.cs
// Release date: 03.04.2016
// Author: Spark
// Contact: "wiktorsharp" on Skype

namespace SparkTech.SparkWalker
{
    using System.Collections.Generic;
    using System.Threading;

    using LeagueSharp;
    using LeagueSharp.SDK.Core.Utils;

    using SparkTech.Cache;

    /// <summary>
    ///     Tracks the soldiers if the used champion is <see cref="E:Azir" />
    ///     <para>Basic concepts taken from Synx' soldier manager, big credits to him!</para>
    /// </summary>
    public static class Soldiers
    {
        #region Constants

        /// <summary>
        ///     The attack range of the soldier
        ///     <para>This is necessary due to the AttackRange API not working correctly with Azir's soldiers</para>
        /// </summary>
        public const float BaseWarriorRange = 250f;

        #endregion

        #region Static Fields

        /// <summary>
        ///     Determines whether the soldiers are now attacking
        /// </summary>
        private static bool attacking;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="SparkWalker.Soldiers" /> class
        /// </summary>
        static Soldiers()
        {
            if (Core.ChampionName != "Azir")
            {
                All = new List<Obj_AI_Minion>(0);
                return;
            }

            All = new List<Obj_AI_Minion>(3);

            var flag = false;

            GameObject.OnCreate += (sender, args) =>
            {
                if (!flag || sender.Name.ToLower() != "azirsoldier")
                {
                    return;
                }

                flag = false;
                All.Add((Obj_AI_Minion)sender);
            };

            var source = new CancellationTokenSource();

            Obj_AI_Base.OnProcessSpellCast += (sender, args) =>
            {
                if (!sender.IsMe)
                {
                    return;
                }

                switch (args.SData.Name.ToLower())
                {
                    case "azirbasicattacksoldier":
                        source.Cancel();
                        source = new CancellationTokenSource();
                        attacking = true;
                        DelayAction.Add(
                            ObjectCache.Player.AttackDelay * 1000f - 100f + Game.Ping / 2f,
                            () => attacking = false,
                            source.Token);
                        break;
                    case "azirw":
                        flag = true;
                        break;
                }
            };

            Obj_AI_Base.OnPlayAnimation += (sender, args) =>
            {
                if (args.Animation != "Death" || sender.Name.ToLower() != "azirsoldier")
                {
                    return;
                }

                var index = All.FindIndex(soldier => soldier.NetworkId == sender.NetworkId);

                if (index >= 0)
                {
                    All.RemoveAt(index);
                }
            };
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the soldiers owned by the <see cref="E:Player" />
        /// </summary>
        public static List<Obj_AI_Minion> All { get; }

        /// <summary>
        ///     Gets a list of soldiers that are prepared to strike
        /// </summary>
        public static List<Obj_AI_Minion> Ready => attacking ? new List<Obj_AI_Minion>(0) : All.FindAll(sol => !sol.IsMoving);

        #endregion
    }
}