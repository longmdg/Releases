/*
namespace SparkTech.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SharpDX;

    public static class Indicator
    {
        private static readonly Vector2 Offset = new Vector2(1f, -5.8f);

        private const int Width = 104;

        private const int Height = 9;

        public static readonly List<UnitDamage> Instances = new List<UnitDamage>(12);

        public static bool Enabled { get; set; }

        static Indicator()
        {
            Drawing.OnDraw += delegate
                {
                    foreach (var instance in Instances.Where(instance => instance.Enabled && instance.Unit.IsHPBarRendered && instance.Unit.Position.IsOnScreen() && instance.Unit.Position.IsOnScreen()))
                    {
                        var position = Offset + new Vector2(instance.Unit.HPBarPosition.X + instance.Unit.HPBarXOffset, instance.Unit.HPBarPosition.Y + instance.Unit.HPBarYOffset);
                        var postDmg = instance.Unit.Health - instance.Damage(instance.Unit);
                        var percentDmg = postDmg > 0f ? postDmg : 0f / instance.Unit.MaxHealth;

                    }
                };
        }

        public class UnitDamage
        {
            public readonly Obj_AI_Base Unit;

            public Color Color = Color.Red;

            private Func<Obj_AI_Base, float> damage = unit => 0f;

            public Func<Obj_AI_Base, float> Damage
            {
                get
                {
                    return damage ?? (unit => 0f);
                }
                set
                {
                    damage = value;
                }
            }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public bool Enabled { get; set; } = true;

            public UnitDamage(Obj_AI_Base unit)
            {
                Unit = unit;
            }
        }
    }
}*/