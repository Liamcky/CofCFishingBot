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
                    int offset = 7;
                    int rMinX = Math.Max(area.Left(), x - offset);
                    int rMaxX = Math.Min(area.Right() - 1, x + offset);

                    //X-Achse
                    for (int rx = rMinX; rx <= rMaxX; rx++)
                    {
                        Color rc = ImageHandler.GetPixel(bmp, rx, y);
                        if (CheckIsClose(rc, schwimmer, 40))
                            return true;
                    }
                }
            }

            return false;
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
