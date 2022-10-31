using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
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
        public static string CurrentVersion = "NclearOS 2 Internal Pre-Alpha Version 0.1";

        public static uint screenX = 1280;
        public static uint screenY = 720;
        public static int fps;
        private static int fps2;
        private static int frames;
        public static bool Lock = true;
        public static bool Pressed;
        public static bool debug;
        private static bool OneClick;
        public static bool LongPress;
        public static bool Loading;
        public static bool WallpaperOn = true;
        public static bool Running = true;
        public static int ExecuteError;

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

        public static Pen WhitePen = new Pen(Color.White);
        public static Pen GrayPen = new Pen(Color.Gray);
        public static Pen DarkPen = new Pen(Color.Black);
        public static Pen DarkGrayPen = new Pen(Color.FromArgb(40, 40, 40));

        public static Pen RedPen = new Pen(Color.DarkRed);
        public static Pen Red2Pen = new Pen(Color.Red);
        public static Pen GreenPen = new Pen(Color.Green);
        public static Pen YellowPen = new Pen(Color.Goldenrod);
        public static Pen BluePen = new Pen(Color.SteelBlue);
        public static Pen DarkBluePen = new Pen(Color.MidnightBlue);
        public static Pen DefaultPen = new Pen(Color.SteelBlue);

        public static Pen SystemPen = new Pen(Color.SteelBlue);

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
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.zap-vga16.psf")]
        public static byte[] Font;

        public static int FreeCount = 0;

        public static uint WallpaperOld { get; internal set; }

        protected override void BeforeRun()
        {
            System.Console.CursorVisible = false;
            System.Console.Clear();
            System.Console.CursorLeft = 35;
            System.Console.CursorTop = 11;
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("NclearOS");
            System.Console.Beep(300, 200);
            Thread.Sleep(300);
            System.Console.Beep(400, 100);
            Thread.Sleep(100);
            System.Console.Beep(500, 100);
            Thread.Sleep(100);
            System.Console.Beep(600, 100);
            Thread.Sleep(100);
            System.Console.Beep(700, 100);
            Files.Init();

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
            font = PCScreenFont.LoadFont(Kernel.Font);

            try
            {
                System.Console.ResetColor();
                System.Console.CursorTop = System.Console.WindowHeight - 1;
                System.Console.Write("Initializing canvas at 1280x720@32...");
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode((int)screenX, (int)screenY, ColorDepth.ColorDepth32));
                canvas.Clear(Color.DimGray);
                canvas.Display();
            }
            catch (Exception e)
            {
                System.Console.ResetColor();
                System.Console.Clear();
                System.Console.Write("Failed to initialize canvas at 1280x720@32\n" + e + "\nAvailalble modes: ");
                foreach (var i in Kernel.canvas.AvailableModes)
                {
                    System.Console.WriteLine(i);
                }
                System.Console.WriteLine("Initializing default mode: " + canvas.DefaultGraphicMode);
                try { canvas = FullScreenCanvas.GetFullScreenCanvas(canvas.DefaultGraphicMode); }
                catch (Exception e2)
                {
                    System.Console.WriteLine("Failed to initialize canvas: " + e2 + "\nPress any key to shutdown system");
                    System.Console.ReadKey();
                    Sys.Power.Shutdown();
                    ACPI.Shutdown();
                    System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                    System.Console.CursorVisible = false;
                    System.Console.Clear();
                    System.Console.SetCursorPosition(18, System.Console.CursorTop = System.Console.WindowHeight / 2 - 1);
                    System.Console.WriteLine("It is now safe to turn off computer.");
                    while (true) ;
                }
            }

            MouseManager.ScreenWidth = screenX - 1;
            MouseManager.ScreenHeight = screenY;
            /*catch (Exception e)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("\n");
                System.Console.SetCursorPosition((System.Console.WindowWidth - "Critical Error".Length) / 2, System.Console.CursorTop);
                System.Console.WriteLine("Critical Error\n");
                System.Console.SetCursorPosition((System.Console.WindowWidth - (e + "").Length) / 2, System.Console.CursorTop);
                System.Console.WriteLine(e);
            }*/
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
            if (fps2 != RTC.Second)
            {
                fps = frames;
                frames = 0;
                fps2 = RTC.Second;
            }

            frames++;
            FreeCount = Heap.Collect();

            switch (MouseManager.MouseState)
            {
                case MouseState.Left or MouseState.Right:
                    if (OneClick)
                    {
                        Pressed = false;
                    }
                    else
                    {
                        Msg.displaymsg = false;
                        OneClick = true;
                        Pressed = true;
                    }
                    LongPress = true;
                    break;
                case MouseState.None:
                    OneClick = false;
                    LongPress = false;
                    Pressed = false;
                    break;
            }

            if (Lock)
            {
                LockScreen.Update();
            }
            else
            {
                if (WallpaperOn) { canvas.DrawImage(wallpaper, 0, 0); } else { canvas.Clear(Color.CadetBlue); }
                Window.Update();
                Input.Update();
                Menu.Update();
                if (Window.display) { canvas.DrawImageAlpha(Window.icon, Window.StartWindowX + 5, Window.StartWindowY + 3); } //to make sure icon is visible when behind Menu (sort of minimized state)
            }
            Msg.Update();
            canvas.DrawString(Convert.ToString(fps), font, WhitePen, (int)screenX - 30, 0);
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
            }
            DrawCursor(MouseManager.X, MouseManager.Y);
            canvas.Display();
        }
        public static void DrawCursor(uint x, uint y)
        {
            if (Loading) { canvas.DrawImageAlpha(cursorload, (int)x, (int)y); } else { canvas.DrawImageAlpha(cursor, (int)x, (int)y); }
        }
        public static void ShutdownPC(bool restart)
        {
            Kernel.Loading = true;
            if (restart)
            {
                Msg.Main("Restarting...", false);
            }
            else
            {
                Msg.Main("Shutting down...", false);
            }
            Kernel.Refresh();
            Kernel.Running = false;
            System.Console.Beep(700, 100);
            System.Console.Beep(600, 100);
            System.Console.Beep(500, 100);
            System.Console.Beep(400, 100);
            System.Console.Beep(300, 100);
            System.Console.Beep(300, 100);
            Thread.Sleep(100);
            if (restart)
            {
                Cosmos.System.Power.Reboot();
                ACPI.Reboot();
            }
            else
            {
                Cosmos.System.Power.Shutdown();
                ACPI.Shutdown();
                canvas.Clear();
                canvas.DrawString("It is now safe to turn off computer.", font, YellowPen, (int)(Kernel.screenX - "It is now safe to turn off computer.".Length * 8) / 2, (int)(Kernel.screenY) / 2 - 8);
                canvas.Display();
            }
        }
    }
}