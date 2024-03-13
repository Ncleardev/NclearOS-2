using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using NclearOS2.Commands;
using System.Drawing;

namespace NclearOS2.GUI
{
    internal class Settings : Window
    {
        static readonly Bitmap cursor1 = new(Resources.Cursor);
        static readonly Bitmap cursor1L = new(Resources.CursorLoad);
        public static readonly Bitmap cursor2 = new(Resources.CursorWhite);
        public static readonly Bitmap cursor2L = new(Resources.CursorWhiteLoad);

        public static int wallpapernum = 0;
        public static bool cursorWhite = false;

        public Settings() : base("Settings", 300, 300, new Bitmap(Resources.Settings), ProcessManager.Priority.None) { OnMoved = Moved; OnStartMoving = StartMoving; OnClicked = Clicked; }
        private void Clicked(int x2, int y2)
        {
            if (x2 > 5 && x2 < 140 && y2 > 70 && y2 < 85)
            {
                if (GUI.ScreenX * GUI.ScreenY > Sysinfo.AvailableRAM * 28000) { Notify("Not enough system memory to complete operation!"); return; }
                double beforebefore = Sysinfo.UsedRAM;
                GUI.Loading = true;
                Toast.Display("Please wait...");
                GUI.canvas.Display();
                if (wallpapernum > 4) { wallpapernum = -1; }
                wallpapernum++;
                LoadWallpaper();
                double before = Sysinfo.UsedRAM;
                GUI.ApplyRes();
                double almost = Sysinfo.UsedRAM;
                AlphaBackground();
                UI();
                //RefreshBorder();
                GUI.Loading = false;
                double end = Sysinfo.UsedRAM;
                if (Kernel.Debug) { Notify("Before operation: " + beforebefore + "  Before ApplyRes: " + before + "  After ApplyRes: " + almost + "  After operation: " + end); }
            }
            if (y2 > 105 && y2 < 135)
            {
                if (x2 > 5 && x2 < 35) { GUI.SystemPen = GUI.BluePen; }
                if (x2 > 45 && x2 < 75) { GUI.SystemPen = GUI.RedPen; }
                if (x2 > 85 && x2 < 115) { GUI.SystemPen = GUI.GreenPen; }
                if (x2 > 125 && x2 < 155) { GUI.SystemPen = GUI.YellowPen; }
                if (x2 > 165 && x2 < 195) { GUI.SystemPen = GUI.DarkBluePen; }
                if (x2 > 205 && x2 < 235) { GUI.SystemPen = GUI.DarkGrayPen; }
                if (x2 > 245 && x2 < 275) { GUI.SystemPen = GUI.DarkPen; }
            }
            if (y2 > 25 && y2 < 45)
            {
                if (x2 > 15 && x2 < 35)
                {
                    Icons.cursor = cursor1;
                    Icons.cursorload = cursor1L;
                    cursorWhite = false;
                }
                else if (x2 > 50 && x2 < 70)
                {
                    Icons.cursor = cursor2;
                    Icons.cursorload = cursor2L;
                    cursorWhite = true;
                }
            }
        }
        internal override int Start() { AlphaBackground(); UI(); return 0; }
        private void Moved() { AlphaBackground(); UI(); }
        private void StartMoving() { Background(0); UI(); }
        private void UI()
        {
            DrawStringAlpha("Cursor", Color.White.ToArgb(), 5, 5);
            DrawHorizontalLine(Color.White.ToArgb(), 60, 11, x);
            DrawStringAlpha("Wallpaper", Color.White.ToArgb(), 5, 50);
            DrawHorizontalLine(Color.White.ToArgb(), 85, 56, x);
            DrawStringAlpha("Colours", Color.White.ToArgb(), 5, 85);
            DrawHorizontalLine(Color.White.ToArgb(), 68, 91, x);
            DrawStringAlpha("Change background", Color.White.ToArgb(), 5, 70);
            DrawFilledCircle(GUI.BluePen.ValueARGB, 20, 120, 15);
            DrawFilledCircle(GUI.RedPen.ValueARGB, 60, 120, 15);
            DrawFilledCircle(GUI.GreenPen.ValueARGB, 100, 120, 15);
            DrawFilledCircle(GUI.YellowPen.ValueARGB, 140, 120, 15);
            DrawFilledCircle(GUI.DarkBluePen.ValueARGB, 180, 120, 15);
            DrawFilledCircle(GUI.DarkGrayPen.ValueARGB, 220, 120, 15);
            DrawFilledCircle(GUI.DarkPen.ValueARGB, 260, 120, 15);
            DrawImageAlpha(cursor1, 15, 25);
            DrawImageAlpha(cursor2, 50, 25);
        }
        public static void LoadWallpaper()
        {
            Images.RequestSystemWallpaperChange(wallpapernum switch
            {
                1 => new Bitmap(Resources.WallpaperOld),
                2 => new Bitmap(Resources.WallpaperLock),
                3 => new Bitmap(Resources.WallpaperOrigami),
                4 => new Bitmap(Resources.Wallpaper2005s),
                5 => new Bitmap(Resources.WallpaperCosmos),
                _ => new Bitmap(Resources.Wallpaper),
            });
        }
    }
}