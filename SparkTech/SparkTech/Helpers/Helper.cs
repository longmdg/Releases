// ReSharper disable InconsistentNaming
namespace SparkTech.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;

    /// <summary>
    /// Provides miscallenous methods and extensions
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Determined whether the game is funning in FullHD
        /// </summary>
        /// <returns></returns>
        public static bool FullHD()
        {
            return Drawing.Height == 1920 && Drawing.Width == 1080;
        }

        /// <summary>
        /// The array containing AD Carry names
        /// </summary>
        private static readonly string[] ADCs =
        {
            "Caitlyn", "Corki", "Draven", "Ashe", "Ezreal", "Graves", "Jinx",
            "Kalista", "Kog'Maw", "Lucian", "Miss Fortune", "Quinn", "Sivir",
            "Teemo", "Tristana", "Twitch", "Varus", "Vayne"
        };

        /// <summary>
        /// Determines whether this <see cref="Obj_AI_Hero"/> instance is an AD Carry
        /// </summary>
        /// <param name="hero"><see cref="Obj_AI_Hero"/> instance</param>
        /// <returns></returns>
        public static bool IsADC(this Obj_AI_Hero hero)
        {
            return ADCs.Contains(hero.ChampionName());
        }

        /// <summary>
        /// Retrieves an erray of all the enum constants
        /// </summary>
        /// <typeparam name="TEnum">THe specified enumaration</typeparam>
        /// <returns></returns>
        public static TEnum[] GetEnumValues<TEnum>()
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        /// <summary>
        /// Returns the first element that matches the predicate or null if none was found
        /// </summary>
        /// <typeparam name="T">The source type</typeparam>
        /// <param name="source">The input <see cref="IEnumerable{T}"/></param>
        /// <param name="predicate">The function to match <see cref="T"/></param>
        /// <returns></returns>
        public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return null;
        }
    }
}