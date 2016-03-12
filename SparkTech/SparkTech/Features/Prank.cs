 // ReSharper disable once CheckNamespace
namespace SparkTech.Features.Pranks
{
    using System;

    using SparkTech.Executors;

    public partial class Prank : Miscallenous
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Prank"/> class
        /// </summary>
        public Prank()
        {
            var today = DateTime.Today;
            var month = today.Month;
            var day = today.Day;

            if (month == 4 && day == 1)
            {
                FirstApril();
            }
        }
        
        /// <summary>
        /// Executes on the first of april
        /// </summary>
        static partial void FirstApril();
    }
}