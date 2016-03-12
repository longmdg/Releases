namespace SparkTech
{
    using System;
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