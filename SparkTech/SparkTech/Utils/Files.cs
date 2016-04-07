/*
namespace SparkTech.Utils
{
    using System;
    using System.IO;
    using System.Linq;

    internal static class Files
    {
        private static bool exp;

        internal static void Expand()
        {
            try
            {
                if (exp) return;
                exp = true;

                foreach (var language in Core.Menu.Item("st_language").GetValue<StringList>().SList)
                {
                    // ty L33T and Asuna
                    var resource = Properties.Resources.ResourceManager.GetObject(language) as byte[];

                    if (resource == null)
                    {
                        Settings.Logger.Catch(language + ".xml couldn't be found!");
                        continue;
                    }

                    /* TODO
                    var destinationPath = Path.Combine(Settings.Files.Cache, language + ".xml");

                    if (!File.Exists(destinationPath) || (File.Exists(destinationPath) && File.GetLastWriteTime(destinationPath) < File.GetLastWriteTime("")))
                    {
                        File.WriteAllBytes(destinationPath, resource);
                    }

                    File.WriteAllBytes(Path.Combine(Settings.Files.CachePath, language + ".xml"), resource);
                }

                if (!new FileInfo(Settings.Files.Spaghetti).Exists)
                {
                    Properties.Resources.p06gEZU1?.Save(Settings.Files.Spaghetti); //, ImageFormat.Bmp); // TODO verify
                }

                foreach (var file in Directory.GetFiles(Settings.Files.LogPath).Select(x => new FileInfo(x)).Where(x => x.Exists && x.CreationTime <= DateTime.Now.AddMonths(-1)))
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                Settings.Logger.Catch(ex);
            }


        }
    }
}*/