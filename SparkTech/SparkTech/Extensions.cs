namespace SparkTech
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;

    using SparkTech.Enumerations;
    using SparkTech.Helpers;

    /// <summary>
    /// Provides extension method for both internal and external usage
    /// </summary>
    public static class Extensions
    {
        #region Public Extensions

        /// <summary>
        /// Debugs an exception
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to be logged</param>
        /// <param name="message">The debug message</param>
        /// <param name="memberName">The member name of the caller</param>
        public static void Catch(this Exception ex, string message, [CallerMemberName] string memberName = null)
        {
            Comms.Debug(memberName ?? "Null", message);

            ExceptionH.Catch(ex, LogLevel.Debug);
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> instance</param>
        /// <param name="level">The <see cref="LogLevel"/></param>
        public static void Catch(this Exception ex, LogLevel level = LogLevel.Error) => ExceptionH.Catch(ex, level);

        /// <summary>
        /// Returns a new list that will contain only instances of the requested type
        /// </summary>
        /// <typeparam name="TOld">The original type</typeparam>
        /// <typeparam name="TNew">The requested type</typeparam>
        /// <param name="source">The original collection</param>
        /// <returns></returns>
        public static List<TNew> OfType<TOld, TNew>(this IEnumerable<TOld> source) where TNew : class, TOld
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var container = new List<TNew>();

            // ReSharper disable once TooWideLocalVariableScope
            TNew temp;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in source as IList<TOld> ?? source)
            {
                if ((temp = item as TNew) != null)
                {
                    container.Add(temp);
                }
            }

            return container;
        }

        #endregion

        #region Internal Extensions

        /// <summary>
        /// Updates the display name of a specified menu component
        /// </summary>
        /// <param name="component">The <see cref="AMenuComponent"/> instance to be updated</param>
        /// <param name="language">The specified language</param>
        /// <returns></returns>
        internal static void Update(this AMenuComponent component, Language? language = null) => Translations.Update(component, language);

        #endregion
    }
}