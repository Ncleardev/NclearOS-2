using Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;

namespace NclearOS2
{
    public class Settings : Window
    {
        public static int wallpapernum = 1;
        public static int cursortype = 0;
        static Bitmap cursor1 = new(Resources.CursorIcon);
        static Bitmap cursor2 = new(Resources.CursorWhiteIcon);
        Bitmap setttingsUI = new Bitmap(Resources.SettingsUI);
        public Settings() : base("Settings", 300, 300, Resources.settings) { }
        public static string Change(string option)
        {
            switch (option)
            {
                case "wallpaper":
                    Kernel.WallpaperOn = true;
                    if (wallpapernum == 1)
                    {
                        Resources.wallpaper = Resources.wallpaperlock;
                        wallpapernum = 2;
                    }
                    else if (wallpapernum == 2)
                    {
                        Resources.wallpaper = Resources.wallpaperold;
                        wallpapernum = 3;
                    }
                    else if (wallpapernum == 3)
                    {
                        Kernel.WallpaperOn = false;
                        wallpapernum = 0;
                    }
                    else
                    {
                        Resources.wallpaper = Resources.wallpapernew;
                        wallpapernum = 1;
                    }
                    break;
                case "cursorwhite":
                    Resources.cursor = new Bitmap(Resources.CursorWhiteIcon);
                    Resources.cursorload = new Bitmap(Resources.CursorWhiteLoad);
                    cursortype = 1;
                    break;
                case "cursordark":
                    Resources.cursor = new Bitmap(Resources.CursorIcon);
                    Resources.cursorload = new Bitmap(Resources.CursorLoad);
                    cursortype = 0;
                    break;
                case "default":
                    Kernel.SystemPen = Color.SteelBlue;

                    break;
                case "red":
                    Kernel.SystemPen = Color.DarkRed;

                    break;
                case "green":
                    Kernel.SystemPen = Color.Green;

                    break;
                case "yellow":
                    Kernel.SystemPen = Color.Goldenrod;

                    break;
                case "darkblue":
                    Kernel.SystemPen = Color.MidnightBlue;

                    break;
                case "gray":
                    Kernel.SystemPen = Color.FromArgb(40, 40, 40);

                    break;
                case "black":
                    Kernel.SystemPen = Color.Black;

                    break;
                case "res":
                    string result = "Usage: set res WidthxHeight     Optional: set res WidthxHeight@ColorDepth\nExamples: set res 1920x1080     set res 1920x1080@32\nAvailable resolutions: ";
                    foreach (var i in Kernel.canvas.AvailableModes)
                    {
                        result += "\n" + i;
                    }
                    return result;
                case { } when option.StartsWith("res "):
                    return Kernel.SetRes(option[4..]);
                default:
                    return "Unknown command; Type 'set' for help";
            }
            return "Set successfully";
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.GrayPen, StartX, StartY, x, y);
            Kernel.canvas.DrawImage(setttingsUI, StartX, StartY + 7); //because of many memory leaks and performance problems I decided to use screeenshot of UI instead of drawing UI
            if (MouseManager.X > StartX + 5 && MouseManager.X < StartX + 128 && MouseManager.Y > StartY + 70 && MouseManager.Y < StartY + 90)
            {
                if (Kernel.Pressed)
                { Change("wallpaper"); }
            }
            if (Kernel.Pressed && MouseManager.Y > StartY + 105 && MouseManager.Y < StartY + 135)
            {
                if (MouseManager.X > StartX + 5 && MouseManager.X < StartX + 35) { Change("default"); }
                if (MouseManager.X > StartX + 45 && MouseManager.X < StartX + 75) { Change("red"); }
                if (MouseManager.X > StartX + 85 && MouseManager.X < StartX + 115) { Change("green"); }
                if (MouseManager.X > StartX + 125 && MouseManager.X < StartX + 155) { Change("yellow"); }
                if (MouseManager.X > StartX + 165 && MouseManager.X < StartX + 195) { Change("darkblue"); }
                if (MouseManager.X > StartX + 205 && MouseManager.X < StartX + 235) { Change("gray"); }
                if (MouseManager.X > StartX + 245 && MouseManager.X < StartX + 275) { Change("black"); }
            }
            if (Kernel.Pressed && MouseManager.Y > StartY + 25 && MouseManager.Y < StartY + 40)
            {
                if (MouseManager.X > StartX + 15 && MouseManager.X < StartX + 35)
                {
                    Change("cursordark");
                }
                else if (MouseManager.X > StartX + 50 && MouseManager.X < StartX + 70)
                {
                    Change("cursorwhite");
                }
            }
            return true;
        }
        internal override bool Start() { return true; }
        internal override int Stop() { return 0; }
        public static void UI(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawString("Cursor", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 5);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + 60, StartY + 11, StartX + x, StartY + 11);
            Kernel.canvas.DrawString("Wallpaper", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 50);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + 85, StartY + 56, StartX + x, StartY + 56);
            Kernel.canvas.DrawString("Colours", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 85);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + 68, StartY + 91, StartX + x, StartY + 91);
            Kernel.canvas.DrawImageAlpha(cursor1, StartX + 15, StartY + 25);
            Kernel.canvas.DrawImageAlpha(cursor2, StartX + 50, StartY + 25);
            Kernel.canvas.DrawString("Change background", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 70);
            Kernel.canvas.DrawString("Change background", Kernel.font, Kernel.SystemPen, StartX + 5, StartY + 70);
            Kernel.canvas.DrawFilledCircle(Kernel.DefaultPen, StartX + 20, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.RedPen, StartX + 60, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.GreenPen, StartX + 100, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.DarkBluePen, StartX + 180, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.DarkGrayPen, StartX + 220, StartY + 120, 15);
        }
        internal override void Key(ConsoleKeyEx key) { }
    }
}