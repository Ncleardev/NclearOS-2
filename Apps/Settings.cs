using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;

namespace NclearOS2
{
    public class Settings
    {
        public static int wallpapernum = 1;
        public static void Change(string option)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            switch (option)
            {
                case "wallpaper":
                    Kernel.WallpaperOn = true;
                    if (wallpapernum == 1)
                    {
                        Kernel.wallpaper = Kernel.wallpaperlock;
                        wallpapernum = 2;
                    }
                    else if (wallpapernum == 2)
                    {
                        Kernel.wallpaper = Kernel.wallpaperold;
                        wallpapernum = 3;
                    }
                    else if (wallpapernum == 3)
                    {
                        Kernel.WallpaperOn = false;
                        wallpapernum = 0;
                    }
                    else
                    {
                        Kernel.wallpaper = Kernel.wallpapernew;
                        wallpapernum = 1;
                    }
                    Msg.Main("Successfully changed background", false);
                    break;
                case "cursorwhite":
                    Kernel.cursor = new Bitmap(Kernel.CursorWhiteIcon);
                    Kernel.cursorload = new Bitmap(Kernel.CursorWhiteLoad);
                    Msg.Main("Successfully changed cursor theme", false);
                    break;
                case "cursordark":
                    Kernel.cursor = new Bitmap(Kernel.CursorIcon);
                    Kernel.cursorload = new Bitmap(Kernel.CursorLoad);
                    Msg.Main("Successfully changed cursor theme", false);
                    break;
                case "default":
                    Kernel.SystemPen.Color = Color.SteelBlue;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "red":
                    Kernel.SystemPen.Color = Color.DarkRed;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "green":
                    Kernel.SystemPen.Color = Color.Green;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "yellow":
                    Kernel.SystemPen.Color = Color.Goldenrod;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "darkblue":
                    Kernel.SystemPen.Color = Color.MidnightBlue;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "gray":
                    Kernel.SystemPen.Color = Color.FromArgb(40, 40, 40);
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "black":
                    Kernel.SystemPen.Color = Color.Black;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                default:
                    Msg.Main("Error, type settings for help", true);
                    break;
            }
            Kernel.Loading = false;
        }
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.GrayPen, StartX, StartY, x, y);
            Kernel.canvas.DrawString("Cursor", Kernel.font, Kernel.WhitePen, StartX+5, StartY+5);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + 60, StartY + 11, StartX + x, StartY + 11);
            Kernel.canvas.DrawString("Wallpaper", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 50);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + 85, StartY + 56, StartX + x, StartY + 56);
            Kernel.canvas.DrawString("Colours", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 85);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + 68, StartY + 91, StartX + x, StartY + 91);
            Bitmap cursor1 = new (Kernel.CursorIcon);
            Bitmap cursor2 = new (Kernel.CursorWhiteIcon);
            Kernel.canvas.DrawImageAlpha(cursor1, StartX+15, StartY+25);
            Kernel.canvas.DrawImageAlpha(cursor2, StartX+50, StartY+25);
            if (Kernel.Pressed && MouseManager.Y > StartY + 25 && MouseManager.Y < StartY + 40)
            {
                if(MouseManager.X > StartX + 15 && MouseManager.X < StartX + 35)
                {
                    Change("cursordark");
                }else if(MouseManager.X > StartX + 50 && MouseManager.X < StartX + 70){
                    Change("cursorwhite");
                }
            }
            if (MouseManager.X > StartX + 5 && MouseManager.X < StartX + 128 && MouseManager.Y > StartY+70 && MouseManager.Y < StartY + 90)
            {
                Kernel.canvas.DrawString("Change background", Kernel.font, Kernel.SystemPen, StartX + 5, StartY + 70);
                if (Kernel.Pressed)
                { Change("wallpaper"); }
            }
            else
            {
                Kernel.canvas.DrawString("Change background", Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 70);
            }
            Kernel.canvas.DrawFilledCircle(Kernel.DefaultPen, StartX + 20, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.RedPen, StartX + 60, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.GreenPen, StartX + 100, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.YellowPen, StartX + 140, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.DarkBluePen, StartX + 180, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.DarkGrayPen, StartX + 220, StartY + 120, 15);
            Kernel.canvas.DrawFilledCircle(Kernel.DarkPen, StartX + 260, StartY + 120, 15);
            if(Kernel.Pressed && MouseManager.Y > StartY + 105 && MouseManager.Y < StartY + 135)
            {
                if(MouseManager.X > StartX + 5 && MouseManager.X < StartX + 35) { Change("default"); }
                if(MouseManager.X > StartX + 45 && MouseManager.X < StartX + 75) { Change("red"); }
                if(MouseManager.X > StartX + 85 && MouseManager.X < StartX + 115) { Change("green"); }
                if(MouseManager.X > StartX + 125 && MouseManager.X < StartX + 155) { Change("yellow"); }
                if(MouseManager.X > StartX + 165 && MouseManager.X < StartX + 195) { Change("darkblue"); }
                if(MouseManager.X > StartX + 205 && MouseManager.X < StartX + 235) { Change("gray"); }
                if(MouseManager.X > StartX + 245 && MouseManager.X < StartX + 275) { Change("black"); }
            }
        }
    }
}