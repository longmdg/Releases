/*
namespace SparkTech
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using SparkTech.Properties;

    public static class Logger
    {
        private static readonly StreamWriter Writer;

        static Logger()
        {
            Writer = new StreamWriter(Path.Combine(Settings.Files.LogPath, DateTime.Today.Date.ToShortDateString() + ".txt"), true);

            var finfo = new FileInfo(Path.Combine(Settings.Files.LogPath, "SystemInfo.txt"));

            if (finfo.Exists && finfo.LastWriteTime > DateTime.Now.AddDays(-3))
            {
                return;
            }
            if (finfo.Exists)
            {
                finfo.Delete();
            }

            var file = finfo.CreateText();
            var ibul = new StringBuilder();

            ibul.AppendLine("--- ST Info File ---");
            ibul.AppendLine();
            ibul.AppendLine($"Hello, {Environment.UserName} :)");
            ibul.AppendLine();
            ibul.AppendLine($"System Data: {Environment.OSVersion}");
            ibul.AppendLine($".NET Framework Version: {Environment.Version}");
            ibul.AppendLine();
            ibul.AppendLine($"Spaghetti version that generated this: {Assembly.GetExecutingAssembly().GetName().Version}");
            ibul.AppendLine();
            ibul.AppendLine("To be expanded in the the upcoming updates :P");
            ibul.AppendLine(Resources.LogfileEnder);

            file.WriteLine(ibul.ToString());
            file.Close();
        }

        [Obsolete]
        public static void Catch(Exception ex)
        {
            Catch($"What error? : {ex.Message}{Environment.NewLine}Where? :{ex.StackTrace}{Environment.NewLine}When? : {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
        }

        [Obsolete]
        public static void Catch(string msg)
        {
            Writer?.WriteLine($"{msg}{Resources.LogfileEnder}");
        }
    }
}*/

/*
namespace SparkTech.Helpers
{
    using System;
    using System.IO;
    using System.Text;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Properties;

    using Booters;
    
    public static class Logger
    {
        private static readonly string LogPath;

        static Logger()
        {
            try
            {
                LogPath = Path.Combine(
                    Settings.Files.Root,
                    "Logs",
                    DateTime.Today.Date.ToShortDateString() + ".txt");

                Boot.OnInit += delegate
                    {
                        Catch($"Initializing a new instance as {ObjectManager.Player.ChampionName} on the {Utility.Map.GetMap().Name} map. Enjoy your game!");
                    };
            }
            catch (Exception ex)
            {
         //       Comms.Print(ex.ToString(), true);
            }
        }
        
        public static void Catch(Exception ex)
        {
            //var sb = new StringBuilder("");

            Catch($"What error? : {ex.Message}{Environment.NewLine}Where? :{ex.StackTrace}{Environment.NewLine}When? : {DateTime.Now}");
        }
        
        public static void Catch(string text)
        {
            try
            {
                if (LogPath != null)
                {
                    using (var writer = new StreamWriter(LogPath, true))
                    {
                        writer.WriteLine(text + Resources.LogfileEnder);
                    }
                }
                else
                {
                    Console.WriteLine(text + Resources.LogfileEnder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Couldn't add a line :" + Environment.NewLine + ex);
            }
        }
    }
}*/

/*
namespace SparkTech.Logger
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    using LeagueSharp;
    using LeagueSharp.Common;

    public abstract class Logger
    {
        protected Logger()
        {
            CustomEvents.Game.OnGameLoad += delegate
            {
                var builder = new StringBuilder();
                builder.AppendLine($"As of {DateTime.Now.ToLongTimeString()}, a new game was just detected!");
                builder.AppendLine($"Initializing a new instance as {ObjectManager.Player.ChampionName} on the {Utility.Map.GetMap().Name} map. Enjoy your game!");

                Write(builder.ToString());
            };

            var finfo = new FileInfo(Path.Combine(Settings.Files.LogPath, "SystemInfo.txt"));

            if (finfo.Exists && finfo.LastWriteTime > DateTime.Now.AddDays(-3))
            {
                return;
            }
            if (finfo.Exists)
            {
                finfo.Delete();
            }

            var ibul = new StringBuilder();

            ibul.AppendLine("--- ST Info File ---");
            ibul.AppendLine();
            ibul.AppendLine($"Hi, {Environment.UserName} :)");
            ibul.AppendLine();
            ibul.AppendLine($"System Data: {Environment.OSVersion}");
            ibul.AppendLine($".NET Framework Version: {Environment.Version}");
            ibul.AppendLine();
            ibul.AppendLine($"Spaghetti version that generated this: {Assembly.GetExecutingAssembly().GetName().Version}");
            ibul.AppendLine();
            ibul.AppendLine("To be expanded in one of the upcoming updates :D");
            ibul.AppendLine(Properties.Resources.LogfileEnder);

            var file = finfo.CreateText();
            file.WriteLine(ibul.ToString());
            file.Close();
        }

        public void Catch(Exception ex)
        {
            CustomEvents.Game.OnGameLoad +=
                args =>
                Write(
                    $"What error? : {ex.Message}{Environment.NewLine}Where? :{ex.StackTrace}{Environment.NewLine}When? : {DateTime.Now}");
        }

        public void Catch(string msg)
        {
            CustomEvents.Game.OnGameLoad += args => Write(msg);
        }
        
        protected abstract void Write(string msg);
    }
}
*/

/*
namespace SparkTech.Logger
{
    using System;
    using System.IO;

    using Properties;

    internal sealed class File : Logger
    {
        private static readonly StreamWriter Writer =
            new StreamWriter(
                Path.Combine(Settings.Files.LogPath, DateTime.Today.Date.ToShortDateString() + ".txt"),
                true);

        protected override void Write(string msg)
        {
            Writer.WriteLine(msg + Resources.LogfileEnder);
        }
    }
}
*/