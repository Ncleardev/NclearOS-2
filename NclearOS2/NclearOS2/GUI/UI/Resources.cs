﻿using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;

namespace NclearOS2.GUI
{
    public static class Icons
    {
        //Cursors
        public static Bitmap cursor;
        public static Bitmap cursorload;
        //UI
        public static Bitmap warn;
        public static Bitmap info;
        public static Bitmap error;
        public static Bitmap load;

        public static Bitmap close;
        public static Bitmap close2;
        public static Bitmap minimize;
        public static Bitmap minimize2;
        public static Bitmap min;
        public static Bitmap min2;
        public static Bitmap max;
        public static Bitmap max2;

        public static Bitmap program;

        public static Bitmap shutdown;
        public static Bitmap reboot;
        public static Bitmap lockicon;
        public static Bitmap connected;
    }
    public static class Images
    {
        //wallpapers
        public static Bitmap systemWallpaper { get; private set; }
        public static Bitmap wallpaper;
        public static Bitmap wallpaperBlur;
        public static void RequestSystemWallpaperChange(Bitmap bitmap)
        {
            systemWallpaper = bitmap;
        }
    }
}