namespace SparkTech.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SparkTech.Cache;
    using SparkTech.Utils;
    using SparkTech.Web;

    using static Features.Connecting;

    /// <summary>
    ///     Responsible for awakening the core library functions and methods
    /// </summary>
    internal static class Handler
    {
        #region Static Fields

        /// <summary>
        ///     The list of assemblies that need to be inspected in <see cref="E:OnLoad" />.
        ///     <para>
        ///         This list is only being added to before the <see cref="E:OnLoad" /> function actually executes and is
        ///         obsoleted afterwards
        ///     </para>
        /// </summary>
        private static readonly List<Assembly> PreBoot = new List<Assembly>();

        /// <summary>
        ///     The permission set that lifts the sandbox restrictions on creating instances
        /// </summary>
        private static readonly PermissionSet Unrestricter = new PermissionSet(PermissionState.Unrestricted);

        /// <summary>
        ///     Determines whether the <see cref="E:OnLoad" /> has already executed
        /// </summary>
        private static bool inside;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Handler" /> class
        /// </summary>
        static Handler()
        {
            Events.OnLoad += delegate
            {
                new List<Type> { typeof(ObjectCache), typeof(SparkWalker.Orbwalker), typeof(Threading) }.ForEach(
                    type => RuntimeHelpers.RunClassConstructor(type.TypeHandle));

                Unrestricter.Assert();

                /* Initialize the library components */
                foreach (var type in Core.Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(IMenuPiece))))
                {
                    Core.Menu.Add(
                        ((IMenuPiece)
                         Activator.CreateInstance(
                             type,
                             BindingFlags.NonPublic | BindingFlags.Instance,
                             null,
                             null,
                             null)).Piece);
                }

                CodeAccessPermission.RevertAssert();

                Translations.UpdateAll();

                /* Initialize the outer components */
                lock (PreBoot)
                {
                    inside = true;
                    PreBoot.ForEach(Activate);
                    PreBoot.Clear();
                    PreBoot.TrimExcess();
                }
            };

            Game.OnStart += delegate
            {
                if (Enabled)
                {
                    Enabled = false;
                }
            };

            Game.OnEnd += delegate
            {
                
            };
            
            if (ObjectManager.Player == null)
            {
                Enabled = true;
            }

            new ActionUpdater(StringH.UpdatePath, Core.Assembly).CheckPerformed += args =>
            {
                Events.OnLoad += delegate
                { args.Notify(); };
            };
        }

        /// <summary>
        ///     Adds an assembly to the initialization list
        /// </summary>
        internal static void Register(Assembly caller)
        {
            lock (PreBoot)
            {
                if (!inside)
                {
                    PreBoot.Add(caller);
                }
                else
                {
                    Activate(caller);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Activates the types of the provided assembly
        /// </summary>
        /// <param name="assembly">The assembly to be activated</param>
        private static void Activate(Assembly assembly)
        {
            // Get all the accessible types. 
            // There's no need to check for .IsClass / !.IsAbstract etc because the later 
            // .IsSubclassOf() check on all execution paths eliminates the need
            var posttypes =
                assembly.GetTypes()
                    .Where(type => type.IsVisible && type.GetConstructor(Type.EmptyTypes) != null)
                    .ToList();

            // Initialize all the the Miscallenous types
            posttypes.FindAll(type => type.IsSubclassOf(typeof(Miscallenous))).ForEach(type => New<Miscallenous>(type));

            // Don't even bother searching for the champion type if we've got one already :roto2:
            if (Core.Champion != null)
            {
                return;
            }

            var hero = posttypes.Find(type => type.IsSubclassOf(typeof(HeroBase)) && type.Name == Core.ChampionName);

            // We got the champ right here! :D
            if (hero != null)
            {
                Core.Champion = New<HeroBase>(hero);
            }
        }

        /// <summary>
        ///     Create a new instance of the specified type
        /// </summary>
        /// <typeparam name="TType">The requested output type</typeparam>
        /// <param name="type">The type to be instantiated</param>
        /// <returns></returns>
        public static TType New<TType>(Type type = null) where TType : class
        {
            if (type == null)
            {
                type = typeof(TType);
            }

            if (!type.IsVisible || (!type.IsValueType && (!type.IsClass || type.IsAbstract)))
            {
                return null;
            }

            Unrestricter.Assert();

            var instance = (TType)Activator.CreateInstance(type);

            CodeAccessPermission.RevertAssert();

            return instance;
        }

        #endregion
    }
}