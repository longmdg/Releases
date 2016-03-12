namespace STAutoPlay
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Timers;

    using LeagueSharp;
    using LeagueSharp.Common;

    /// <summary>
    /// Queues and processes the messages
    /// </summary>
    internal class Message
    {
        /// <summary>
        /// The content of the message
        /// </summary>
        internal readonly string Content;

        /// <summary>
        /// Determines whether the printing should be randomized
        /// </summary>
        internal readonly bool Randomize;

        /// <summary>
        /// Determines whether the printing should be instant
        /// </summary>
        internal readonly bool Instant;

        internal Message(MessageType messageType, bool allChat = false, bool randomize = true, bool instant = false, bool xd = true)
        {
            Content = (allChat ? "/all " : null) + GetContent(messageType, xd);

            Randomize = randomize;

            Instant = instant;
        }

        internal Message(string message, bool randomize = true, bool instant = false)
        {
            Content = message;

            Randomize = randomize;

            Instant = instant;
        }

        /// <summary>
        /// The dictionary - queue containing all the messages and the remaining time to say them
        /// </summary>
        private static Dictionary<Message, int> queue = new Dictionary<Message, int>();

        /// <summary>
        /// The notification
        /// </summary>
        private static readonly Notification Notification = new Notification("Pending messages: 0");

        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random(Environment.TickCount);

        /// <summary>
        /// :cat_lazy:
        /// </summary>
        private static readonly Func<KeyValuePair<Message, int>, bool> Func = x => x.Value <= 0 || x.Key.Instant;

        private static readonly MenuItem Enabled = new MenuItem("st_messaging_enable", "Enable messages").SetValue(true);

        internal static int LastSpoken;

        static Message()
        {
            // create the submenu
            {
                var child = new Menu("Messaging", "st_messaging");
                child.AddItem(Enabled);

                Enabled.ValueChanged += (sender, args) =>
                {
                    var newVal = args.GetNewValue<bool>();
                    Notification.Draw = newVal;

                    if (!newVal)
                        return;

                    queue.Clear();
                    Notification.Text = "Pending messages: 0";
                };

                Core.Menu.AddSubMenu(child);
            }

            // add the notification
            Notifications.AddNotification(Notification);

            new Timer(400d) { Enabled = true }.Elapsed += delegate
            {
                if (Core.Instance.AllowSpeaking)
                {
                    queue = queue.ToDictionary(pair => pair.Key, pair => pair.Value - 400);
                }
            };

            Game.OnUpdate += delegate
            {
                if (!Enabled.GetValue<bool>() || !Core.Instance.AllowSpeaking)
                {
                    return;
                }

                // Why don't we say some random crap at some random time lol
                if (LastSpoken - Environment.TickCount < 3000 && Random.Next(0, 1000000) == 0)
                {
                    Say(new Message(MessageType.Random));
                }

                // Don't do anything if there are no valid messages to be spoken
                if (!queue.Any(Func))
                {
                    return;
                }

                // Get the key of the thing we want to say
                var itemKey = queue.OrderByDescending(x => x.Value).FirstOrDefault(Func).Key;

                if (itemKey == null)
                {
                    Console.WriteLine("return 3");
                    return;
                }

                var next = (byte)Random.Next(0, 1);

                if (!itemKey.Randomize || next != 0)
                {
                    Game.Say(itemKey.Content);
                }

                LastSpoken = Environment.TickCount;

                // Remove the said message
                queue.Remove(itemKey);

                // Delay every other unspoken message
                queue = queue.ToDictionary(pair => pair.Key, pair => pair.Value + pair.Key.Content.Length * 150 + 800);

                // Update the notification text
                Notification.Text = $"Pending messages: {queue.Count}";
            };

            Game.OnChat += args =>
            {
                // patented algorithms kappa
                if (!args.Sender.IsMe && Random.Next(0, 7) == 0)
                {
                    // let these idiots think we care bout what they are saying
                    Delay(new Message(MessageType.Responsive, args.Sender.IsEnemy), Random.Next(800, 3000));
                }

                if (args.Sender.IsMe && args.Message == "t")
                {
                    foreach (var i in queue)
                    {
                        Console.WriteLine(i.Key.Content);
                    }
                }
            };
        }

        /// <summary>
        /// Says the message as quickly as possible
        /// </summary>
        /// <param name="msg">The message</param>
        internal static void Say(Message msg) => Delay(msg, 0);

        /// <summary>
        /// Delays the message by a specified amount of time
        /// </summary>
        /// <param name="msg"> The message </param>
        /// <param name="msDelay"> The delay in miliseconds </param>
        internal static void Delay(Message msg, int msDelay) => queue.Add(msg, msDelay);

        private static string GetContent(MessageType type, bool xd)
        {
            var rnd = Random.Next(0, 10);
            string final;
            switch (type)
            {
                case MessageType.Motivating:
                    final = Motivating[rnd];
                    break;
                case MessageType.Flaming:
                    final = Flaming[rnd];
                    break;
                case MessageType.Spamming:
                    final = Spamming[rnd];
                    break;
                case MessageType.EscapingEnemy:
                    final = EscapingEnemy[rnd];
                    break;
                case MessageType.Joke:
                    final = Joke[rnd];
                    break;
                case MessageType.Responsive:
                    final = Responsive[rnd];
                    break;
                case MessageType.EnemyLucker:
                    final = EnemyLucker[rnd];
                    break;
                case MessageType.MeLucker:
                    final = MeLucker[rnd];
                    break;
                case MessageType.Random:
                    final = RandomS[rnd];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return final + (xd ? $" {Randomxd}": null);
        }

        // Patented
        internal static string Randomxd
        {
            get
            {
                switch (Random.Next(0, 4))
                {
                    case 0:
                        return "XD";
                    case 1:
                        return null;
                    default:
                        var sb = new StringBuilder(Random.Next(1, 3) == 1 ? "x" : "X");
                        var ds = (byte)Random.Next(1, 14);
                        while (ds-- > 0)
                            sb.Append(Random.Next(1, 10) != 1 ? "D" : "d");
                        return sb.ToString();
                }
            }
        }
        // Endpatented

        #region MessageDB

        private static readonly string[] Responsive =
            {
                "lol", "lmao", "lmaooo", "yo mama", "k", "na", "2/10", "10/10",
                "i can vouch", "staph", "mkay"
            };

        private static readonly string[] Motivating =
            {
                
            };

        private static readonly string[] Flaming =
            {
                
            };

        private static readonly string[] Spamming =
            {
                
            };

        private static readonly string[] EnemyLucker =
            {
                
            };

        private static readonly string[] MeLucker =
            {
                
            };

        private static readonly string[] RandomS =
            {
                "islam is a religion of peace" 
            };

        private static readonly string[] Joke =
            {
                
            };

        private static readonly string[] EscapingEnemy =
            {
                
            };

        #endregion
    }
}