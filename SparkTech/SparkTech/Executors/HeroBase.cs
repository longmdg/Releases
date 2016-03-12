namespace SparkTech.Executors
{
    using System;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SparkTech.EventData;
    using SparkTech.Helpers;

    using Orbwalker = SparkTech.Enumerations.Orbwalker;

    /// <summary>
    /// The <see cref="HeroBase"/> class
    /// </summary>
    public abstract class HeroBase
    {
        /// <summary>
        /// The current orbwalker
        /// </summary>
        protected internal Orbwalker CurrentOrbwalker;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeroBase"/> class
        /// </summary>
        protected HeroBase()
        {
            Core.GameUpdate += CoreGameUpdate;

            Variables.Orbwalker.OnAction += (sender, args) =>
                {
                    if (CurrentOrbwalker != Orbwalker.SDKOrbwalker)
                    {
                        return;
                    }

                    try
                    {
                        switch (args.Type)
                        {
                            case OrbwalkingType.AfterAttack:
                                OnAfterAttack(new AfterPlayerAttackEventArgs(args.Target));
                                break;
                            case OrbwalkingType.None:
                                break;
                            case OrbwalkingType.Movement:
                                break;
                            case OrbwalkingType.StopMovement:
                                break;
                            case OrbwalkingType.BeforeAttack:
                                OnBeforeUnhumanAttack(new BeforePlayerUnhumanAttackEventArgs(args.Target));
                                break;
                            case OrbwalkingType.OnAttack:
                                OnAttack(new PlayerAttackEventArgs(args.Target));
                                break;
                            case OrbwalkingType.NonKillableMinion:
                                var minion = args.Target as Obj_AI_Minion;
                                if (minion != null)
                                {
                                    OnUnKillableMinion(new UnkillableMinionsEventArgs(new[] { minion }));
                                }
                                break;
                            case OrbwalkingType.TargetSwitch:
                                OnTargetSwitch(new PlayerTargetSwitchEventArgs(null, args.Target));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(args.Type), args.Type, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Catch();
                    }
                };
        }

        /// <summary>
        /// The current <see cref="E:Player"/> instance
        /// </summary>
        protected static Obj_AI_Hero Player => GameObjects.Player;

        /// <summary>
        /// The <see cref="E:CoreGameUpdate"/> method fired when game engine updates
        /// </summary>
        /// <param name="args">The <see cref="GameUpdateEventArgs"/> instance</param>
        protected internal virtual void CoreGameUpdate(GameUpdateEventArgs args) => CurrentOrbwalker = args.Orbwalker;

        /// <summary>
        /// Fired after the attack
        /// </summary>
        /// <param name="args">The event data</param>
        protected virtual void OnAfterAttack(AfterPlayerAttackEventArgs args)
        { }

        /// <summary>
        /// Fired before the attack is fired, useful for cancelling it
        /// </summary>
        /// <param name="args">The event data</param>
        protected virtual void OnBeforeUnhumanAttack(BeforePlayerUnhumanAttackEventArgs args)
        { }

        /// <summary>
        /// Fired when the targets switch
        /// </summary>
        /// <param name="args">The event data</param>
        protected virtual void OnTargetSwitch(PlayerTargetSwitchEventArgs args)
        { }

        /// <summary>
        /// Fired when there's an unkillable minion
        /// </summary>
        /// <param name="args">The event data</param>
        protected virtual void OnUnKillableMinion(UnkillableMinionsEventArgs args)
        { }

        /// <summary>
        /// Fired when the <see cref="E:Player"/> attacks
        /// </summary>
        /// <param name="args">The event data</param>
        protected virtual void OnAttack(PlayerAttackEventArgs args)
        { }
    }
}