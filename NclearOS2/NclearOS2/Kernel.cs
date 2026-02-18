using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.System.Graphics;
using NclearOS2.GUI;
using System;
using System.Threading;
using Display = NclearOS2.GUI.GUI;
using Sys = Cosmos.System;

namespace NclearOS2
{
    public class Kernel : Sys.Kernel
    {
        public static readonly string OSVERSION = "NclearOS 2 Version Alpha 0.6.1";
        public static readonly string OSNAME = "NclearOS 2";
        public static string PCNAME = "pc";
        public static readonly string MAINDISK = "0:\\";
        public static readonly string SYSTEMPATH = MAINDISK + "NclearOS\\";
        public static readonly string USERSPATH = SYSTEMPATH + "Users\\";

        public static readonly string PROGRAMSPATH = SYSTEMPATH + "Programs\\";
        public static readonly string PROGRAMSDATAPATH = SYSTEMPATH + "Config\\";
        public static readonly string SYSTEMCONFIG = PROGRAMSDATAPATH + "systemConfig.cfg";

        public static readonly string CURRENTUSER = USERSPATH + "Admin\\";
        public static readonly string USERPROGRAMSPATH = CURRENTUSER + "Programs\\";
        public static readonly string USERPROGRAMSDATAPATH = CURRENTUSER + "Config\\";
        public static readonly string USERCONFIG = USERPROGRAMSDATAPATH + "userConfig.cfg";

        public static bool GUIenabled = false;
        public static bool useDisks = true;
        public static bool useNetwork = true;
        public static bool networkConnected = false;
        public static bool safeMode = false;

        private static bool debug;
        public static bool Debug
        {
            get { return debug; }
            set
            {
                debug = value;
                if (GUIenabled) { if (debug) { InfoDisplay.infoDisplay = ProcessManager.Run(new InfoDisplay()); } else { InfoDisplay.infoDisplay.Exit(); } }
            }
        }
        public static byte ExecuteError;

        public static int[] debugInt;
        public static string[] debugStr;

        protected override void BeforeRun()
        {
            try
            {
                for (int i = 0; i < debugInt.Length; i++)
                {
                    Console.Write("Enter value for debug integer No. " + i + ": ");
                    debugInt[i] = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine();
                }                                                                         //used for debugging
                for (int i = 0; i < debugStr.Length; i++)
                {
                    Console.Write("Enter value for debug string No. " + i + ": ");
                    debugStr[i] = Console.ReadLine();
                    Console.WriteLine();
                }
                Console.Clear();
                //Console.CursorVisible = false;
                Console.CursorLeft = 35;
                Console.CursorTop = 11;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("NclearOS");
                Console.CursorLeft = 0;
                Console.CursorTop = Console.WindowHeight - 1;
                TextMode.Run();
            }
            catch (Exception e)
            {
                SystemCrash(e);
            }

        }
        protected override void Run()
        {
            try
            {
                if (Debug)
                {
                    if (ExecuteError == 1) { ExecuteError = 0; throw new Exception("Manual crash"); }
                    else if (ExecuteError == 2) { throw new Exception("Manual crash"); }
                }
                if (GUIenabled) { Display.Refresh(); }
                else if (Display.canvas != null) { Display.ShutdownGUI(); }
                else { TextMode.ConsoleMode(); }
            }
            catch (Exception e)
            {
                try
                {
                    if (ExecuteError == 2) { throw new Exception("Manual crash"); }
                
                    if (GUIenabled)
                    { Display.Refresh(); Msg.Main("Error", "System " + e, GUI.Icons.error); }
                    else { Console.WriteLine("System " + e); //Run();
                    }
                }
                catch (Exception ex)
                {
                    SystemCrash(ex);
                }
            }
        }

        public static void SystemCrash(Exception e)
        {
            Display.canvas?.Disable();
            //Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.CursorTop = Console.WindowHeight / 3;
            Console.CursorLeft = (Console.WindowWidth - OSVERSION.Length - 4) / 2;
            Console.Write("  " + OSVERSION + "  ");
            Thread.Sleep(500);
            Sys.KeyboardManager.TryReadKey(out _);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.CursorLeft = (Console.WindowWidth - ("Fatal System " + e.ToString()).Length) / 2;
            Console.Write("Fatal System " + e.ToString());
            Thread.Sleep(500);
            Sys.KeyboardManager.TryReadKey(out _);
            Console.WriteLine();
            Console.WriteLine();
            //"Detailed info: " + e.InnerException + " " + e.Data + " " + e.HResult;
            Console.CursorLeft = (Console.WindowWidth - ("Error Code: " + e.HResult).Length) / 2;
            Console.Write("Error Code: " +  e.HResult);
            Thread.Sleep(500);
            Console.WriteLine();
            Console.WriteLine();
            Console.CursorLeft = (Console.WindowWidth - "Press any key to restart computer ".Length) / 2;
            Sys.KeyboardManager.TryReadKey(out _);
            Console.Write("Press any key to restart computer ");
            Console.ReadKey(true);
            Sys.Power.Reboot();
            ACPI.Reboot();
        }
        public static void Shutdown(bool restart = false, byte force = 0)//0: standard, 1: force apps to close, 2: shutdown immediately
        {
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Clear();
            TextMode.MsgInCenter(restart ? "Restarting..." : "Shutting down...");
            if (force == 2) { if (restart) { Sys.Power.Reboot(); } else { Sys.Power.Shutdown(); } }
            else
            {
                if (GUIenabled)
                {
                    if(!Display.ShutdownGUI(restart, false, force > 0)) { return; }
                }
                Console.Beep(700, 100);
                Console.Beep(600, 100);
                Console.Beep(500, 100);
                if (restart)
                {
                    Console.Beep(400, 300);
                    if (GUIenabled) { Display.canvas.Disable(); }
                    Thread.Sleep(100);
                    Sys.Power.Reboot();
                }
                else
                {
                    Console.Beep(400, 100);
                    Console.Beep(300, 200);
                    if (GUIenabled) { Display.canvas.Disable(); }
                    Thread.Sleep(100);
                    Sys.Power.Shutdown();
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Clear();
            TextMode.MsgInCenter("It is now safe to turn off computer.");
            while (true) Thread.Sleep(100);
        }
    }
}