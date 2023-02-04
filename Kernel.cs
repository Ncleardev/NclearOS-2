using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.HAL.Drivers.PCI.Video;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using System;
using System.Drawing;
using System.Threading;
using Sys = Cosmos.System;

namespace NclearOS2
{
    public class Kernel : Sys.Kernel
    {
        public static string CurrentVersion = "NclearOS 2 Version Alpha 0.2";

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
        public static bool UseProfile = true;

        public static Canvas canvas;

        public static Bitmap check;
        public static Bitmap close;
        public static Bitmap closered;
        public static Bitmap console;
        public static Bitmap cursor;
        public static Bitmap disk;
        public static Bitmap cursorload;
        public static Bitmap wallpaper;
        public static Bitmap wallpapernew;
        public static Bitmap wallpaperold;
        public static Bitmap wallpaperlock;
        public static Bitmap logo;
        public static Bitmap lockicon;
        public static Bitmap program;
        public static Bitmap reboot;
        public static Bitmap settings;
        public static Bitmap shutdown;
        public static Bitmap start;
        public static Bitmap start2;
        public static Bitmap notepad;
        public static Bitmap filesicon;
        public static Bitmap fileicon;
        public static Bitmap sysinfo;

        public static Pen WhitePen = new(Color.White);
        public static Pen GrayPen = new(Color.Gray);
        public static Pen DarkPen = new(Color.Black);
        public static Pen DarkGrayPen = new(Color.FromArgb(40, 40, 40));

        public static Pen RedPen = new(Color.DarkRed);
        public static Pen Red2Pen = new(Color.Red);
        public static Pen GreenPen = new(Color.Green);
        public static Pen YellowPen = new(Color.Goldenrod);
        public static Pen BluePen = new(Color.SteelBlue);
        public static Pen DarkBluePen = new(Color.MidnightBlue);
        public static Pen DefaultPen = new(Color.SteelBlue);

        public static Pen SystemPen = new(Color.SteelBlue);

        public static PCScreenFont font;

        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.check.bmp")]
        public static byte[] Check;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.close.bmp")]
        public static byte[] CloseButton;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.closered.bmp")]
        public static byte[] CloseRed;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.disk.bmp")]
        public static byte[] Disk;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.files.bmp")]
        public static byte[] FilesIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.file.bmp")]
        public static byte[] FileIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.notepad.bmp")]
        public static byte[] NotepadIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.console.bmp")]
        public static byte[] ConsoleIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.Cursor.bmp")]
        public static byte[] CursorIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.CursorLoad.bmp")]
        public static byte[] CursorLoad;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.CursorWhite.bmp")]
        public static byte[] CursorWhiteIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.CursorWhiteLoad.bmp")]
        public static byte[] CursorWhiteLoad;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.logo.bmp")]
        public static byte[] Logo;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.program.bmp")]
        public static byte[] Program;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.reboot.bmp")]
        public static byte[] Reboot;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.settings.bmp")]
        public static byte[] Settings;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.shutdown.bmp")]
        public static byte[] Shutdown;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.lock.bmp")]
        public static byte[] LockIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.start.bmp")]
        public static byte[] StartButton;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.start2.bmp")]
        public static byte[] Start2;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.sysinfo.bmp")]
        public static byte[] SysInfo;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.WallpaperOld.bmp")]
        public static byte[] WallpaperFile2;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.WallpaperNew.bmp")]
        public static byte[] WallpaperFile;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.WallpaperLock.bmp")]
        public static byte[] WallpaperLock;

        protected override void BeforeRun()
        {
            System.Console.CursorVisible = false;
            System.Console.Clear();
            System.Console.CursorLeft = 35;
            System.Console.CursorTop = 11;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("NclearOS");
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.CursorTop = System.Console.WindowHeight - 1;
            System.Console.Write("Hold Shift for advanced options");
            System.Console.Beep(300, 200);
            Thread.Sleep(200);
            System.Console.Beep(400, 100);
            Thread.Sleep(50);
            System.Console.Beep(500, 100);
            Thread.Sleep(50);
            System.Console.Beep(600, 100);
            Thread.Sleep(50);
            System.Console.Beep(700, 100);
            System.Console.CursorLeft = 0;
            System.Console.WriteLine(new string(' ', System.Console.WindowWidth));
            if (KeyboardManager.ShiftPressed) { System.Console.Beep(); SafeMode("ok"); }
            if (UseDisks)
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write("| .. |");
                System.Console.ResetColor();
                System.Console.Write(" File System");
                System.Console.CursorLeft = 0;
                string temp = Files.Init();
                if (temp.Contains("Success"))
                {
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.Write("| OK |");
                    System.Console.ResetColor();
                    System.Console.Write(" File System \n");
                }
                else
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.Write("| ERR |");
                    System.Console.ResetColor();
                    System.Console.Write(" File System \n");
                    System.Console.WriteLine(temp + "; \nFile System is now disabled.\nPress any key to continue");
                    System.Console.ReadKey(true);
                }
            }

            cursor = new Bitmap(CursorIcon);
            check = new Bitmap(Check);
            cursorload = new Bitmap(CursorLoad);
            notepad = new Bitmap(NotepadIcon);
            logo = new Bitmap(Logo);
            program = new Bitmap(Program);
            settings = new Bitmap(Settings);
            shutdown = new Bitmap(Shutdown);
            wallpapernew = new Bitmap(WallpaperFile);
            wallpaper = wallpapernew;
            wallpaperold = new Bitmap(WallpaperFile2);
            wallpaperlock = new Bitmap(WallpaperLock);
            start = new Bitmap(StartButton);
            start2 = new Bitmap(Start2);
            close = new Bitmap(CloseButton);
            closered = new Bitmap(CloseRed);
            console = new Bitmap(ConsoleIcon);
            reboot = new Bitmap(Reboot);
            lockicon = new Bitmap(LockIcon);
            filesicon = new Bitmap(FilesIcon);
            disk = new Bitmap(Disk);
            fileicon = new Bitmap(FileIcon);
            sysinfo = new Bitmap(SysInfo);
            font = Cosmos.System.Graphics.Fonts.PCScreenFont.Default;
            if (!customRes)
            {
                string initRes = SetRes(Convert.ToString(screenX + "x" + screenY + "@" + colorDepth), true);
                if (!initRes.Contains("Success")) { SafeMode(initRes); }
                if (UseProfile && UseDisks)
                {
                    string loadRes = Profiles.Load();
                    if (loadRes != null)
                    {
                        string applyRes = SetRes(loadRes);
                        if (!applyRes.Contains("Success")) { Msg.Main(applyRes, true); }
                    }
                }
                else
                { UseProfile = false; }
            }
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
                        Msg.Main("System " + e + "; Press any key to continue", true);
                        Msg.Update();
                        canvas.Display();
                        System.Console.ReadKey();
                        Refresh();
                    }
                    catch
                    {
                        Running = false;
                        canvas.Clear();
                        canvas.DrawString(CurrentVersion, font, Red2Pen, (int)(Kernel.screenX - CurrentVersion.Length * 8) / 2, 10);
                        canvas.DrawString("Critical System Error", font, Red2Pen, (int)(Kernel.screenX - "Critical System Error".Length * 8) / 2, 50);
                        canvas.DrawString("" + e, font, Red2Pen, (int)(Kernel.screenX - ("" + e).Length * 8) / 2, 70);
                        canvas.DrawString("Press any key to restart computer...", font, Red2Pen, (int)(Kernel.screenX - "Press any key to restart computer...".Length * 8) / 2, 100);
                        canvas.Display();
                        System.Console.ReadKey();
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
                    Msg.displaymsg = false;
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

                if (Lock) { LockScreen.Update(); }
                else
                {
                    if (KeyboardManager.TryReadKey(out KeyEvent keyEvent)) { Input.Update(keyEvent); userInactivity = false; userInactivityTime = -1; }
                    if (WallpaperOn) { canvas.DrawImage(wallpaper, 0, 0); } else { canvas.Clear(Color.CadetBlue); }
                    if (Window.display) { Window.Update(); }
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
                Msg.Update();
                canvas.DrawString(Convert.ToString(fps), font, WhitePen, (int)screenX - 30, 0);
            } 
            if (debug)
            {
                if (ExecuteError == 1) { ExecuteError = 0; Convert.ToInt32("bruh"); }
                else if (ExecuteError == 2) { Convert.ToInt32("bruh"); }
                canvas.DrawString(Convert.ToString(MouseManager.X), font, WhitePen, (int)screenX - 50, 15);
                canvas.DrawString(Convert.ToString(MouseManager.Y), font, WhitePen, (int)screenX - 50, 30);
                canvas.DrawString(Convert.ToString(OneClick), font, WhitePen, (int)screenX - 50, 45);
                canvas.DrawString(Convert.ToString(Pressed), font, WhitePen, (int)screenX - 50, 60);
                canvas.DrawString(Convert.ToString(LongPress), font, WhitePen, (int)screenX - 50, 75);
                canvas.DrawString(Convert.ToString(Loading), font, WhitePen, (int)screenX - 50, 90);
                Graphic.DisplayText(Sysinfo.Main(), 10, 10, WhitePen);
            }
            if (!HideCursor) { DrawCursor(MouseManager.X, MouseManager.Y); }
            canvas.Display();
            Heap.Collect();
        }
        public static void DrawCursor(uint x, uint y)
        {
            if (Loading) { canvas.DrawImageAlpha(cursorload, (int)x, (int)y); } else { canvas.DrawImageAlpha(cursor, (int)x, (int)y); }
        }
        public static void ShutdownPC(bool restart)
        {
            Kernel.Loading = true;
            if (Kernel.UseDisks && Kernel.UseProfile) { Msg.Main("Saving user settings...", false); Kernel.Refresh(); Profiles.Save(); }
            Kernel.Running = false;
            if (restart)
            {
                Msg.Main("Restarting...", false);
                canvas.Clear();
                Msg.Update();
                canvas.Display();
                System.Console.Beep(700, 100);
                System.Console.Beep(600, 100);
                System.Console.Beep(500, 100);
                System.Console.Beep(400, 300);
                Thread.Sleep(100);
                Sys.Power.Reboot();
                ACPI.Reboot();
            }
            else
            {
                Msg.Main("Shutting down...", false);
                canvas.Clear();
                Msg.Update();
                canvas.Display();
                System.Console.Beep(700, 100);
                System.Console.Beep(600, 100);
                System.Console.Beep(500, 100);
                System.Console.Beep(400, 100);
                System.Console.Beep(300, 200);
                Thread.Sleep(100);
                Sys.Power.Shutdown();
                ACPI.Shutdown();
                canvas.Clear();
                canvas.DrawString("It is now safe to turn off computer.", font, YellowPen, (int)(Kernel.screenX - "It is now safe to turn off computer.".Length * 8) / 2, (int)(Kernel.screenY) / 2 - 8);
                while (true) { canvas.Display(); }
            }
        }
        public static void SafeMode(string err)
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
                            UseProfile = false;
                            SetRes("800x600@32");
                            return;
                        case ConsoleKey.Z:
                            UseProfile = false;
                            SetRes("800x600@32");
                            return;
                        case ConsoleKey.A:
                            UseDisks = false;
                            UseProfile = false;
                            System.Console.Write("\n\nType WidthxHeight@ColorDepth\tExample: 1920x1080@32: ");
                            SetRes(System.Console.ReadLine());
                            return;
                        case ConsoleKey.X:
                            UseProfile = false;
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
                    canvas.Clear(Color.DimGray);
                    canvas.Display();
                }
                catch(Exception e)
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
            switch (str)
            {
                case "32":
                    return ColorDepth.ColorDepth32;
                case "24":
                    return ColorDepth.ColorDepth24;
                case "16":
                    return ColorDepth.ColorDepth16;
                case "8":
                    return ColorDepth.ColorDepth8;
                case "4":
                    return ColorDepth.ColorDepth4;
                default:
                    return ColorDepth.ColorDepth32;
            }
        }
    }
}