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

        public Settings() : base("Settings", 300, 300, new Bitmap(Resources.Settings), Priority.Realtime) { OnMoved = Moved; OnStartMoving = StartMoving; OnBackgroundChange = BgChange; }
        internal override void Update()
        {
            if (!minimized)
            {
                if (MouseManager.X > StartX + 5 && MouseManager.X < StartX + 128 && MouseManager.Y > StartY + 100 && MouseManager.Y < StartY + 115)
                {
                    Font.DrawString("Change background", GUI.SystemPen.Color, StartX + 5, StartY + 100);
                    if (GUI.Pressed)
                    {
                        GUI.Loading = true;
                        Toast.Display("Please wait...");
                        GUI.canvas.Display();
                        switch (wallpapernum)
                        {
                            case 0:
                                GUI.ApplyRes(new(Resources.WallpaperOld)); break;
                            case 1:
                                GUI.ApplyRes(new(Resources.WallpaperLock)); break;
                            case 2:
                                GUI.ApplyRes(new(Resources.WallpaperOrigami)); break;
                            case 3:
                                GUI.ApplyRes(new(Resources.Wallpaper2005s)); break;
                            case 4:
                                GUI.ApplyRes(new(Resources.WallpaperCosmos)); break;
                            default:
                                GUI.ApplyRes(new(Resources.Wallpaper)); wallpapernum = -1; break;
                        }
                        wallpapernum++;
                        AlphaBackground();
                        UI();
                        borderCanvas = PostProcess.CropBitmap(Images.wallpaperBlur, StartX, StartY, x, 30);
                        Font.DrawString(name, Color.White.ToArgb(), 36, 10, borderCanvas.rawData, x);
                        Font.DrawImageAlpha(icon, 5, 3, borderCanvas.rawData, x);
                        Font.DrawImageAlpha(Icons.minimize, x - 50, 7, borderCanvas.rawData, x);
                        Font.DrawImageAlpha(Icons.close, x - 20, 7, borderCanvas.rawData, x);
                        GUI.Loading = false;
                    }
                }
                if (GUI.Pressed && MouseManager.Y > StartY + 135 && MouseManager.Y < StartY + 165)
                {
                    if (MouseManager.X > StartX + 5 && MouseManager.X < StartX + 35) { GUI.SystemPen = GUI.BluePen; }
                    if (MouseManager.X > StartX + 45 && MouseManager.X < StartX + 75) { GUI.SystemPen = GUI.RedPen; }
                    if (MouseManager.X > StartX + 85 && MouseManager.X < StartX + 115) { GUI.SystemPen = GUI.GreenPen; }
                    if (MouseManager.X > StartX + 125 && MouseManager.X < StartX + 155) { GUI.SystemPen = GUI.YellowPen; }
                    if (MouseManager.X > StartX + 165 && MouseManager.X < StartX + 195) { GUI.SystemPen = GUI.DarkBluePen; }
                    if (MouseManager.X > StartX + 205 && MouseManager.X < StartX + 235) { GUI.SystemPen = GUI.DarkGrayPen; }
                    if (MouseManager.X > StartX + 245 && MouseManager.X < StartX + 275) { GUI.SystemPen = GUI.DarkPen; }
                }
                if (GUI.Pressed && MouseManager.Y > StartY + 55 && MouseManager.Y < StartY + 75)
                {
                    if (MouseManager.X > StartX + 15 && MouseManager.X < StartX + 35)
                    {
                        Icons.cursor = cursor1;
                        Icons.cursorload = cursor1L;
                        cursorWhite = false;
                    }
                    else if (MouseManager.X > StartX + 50 && MouseManager.X < StartX + 70)
                    {
                        Icons.cursor = cursor2;
                        Icons.cursorload = cursor2L;
                        cursorWhite = true;
                    }
                }
            }
        }
        internal override int Start() { AlphaBackground(); UI(); return 0; }
        internal override int Stop() { return 0; }
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
        private void BgChange() { Background(0); UI(); }
    }
}