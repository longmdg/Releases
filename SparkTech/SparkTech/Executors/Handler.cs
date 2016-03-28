namespace SparkTech.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SparkTech.Cache;
    using SparkTech.Helpers;
    using SparkTech.Web;

    using static Features.Connecting;

    /// <summary>
    ///     Responsible for awakening the core library functions and methods
    /// </summary>
    public abstract class Handler
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
        ///     Determines whether the <see cref="E:OnLoad" /> has already executed
        /// </summary>
        private static bool inside;

        #endregion

        #region Fields

        /// <summary>
        ///     The calling <see cref="Assembly" />
        /// </summary>
        private readonly Assembly assembly;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Handler" /> class
        /// </summary>
        static Handler()
        {
            Events.OnLoad += delegate
            {
                RuntimeHelpers.RunClassConstructor(typeof(ObjectCache).TypeHandle);
                RuntimeHelpers.RunClassConstructor(typeof(Threading).TypeHandle);

                foreach (var type in Core.Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(IMenuPiece))))
                {
                    Core.Menu.Add(Initializer.CreateInstance<IMenuPiece>(type).Piece);
                }

                lock (PreBoot)
                {
                    inside = true;
                    PreBoot.ForEach(Activate);
                    PreBoot.Clear();
                    PreBoot.TrimExcess();
                }

                Translations.UpdateAll();

                // let me check for my own stuff :sisi3:
                const string Link = "https://raw.githubusercontent.com/Wiciaki/Releases/master/SparkTech/SparkTech/Properties/AssemblyInfo.cs";

                new ActionUpdater(Link, Core.Assembly).CheckPerformed += args => args.Notify();
            };

            Game.OnStart += delegate
            {
                if (Enabled)
                {
                    Enabled = false;
                }
            };

            Game.OnEnd += delegate { };

            // broscience :roto2:
            if (ObjectManager.Player == null)
            {
                Enabled = true;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Handler" /> class
        ///     <para>This constructor will additionally check for the updates and notify the result to user</para>
        ///     <para>This works with raw GitHub links only</para>
        /// </summary>
        protected Handler(string updatePath) : this()
        {
            Events.OnLoad += delegate
            {
                new ActionUpdater(updatePath, assembly).CheckPerformed += args => args.Notify();
            };
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Handler" /> class
        /// </summary>
        protected Handler()
        {
            assembly = Assembly.GetAssembly(GetType());

            if (inside)
            {
                Activate(assembly);
            }
            else
            {
                PreBoot.Add(assembly);
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
                    .OrderBy(type => type.Name)
                    .ToList()
                    .FindAll(type => type.IsVisible && type.GetConstructor(Type.EmptyTypes) != null);

            // Don't even bother searching for the champion type if we've got one already
            if (Core.Champion == null)
            {
                var hero = posttypes.Find(type => type.IsSubclassOf(typeof(HeroBase)) && type.Name == Core.ChampionName);

                // We got the champ right here! :D
                if (hero != null)
                {
                    Core.Champion = Initializer.CreateInstance<HeroBase>(hero);
                }
            }

            // Initialize all the the Miscallenous
            foreach (var type in posttypes.FindAll(type => type.IsSubclassOf(typeof(Miscallenous))))
            {
                Initializer.CreateInstance<Miscallenous>(type);
            }
        }

        #endregion
    }
}