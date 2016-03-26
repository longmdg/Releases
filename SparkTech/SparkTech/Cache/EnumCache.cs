namespace SparkTech.Cache
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    // http://i.imgur.com/SVLO5l1.png

    /// <summary>
    /// Exposes the enumeration for fast access
    /// </summary>
    /// <typeparam name="TEnum">The enumeration to be cached</typeparam>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "The members differ for every generic type provided, therefore the supression is fine.")]
    public static class EnumCache<TEnum> where TEnum : struct, IConvertible
    {
        /// <summary>
        /// The enumeration values represented by a list
        /// </summary>
        public static readonly List<TEnum> Values;

        /// <summary>
        /// The names of the constants in the enumeration
        /// </summary>
        public static readonly List<string> Names;

        /// <summary>
        /// The amount of all the values in an enumeration
        /// </summary>
        public static readonly int Count;

        /// <summary>
        /// Contains the descriptions of the enum members
        /// </summary>
        private static readonly Dictionary<TEnum, string> Descriptions;

        /// <summary>
        /// Retrieves a description from the specified <see cref="TEnum"/>
        /// <para>This returns null if the item has no description</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string Description(TEnum item) => Descriptions[item];

        /// <summary>
        /// Initializes static members of the <see cref="EnumCache{TEnum}"/> class
        /// </summary>
        /// <exception cref="InvalidOperationException">TEnum is not an enum</exception>
        /// <exception cref="InvalidEnumArgumentException">The count is wrong</exception>
        static EnumCache()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("TEnum must be of an enumerable type!");
            }

            Values = ((TEnum[])Enum.GetValues(typeof(TEnum))).OrderByDescending(item => item).ToList();

            Names = Enum.GetNames(typeof(TEnum)).ToList();

            if (Values.Count != Names.Count)
            {
                throw new InvalidEnumArgumentException($@"Invalid count in enum ""{typeof(TEnum).Name}""");
            }

            Count = Values.Count;

            Descriptions = Values.ToDictionary(@enum => @enum, @enum =>
                (typeof(TEnum).GetMember(@enum.ToString(CultureInfo.InvariantCulture)).Single()
                     .GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute)?.Description);
        } 
    }
}