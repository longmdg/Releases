namespace SparkTech.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UseNullPropagation")]
    internal class Menu
    {
        #region Event

        internal delegate void OnMenuCreatedH();
        internal static event OnMenuCreatedH OnMenuCreated;

        private static void FireOnMenuCreated()
        {
            if (OnMenuCreated != null)
            {
                OnMenuCreated();
            }
        }

        #endregion

        static Menu()
        {
            FireOnMenuCreated();
        }
    }
}
