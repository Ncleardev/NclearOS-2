using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Drawing;
using System.Threading;
using Sys = Cosmos.System;

namespace NclearOS2
{
    public class Kernel : Sys.Kernel
    {
        public static string CurrentVersion = "NclearOS 2 Version Alpha 0.3 Pre-release";

        public static uint screenX = 1280;
        public static uint screenY = 720;
        public static int colorDepth = 32;
        public static Mode DisplayMode;
        private static bool customRes = false;

        public static int fps;
        private static int fps2;
        private static int frames;

        private static uint oldXMouse;
        private static uint oldYMouse;
        public static bool userInactivity;
        public static int userInactivityTime = -1;

        public static bool Lock = true;
        public static bool screenSaver;
        public static bool debug;
        public static int ExecuteError;
        public static bool WallpaperOn = true;
        public static bool Running = true;

        public static bool Pressed;
        private static bool OneClick;
        private static bool StartOneClick;
        public static bool StartClick;
        public static bool LongPress;
        public static bool Loading;
        public static bool HideCursor;

        public static bool UseDisks = true;
        public static bool network;
        public static bool safeMode = false;

        public static Canvas canvas;

        public static Color WhitePen = Color.White;
        public static Color GrayPen = Color.Gray;
        public static Color DarkPen = Color.Black;
        public static Color DarkGrayPen = Color.FromArgb(40, 40, 40);
        public static Color RedPen = Color.DarkRed;
        public static Color Red2Pen = Color.Red;
        public static Color GreenPen = Color.Green;
        public static Color YellowPen = Color.Goldenrod;
        public static Color BluePen = Color.SteelBlue;
        public static Color DarkBluePen = Color.MidnightBlue;
        public static Color DefaultPen = Color.SteelBlue;
        public static Color SystemPen = Color.SteelBlue;

        public static PCScreenFont font;

        protected override void BeforeRun()
        {
            System.Console.Clear();
            System.Console.CursorVisible = false;
            System.Console.CursorLeft = 35;
            System.Console.CursorTop = 11;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("NclearOS");
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.CursorTop = System.Console.WindowHeight - 1;
            System.Console.Write("Hold Shift for advanced options");
            System.Console.CursorVisible = true;
            System.Console.Beep(300, 200);
            Thread.Sleep(100);
            System.Console.Beep(400, 100);
            Thread.Sleep(50);
            System.Console.Beep(500, 100);
            Thread.Sleep(50);
            System.Console.Beep(600, 100);
            Thread.Sleep(50);
            System.Console.Beep(700, 100);
            if (KeyboardManager.ShiftPressed) { System.Console.Beep(); SafeMode(); }
            System.Console.CursorVisible = false;
            System.Console.CursorLeft = 0;
            System.Console.Write("                               ");
            System.Console.CursorLeft = 0;
            
            if (UseDisks) { UseDisks = false; FileManager.Start(); }
            Resources.InitResources();
            font = PCScreenFont.Default;

            if (!customRes)
            {
                string res = Convert.ToString(screenX + "x" + screenY + "@" + colorDepth);
                if (!safeMode && UseDisks)
                {
                    res = Profiles.LoadSystem() ?? res;
                }
                string initRes = SetRes(res, true);
                if (!initRes.Contains("Success")) { SafeMode(initRes); }
            }

            canvas.Clear();
            if (safeMode)
            {
                WallpaperOn = false;
                Toast.Display("Starting Input Manager Service...");
                canvas.Display();
                ProcessManager.Run(new GlobalInput());
                Thread.Sleep(300);
                canvas.Clear();
            }
            else
            {
                canvas.DrawImage(Resources.wallpaperlock, 0, 0);
                Toast.Display("Starting services...");
                canvas.Display();

                ProcessManager.Run(new GlobalInput());
                ProcessManager.Run(new Net());

                Thread.Sleep(300);
                Toast.Display("Loading user settings...");
                canvas.Display();
                Profiles.LoadUser();
                Thread.Sleep(300);
                
                canvas.Clear();
                canvas.DrawImage(Resources.wallpaperlock, 0, 0);
            }
            Toast.Force("Welcome");
        }
        protected override void Run()
        {
            if (Running)
            {
                try
                { Refresh(); }
                catch (Exception e)
                {
                    try
                    {
                        Refresh();
                        Msg.Main("Error", "System " + e, Resources.error);
                    }
                    catch
                    {
                        Running = false;
                        canvas.Disable();
                        System.Console.BackgroundColor = ConsoleColor.DarkBlue;
                        System.Console.Clear();
                        System.Console.CursorVisible = true;
                        System.Console.BackgroundColor = ConsoleColor.White;
                        System.Console.ForegroundColor = ConsoleColor.DarkBlue;
                        System.Console.CursorTop = System.Console.WindowHeight / 3;
                        System.Console.CursorLeft = (System.Console.WindowWidth - CurrentVersion.Length - 4) / 2;
                        System.Console.WriteLine("  " + CurrentVersion + "  ");
                        System.Console.BackgroundColor = ConsoleColor.DarkBlue;
                        System.Console.ForegroundColor = ConsoleColor.White;
                        System.Console.WriteLine();
                        System.Console.WriteLine();
                        System.Console.CursorLeft = (System.Console.WindowWidth - ("Fatal System " + e).Length) / 2;
                        System.Console.WriteLine("Fatal System " + e);
                        System.Console.WriteLine();
                        System.Console.CursorLeft = (System.Console.WindowWidth - "Press any key to restart computer ".Length) / 2;
                        System.Console.Write("Press any key to restart computer ");
                        System.Console.ReadKey(true);
                        Sys.Power.Reboot();
                        ACPI.Reboot();
                    }
                }
            }
        }
        public static void Refresh()
        {
            switch (MouseManager.MouseState)
            {
                case MouseState.Left or MouseState.Right:
                    if (StartOneClick) { StartClick = false; }
                    else { StartOneClick = true; StartClick = true; }
                    OneClick = true;
                    LongPress = true;
                    userInactivity = false;
                    userInactivityTime = -1;
                    break;
                case MouseState.None:
                    if (OneClick)
                    {
                        Pressed = true;
                        OneClick = false;
                        Toast.msg = null;
                    }
                    else
                    {
                        Pressed = false;
                    }
                    StartOneClick = false;
                    StartClick = false;
                    LongPress = false;
                    break;
            }

            if (screenSaver) { ScreenSaver.Update(); }
            else
            {
                if (fps2 != RTC.Second)
                {
                    fps = frames;
                    frames = 0;
                    fps2 = RTC.Second;
                }
                frames++;

                if (MouseManager.X == oldXMouse || MouseManager.Y == oldYMouse)
                { userInactivity = true; }
                else
                {
                    oldXMouse = MouseManager.X;
                    oldYMouse = MouseManager.Y;
                    userInactivity = false;
                    userInactivityTime = -1;
                }
                ProcessManager.RefreshService();
                if (Lock) { LockScreen.Update(); }
                else
                {
                    if (WallpaperOn) { canvas.DrawImage(Resources.wallpaper, 0, 0); } else { canvas.Clear(Color.CadetBlue); }
                    ProcessManager.Refresh();
                    Animation.Refresh();
                    Menu.Update();
                }
                if (userInactivity)
                {
                    if (userInactivityTime == -1)
                    {
                        if (RTC.Second < 30)
                        {
                            userInactivityTime = 59;
                        }
                        else
                        {
                            userInactivityTime = 29;
                        }
                    }
                    else
                    {
                        if (RTC.Second == userInactivityTime)
                        {
                            Kernel.canvas.Clear();
                            screenSaver = true;
                            Lock = true;
                            HideCursor = true;
                            return;
                        }
                    }
                }
                canvas.DrawString(Convert.ToString(fps), font, WhitePen, (int)screenX - 30, 0);
            }
            if (debug)
            {
                if (ExecuteError == 1) { ExecuteError = 0; throw new Exception("Manual crash"); }
                else if (ExecuteError == 2) { throw new Exception("Manual crash"); }
                canvas.DrawString(Convert.ToString(MouseManager.X), font, WhitePen, (int)screenX - 50, 15);
                canvas.DrawString(Convert.ToString(MouseManager.Y), font, WhitePen, (int)screenX - 50, 30);
                canvas.DrawString(Convert.ToString(OneClick), font, WhitePen, (int)screenX - 50, 45);
                canvas.DrawString(Convert.ToString(Pressed), font, WhitePen, (int)screenX - 50, 60);
                canvas.DrawString(Convert.ToString(LongPress), font, WhitePen, (int)screenX - 50, 75);
                canvas.DrawString(Convert.ToString(Loading), font, WhitePen, (int)screenX - 50, 90);
                Graphic.TextView(Sysinfo.Main(), 10, 10, WhitePen);
            }
            Toast.Update();
            if (!HideCursor) { if (Loading) { canvas.DrawImageAlpha(Resources.cursorload, (int)MouseManager.X, (int)MouseManager.Y); } else { canvas.DrawImageAlpha(Resources.cursor, (int)MouseManager.X, (int)MouseManager.Y); } }
            Heap.Collect();
            canvas.Display();
        }
        public static void ShutdownPC(bool restart)
        {
            canvas.Clear();
            if (WallpaperOn) { canvas.DrawImage(Resources.wallpaperlock, 0, 0); }
            if (Kernel.UseDisks && !safeMode)
            {
                Toast.Display("Saving user settings...");
                canvas.Display();
                Profiles.Save();
                canvas.Clear();
                if (WallpaperOn) { canvas.DrawImage(Resources.wallpaperlock, 0, 0); }
            }
            Kernel.Running = false;
            if (restart)
            {
                Toast.Display("Restarting...");
                canvas.Display();
                System.Console.Beep(700, 100);
                System.Console.Beep(600, 100);
                System.Console.Beep(500, 100);
                System.Console.Beep(400, 300);
                Thread.Sleep(100);
                Sys.Power.Reboot();
            }
            else
            {
                Toast.Display("Shutting down...");
                canvas.Display();
                System.Console.Beep(700, 100);
                System.Console.Beep(600, 100);
                System.Console.Beep(500, 100);
                System.Console.Beep(400, 100);
                System.Console.Beep(300, 200);
                Thread.Sleep(100);
                Sys.Power.Shutdown();
                canvas.Clear();
                canvas.DrawString("It is now safe to turn off computer.", font, YellowPen, (int)(Kernel.screenX - "It is now safe to turn off computer.".Length * 8) / 2, (int)(Kernel.screenY) / 2 - 8);
                while (true) { canvas.Display(); }
            }
        }
        public static void SafeMode(string err = "ok")
        {
            System.Console.ResetColor();
            while (true)
            {
                System.Console.Clear();
                System.Console.ForegroundColor = ConsoleColor.Black;
                System.Console.BackgroundColor = ConsoleColor.White;
                System.Console.CursorLeft = (System.Console.WindowWidth - CurrentVersion.Length + 4) / 2;
                System.Console.WriteLine("  " + CurrentVersion + "  ");
                System.Console.ResetColor();
                if (err != "ok") { System.Console.WriteLine("Failed to initialize canvas\n" + err); }

                System.Console.Write("\nESC - Shutdown System\n\nQ - Safe Mode\nA - Safe Mode - custom resolution\nZ - Safe Mode with File System\nX - Safe Mode with File System - custom resolution\n\nEnter - Start System Normally\nSpace - Start System Normally - custom resolution\n\nSelect option: ");
                try
                {
                    ConsoleKeyInfo cki = System.Console.ReadKey();
                    switch (cki.Key)
                    {
                        case ConsoleKey.Escape:
                            Sys.Power.Shutdown();
                            ACPI.Shutdown();
                            System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                            System.Console.CursorVisible = false;
                            System.Console.Clear();
                            System.Console.SetCursorPosition(18, System.Console.CursorTop = System.Console.WindowHeight / 2 - 1);
                            System.Console.WriteLine("It is now safe to turn off computer.");
                            while (true) ;
                        case ConsoleKey.Q:
                            UseDisks = false;
                            safeMode = true;
                            SetRes("800x600@32");
                            return;
                        case ConsoleKey.Z:
                            safeMode = true;
                            SetRes("800x600@32");
                            return;
                        case ConsoleKey.A:
                            UseDisks = false;
                            safeMode = true;
                            System.Console.Write("\n\nType WidthxHeight@ColorDepth\tExample: 1920x1080@32: ");
                            SetRes(System.Console.ReadLine());
                            return;
                        case ConsoleKey.X:
                            safeMode = true;
                            System.Console.Write("\n\nType WidthxHeight@ColorDepth\tExample: 1920x1080@32: ");
                            SetRes(System.Console.ReadLine());
                            return;
                        case ConsoleKey.Enter:
                            SetRes("1280x720@32");
                            return;
                        case ConsoleKey.Spacebar:
                            System.Console.Write("\n\nType WidthxHeight@ColorDepth\tExample: 1920x1080@32: ");
                            SetRes(System.Console.ReadLine());
                            return;
                    }
                }
                catch (Exception e)
                {
                    err = Convert.ToString(e);
                }

            }
        }
        public static string SetRes(string res, bool isInit = false)
        {
            if (res.Contains('x'))
            {
                ColorDepth colorss;
                uint x;
                uint y;
                if (res.Contains('@'))
                {
                    string[] split = res.Split('@');
                    colorss = GetColorDepth(split[1]);
                    x = Convert.ToUInt32(split[0].Split('x')[0]);
                    y = Convert.ToUInt32(split[0].Split('x')[1]);
                    colorDepth = Convert.ToInt32(split[1]);
                }
                else
                {
                    x = Convert.ToUInt32(res.Split('x')[0]);
                    y = Convert.ToUInt32(res.Split('x')[1]);
                    colorss = GetColorDepth(Convert.ToString(colorDepth));
                }
                try
                {
                    DisplayMode = new((int)x, (int)y, colorss);
                    canvas = FullScreenCanvas.GetFullScreenCanvas(DisplayMode);
                }
                catch (Exception e)
                {
                    return "Error: Resolution " + x + "x" + y + "@" + colorDepth + " is not available; " + e;
                }
                screenX = x;
                screenY = y;
                MouseManager.ScreenWidth = screenX - 1;
                MouseManager.ScreenHeight = screenY;
                if (!isInit) { customRes = true; }
                return "Successfully changed resolution to " + screenX + "x" + screenY + "@" + colorDepth;
            }
            else { return "Error: 'x' character expected"; }
        }
        public static ColorDepth GetColorDepth(string str)
        {
            return str switch
            {
                "32" => ColorDepth.ColorDepth32,
                "24" => ColorDepth.ColorDepth24,
                "16" => ColorDepth.ColorDepth16,
                "8" => ColorDepth.ColorDepth8,
                "4" => ColorDepth.ColorDepth4,
                _ => ColorDepth.ColorDepth32,
            };
        }
    }
}