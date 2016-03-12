namespace SparkTech.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.Utils;

    using SparkTech.Cache;
    using SparkTech.Helpers;
    using SparkTech.Web;

    using static SparkTech.Features.Connecting;

    /// <summary>
    /// Responsible for awakening the core library functions and methods
    /// </summary>
    public abstract class Handler
    {
        /// <summary>
        /// Determines whether <see cref="E:OnLoad"/> has already executed
        /// </summary>
        private static bool inside;

        /// <summary>
        /// The list of assemblies that need to be inspected in <see cref="E:OnLoad"/>.
        /// <para>This list is only being added to before the <see cref="E:OnLoad"/> function actually executes</para>
        /// </summary>
        private static readonly List<Assembly> PreBoot = new List<Assembly> { Core.Assembly };

        /// <summary>
        /// Contains the instances of the processed assemblies
        /// </summary>
        private static readonly HashSet<Assembly> ProcessedAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// The calling <see cref="Assembly"/>
        /// </summary>
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class
        /// <para>This constructor will additionally check for the updates and notify the result to user</para>
        /// <para>This works with raw GitHub links only</para>
        /// </summary>
        protected Handler(string updatePath) : this()
        {
            Events.OnLoad += delegate
            {
                var name = assembly.GetName();
                new ActionUpdater(updatePath, name.Version, name.Name).CheckPerformed += args => args.Notify();
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class
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

        /// <summary>
        /// Initializes static members of the <see cref="Handler"/> class
        /// </summary>
        static Handler()
        {
            Events.OnLoad += delegate
            {
                lock (PreBoot)
                {
                    inside = true;
                    PreBoot.ForEach(Activate);
                    PreBoot.Clear();
                }

                new List<Type> { typeof(ObjectCache), typeof(Threading) }.ForEach(
                    type => RuntimeHelpers.RunClassConstructor(type.TypeHandle));

                Translations.UpdateAll();

                // let me check for my own stuff :sisi3:
                const string UpdatePath =
                    "https://raw.githubusercontent.com/Wiciaki/Releases/master/SparkTech/SparkTech/Properties/AssemblyInfo.cs";

                var name = Core.Assembly.GetName();
                new ActionUpdater(UpdatePath, name.Version, name.Name).CheckPerformed += args => args.Notify();
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

            // broscience :roto2:
            if (ObjectManager.Player == null)
            {
                Enabled = true;
            }
        }

        /// <summary>
        /// Activates the types of the provided assembly
        /// </summary>
        /// <param name="assembly">The assembly to be activated</param>
        private static void Activate(Assembly assembly)
        {
            try
            {
                lock (ProcessedAssemblies)
                {
                    if (!ProcessedAssemblies.Add(assembly))
                    {
                        return;
                    }

                    var thisAssembly = Core.Assembly == assembly;

                    // Get all the accessible types. 
                    // There's no need to check for .IsClass / !.IsAbstract etc because the later 
                    // .IsSubclassOf() check on all execution paths eliminates the need
                    var posttypes =
                        assembly.GetTypes()
                            .OrderBy(type => type.Name)
                            .ToList()
                            .FindAll(type => (thisAssembly || type.IsVisible) && type.GetConstructor(Type.EmptyTypes) != null);

                    // Don't even bother searching for the champion type if we've got one already
                    if (Core.Champion == null)
                    {
                        var hero = posttypes.Find(type => type.IsSubclassOf(typeof(HeroBase)) && type.Name == Core.ChampionName);

                        // We got the champ right here! :D
                        if (hero != null)
                        {
                            // The IsSublassOf() check ensures the cast always succeeds
                            Core.Champion = (HeroBase)DynamicInitializer.NewInstance(hero);
                        }
                    }

                    // Initialize the Miscallenous and add all menu pieces to the core
                    foreach (var type in posttypes)
                    {
                        try
                        {
                            if (type.IsSubclassOf(typeof(IMenuPiece)))
                            {
                                Core.Menu.Add(((IMenuPiece)DynamicInitializer.NewInstance(type)).Piece);
                            }
                            else if (type.IsSubclassOf(typeof(Miscallenous)))
                            {
                                DynamicInitializer.NewInstance(type);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Catch();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Catch();
            }
        }
    }
}