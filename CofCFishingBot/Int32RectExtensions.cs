using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CofCFishingBot
{
    public static class Int32RectExtensions
    {
        public static int Left(this Int32Rect r) => r.X;
        public static int Top(this Int32Rect r) => r.Y;
        public static int Right(this Int32Rect r) => r.X + r.Width;
        public static int Bottom(this Int32Rect r) => r.Y + r.Height;
    }
}

