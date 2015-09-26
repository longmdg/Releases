namespace SparkTech.Resources
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;

    using LeagueSharp;
    using LeagueSharp.Common;

    internal static class LoadingScreen
    {

        static LoadingScreen()
        {
            if (!Helper.IsFullHD)
            {
                // .Resize(Drawing.Width, Drawing.Height);
            }
        }

        internal static void Init()
        {
            CustomEvents.Game.OnGameLoad += Remove;
        }

        private static void Remove(EventArgs args)
        {
            // removal code
            CustomEvents.Game.OnGameLoad -= Remove;
        }

        // Credits to Lizzaran!!!!! (Not entirely sure if it's actually his work though)
        // https://github.com/Lizzaran/LeagueSharp-Dev/blob/master/SFXLibrary/Extensions/NET/BitmapExtensions.cs

        // ReSharper disable once SuggestBaseTypeForParameter
        private static Bitmap Resize(this Bitmap source, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(source, destRect, 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}
