using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CofCFishingBot
{
    public static class TriggerDetection
    {
        #region Detect Fangbereich Area und der Schwimmer darin
        public static bool CheckYellowWithRedPoint(BitmapSource bmp, Int32Rect area, Color fangbereich, Color schwimmer)
        {
            //Schnittbereich in der Area
            Int32Rect gameRect = new Int32Rect(0, 0, (int)bmp.Width, (int)bmp.Height);
            area = ImageHandler.Intersect(gameRect, area);

            //alle Pixel durchsuchen
            //Y-Achse
            for (int y = area.Top(); y < area.Bottom(); y++)
            {
                //X-Achse
                for (int x = area.Left(); x < area.Right(); x++)
                {
                    //Finde Farbe des Pixels
                    Color c = ImageHandler.GetPixel(bmp, x, y);
                    if (!CheckIsClose(c, fangbereich, 40))
                        continue;
                    int offset = 10;
                    int rMinX = Math.Max(area.Left(), x - offset);
                    int rMaxX = Math.Min(area.Right() - 1, x + offset);
                    int rMinY = Math.Max(area.Top(), y - offset);
                    int rMaxY = Math.Min(area.Bottom() - 1, y + offset);

                    //Y-Achse durchgehen
                    for (int ry = rMinY; ry <= rMaxY; ry++)
                    {
                        //X-Achse
                        for (int rx = rMinX; rx <= rMaxX; rx++)
                        {
                            Color rc = ImageHandler.GetPixel(bmp, rx, ry);
                            if (CheckIsClose(rc, schwimmer, 40))
                                return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Find Point on Screen
        /// <summary>
        /// Finds the Point on Screen to Setup the Bot
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>Point where the button is to Setup the Bot</returns>
        public static Point FindPointOnScreen(BitmapSource bitmap)
        {
            Color button = Color.FromRgb(184, 182, 48);

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            int stride = width * bytesPerPixel;

            byte[] pixels = new byte[stride * height];
            bitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height; y++)
            {
                int row = y * stride;

                for (int x = 0; x < width; x++)
                {
                    int index = row + x * bytesPerPixel;

                    byte b = pixels[index];
                    byte g = pixels[index + 1];
                    byte r = pixels[index + 2];

                    //Alpha only for 32Bit
                    byte a = bytesPerPixel >= 4 ? pixels[index + 3] : (byte)255;

                    Color c = Color.FromArgb(a, r, g, b);

                    if (CheckIsClose(c, button, 5))
                        return new Point(x, y);
                }
            }

            return new Point(-1, -1);
        }

        #endregion

        #region Toleranz Bereich für Farbe
        /// <summary>
        /// Check Color in Toleranz Bereich
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="tolerance"></param>
        /// <returns>true wenn Farbe nah dran ansonsten false</returns>
        public static bool CheckIsClose(Color a, Color b, int tolerance)
        {
            return Math.Abs(a.R - b.R) <= tolerance &&
                   Math.Abs(a.G - b.G) <= tolerance &&
                   Math.Abs(a.B - b.B) <= tolerance;
        }
        #endregion
    }
}
