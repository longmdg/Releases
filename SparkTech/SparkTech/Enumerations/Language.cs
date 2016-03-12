namespace SparkTech.Enumerations
{
    using System.ComponentModel;

    /// <summary>
    /// Contains available translations
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// The <see cref="English"/> language
        /// </summary>
        [Description("en-US")]
        English = 0,

        /// <summary>
        /// The <see cref="Polish"/> language
        /// </summary>
        [Description("pl-PL")]
        Polish = 1
    }
}