namespace SparkTech.Executors
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SparkTech.Base;
    using SparkTech.Enumerations;
    using SparkTech.EventData;

    using Orbwalker = SparkTech.Enumerations.Orbwalker;

    /// <summary>
    /// The <see cref="Hero{TSpellEnumeration}"/> class
    /// </summary>
    /// <typeparam name="TSpellEnumeration">Your spell enum</typeparam>
    public abstract class Hero<TSpellEnumeration> : HeroBase where TSpellEnumeration : struct, IConvertible
    {
        /// <summary>
        /// The <see cref="E:CoreGameUpdate"/> method fired when game engine updates
        /// </summary>
        /// <param name="args">The <see cref="GameUpdateEventArgs"/> instance</param>
        protected internal override void CoreGameUpdate(GameUpdateEventArgs args)
        {
            Slacking = args.OrbwalkingMode == Mode.None;

            base.CoreGameUpdate(args);

            try
            {
                Automated(args);
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            try
            {
                if (KillSteal())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            try
            {
                switch (args.OrbwalkingMode)
                {
                    case Mode.Combo:
                        Combo();
                        break;
                    case Mode.LaneClear:
                        LaneClear();
                        break;
                    case Mode.Harass:
                        Harass();
                        break;
                    case Mode.LastHit:
                        LastHit();
                        break;
                    case Mode.Freeze:
                        Freeze();
                        break;
                    case Mode.Flee:
                        // SDK has no flee rite?
                        if (args.Orbwalker == Orbwalker.SDKOrbwalker)
                        {
                            Variables.Orbwalker.Move(Game.CursorPos);
                        }
                        Flee();
                        break;
                    case Mode.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(args.OrbwalkingMode), args.OrbwalkingMode, null);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="E:Player"/> is slacking
        /// </summary>
        protected bool Slacking { get; private set; }

        /// <summary>
        /// Determines whether <see cref="E:KillSteal"/> was executed
        /// </summary>
        protected virtual bool KillSteal() => false;

        /// <summary>
        /// The dictionary containing <see cref="SparkSpell"/> instances
        /// </summary>
        protected readonly Dictionary<TSpellEnumeration, SparkSpell> Spells = new Dictionary<TSpellEnumeration, SparkSpell>(0xf);

        /// <summary>
        /// The function executing on <see cref="E:Core.GameUpdate"/>
        /// </summary>
        /// <param name="args">The event data</param>
        protected virtual void Automated(GameUpdateEventArgs args)
        { }

        /// <summary>
        /// The <see cref="E:Player"/> is pressing spacebar HARD
        /// <para>Show these motherf#$%ers how much you can wreck them here!</para>
        /// </summary>
        protected virtual void Combo()
        { }

        /// <summary>
        /// The <see cref="E:Player"/> is harassing
        /// </summary>
        protected virtual void Harass()
        { }

        /// <summary>
        /// The <see cref="E:Player"/> clears the lane
        /// </summary>
        protected virtual void LaneClear()
        { }

        /// <summary>
        /// The <see cref="E:Player"/> LastHits
        /// </summary>
        protected virtual void LastHit()
        { }

        /// <summary>
        /// The <see cref="E:Player"/> freezes the lane
        /// </summary>
        protected virtual void Freeze()
        { }

        /// <summary>
        /// The <see cref="E:Player"/> flees
        /// </summary>
        protected virtual void Flee()
        { }
    }
}