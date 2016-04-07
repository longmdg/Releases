namespace SparkTech.Features
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SparkTech.Cache;
    using SparkTech.Executors;

    /// <summary>
    /// The class offering you to draw text under the champions in an easy way
    /// </summary>
    public class ObjectText : IMenuPiece
    {
        /// <summary>
        /// The <see cref="ObservableCollection{T}"/> of champion texts to be drawn
        /// </summary>
        public static readonly ObservableCollection<ObjectTextEntry> Entries = new ObservableCollection<ObjectTextEntry>();

        /// <summary>
        /// Determined the height difference between consecutive drawn texts
        /// </summary>
        private const byte StepSize = 25;

        /// <summary>
        /// Holds the text menu item
        /// </summary>
        private static Menu menu;

        /// <summary>
        /// The <see cref="Menu"/> holding down the values
        /// </summary>
        Menu IMenuPiece.Piece
        {
            get
            {
                menu = new Menu("st_core_drawings_text", "Text below units");
                Entries.ForEach(AddItem);
                return menu;
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="E:TextHandlerMenu"/>
        /// </summary>
        /// <param name="item">The <see cref="ObjectText"/>'s component to be added</param>
        private static void AddItem(ObjectTextEntry item)
        {
            menu?.Add(new MenuBool($"st_core_drawings_text_{item.Id}", $"Enable \"{item.MenuText}\"", item.OnByDefault));
        }

        /// <summary>
        /// Removes an item from the <see cref="E:TextHandlerMenu"/>
        /// </summary>
        /// <param name="item">The <see cref="ObjectText"/>'s component to be removed</param>
        private static void RemoveItem(ObjectTextEntry item)
        {
            menu?.Remove(menu.Components.Values.OfType<MenuBool>().Single(component => component.Name == $"st_core_drawings_text_{item.Id}"));
        }

        /// <summary>
        /// Initializes static members of the <see cref="ObjectText"/> class
        /// </summary>
        static ObjectText()
        {
            Entries.CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddItem((ObjectTextEntry)args.NewItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveItem((ObjectTextEntry)args.OldItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        // thought you could break my stuff? I have some bad news for you :kappa:
                        args.OldItems.Cast<ObjectTextEntry>().ForEach(Entries.Add);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        AddItem((ObjectTextEntry)args.NewItems[0]);
                        RemoveItem((ObjectTextEntry)args.OldItems[0]);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(args.Action), args.Action, null);
                }
            };

            Drawing.OnDraw += delegate
            {
                    if (menu == null || !menu["st_core_drawings_text_enable"].GetValue<MenuBool>().Value)
                    {
                        return;
                    }

                    var entries =
                        Entries.Where(
                            item =>
                            menu[$"st_core_drawings_text_{item.Id}"].GetValue<MenuBool>().Value && item.Condition())
                            .OrderBy(item => item.Id).ToList();

                    foreach (var o in ObjectCache.GetNative<GameObject>())
                    {
                        var pos = Drawing.WorldToScreen(o.Position);
                        var steps = 0;

                        foreach (var item in entries.FindAll(item => item.Draw(o)))
                        {
                            var text = item.DrawnText(o);

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                Drawing.DrawText(pos.X - text.Length * 5, pos.Y - steps++ * StepSize, item.Color(o), text);
                            }
                        }
                    }
            };
        }

        /// <summary>
        /// The <see cref="ObjectTextEntry"/> nested class
        /// </summary>
        public class ObjectTextEntry
        {
            /// <summary>
            /// Responsible for delivering the Ids
            /// </summary>
            private static ushort id;

            /// <summary>
            /// The <see cref="Func{TResult}"/> whether to draw on the specified <see cref="GameObject"/>
            /// </summary>
            internal readonly Func<GameObject, bool> Draw;

            /// <summary>
            /// The <see cref="System.Drawing.Color"/> <see cref="Func{TResult}"/>
            /// </summary>
            internal readonly Func<GameObject, Color> Color;

            /// <summary>
            /// The condition <see cref="Func{TResult}"/>
            /// </summary>
            internal readonly Func<bool> Condition;

            /// <summary>
            /// The drawing <see cref="E:id"/>
            /// </summary>
            internal readonly ushort Id;

            /// <summary>
            /// The text to be drawn
            /// </summary>
            internal readonly Func<GameObject, string> DrawnText;

            /// <summary>
            /// The text to appear in the menu
            /// </summary>
            internal readonly string MenuText;

            /// <summary>
            /// Indicates whether this item should be enabled by default
            /// </summary>
            internal readonly bool OnByDefault;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObjectText"></see> class
            /// </summary>
            /// <param name="drawOnObject">The <see cref="Func{TResult}"/> whether to draw on the specified <see cref="GameObject"/></param>
            /// <param name="color">The <see cref="System.Drawing.Color"/> <see cref="Func{TResult}"/></param>
            /// <param name="condition">The condition <see cref="Func{TResult}"/></param>
            /// <param name="drawnText">The text to be drawn</param>
            /// <param name="menuText">The text to appear in the menu</param>
            /// <param name="onByDefault">Indicates whether this item should be enabled by default</param>
            public ObjectTextEntry(
                Func<GameObject, bool> drawOnObject,
                Func<GameObject, Color> color,
                Func<bool> condition,
                Func<GameObject, string> drawnText,
                string menuText,
                bool onByDefault = true)
            {
                Draw = drawOnObject ?? (o => false);

                Color = color ?? (o => System.Drawing.Color.White);

                Condition = condition ?? (() => false);

                DrawnText = drawnText ?? (o => null);

                MenuText = menuText;

                OnByDefault = onByDefault;

                Id = ++id;
            }
        }
    }
}