using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Microsoft.VisualBasic;
using NclearOS2.Commands;
using System;
using System.Drawing;
using System.Threading;
using Display = NclearOS2.GUI.GUI;
using System.Collections.Generic;
using System.Linq;

namespace NclearOS2
{
    public class TextMode
    {
        private static bool useGUI = true;
        private static string err = "OK";

        private static string input;
        private static CommandShell shell;
        private static List<string> history = new List<string>();
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.CursorTop = Console.WindowHeight - 1;
            Console.Write("Hold Shift for advanced options");
            //Console.CursorVisible = true;
            Console.Beep(300, 200);
            Thread.Sleep(50);
            Console.Beep(400, 100);
            Thread.Sleep(20);
            Console.Beep(500, 100);
            Thread.Sleep(20);
            Console.Beep(600, 100);
            Thread.Sleep(20);
            Console.Beep(700, 100);

            Console.CursorLeft = 0;
            Console.Write("                               ");
            Console.CursorLeft = 0;
            Console.ResetColor();
            //Console.CursorVisible = false;

            if (Sys.KeyboardManager.ShiftPressed) { Console.Beep(); BootMenu(); }

            CommandManager.Register();

            if (Kernel.safeMode) { Sysinfo.CPUname = "Unknown"; }
            else
            {
                Sysinfo.CPUname = CPU.GetCPUBrandString();
                if (Kernel.useDisks) { FileManager.Start(); }
            }

            while (true)
            {
                if (useGUI)
                {
                    err = Display.Init();
                    if (err == "OK") { return; } else { useGUI = false; BootMenu(); }
                }
                else { return; }
            }
        }
        public static void BootMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.CursorLeft = (Console.WindowWidth - Kernel.OSVERSION.Length + 4) / 2;
                Console.WriteLine("  " + Kernel.OSVERSION + "  ");
                Console.ResetColor();
                if (err != "OK") { Console.WriteLine("Failed to initialize GUI\n" + err); }

                Console.Write("\nESC - Shutdown System\nEnter - Start System Normally\nQ - GUI Safe Mode\nC - Console (Text) Mode\n\nSelect option: ");
                try
                {
                    ConsoleKeyInfo cki = Console.ReadKey();
                    switch (cki.Key)
                    {
                        case ConsoleKey.Escape:
                            Kernel.Shutdown();
                            while (true);
                        case ConsoleKey.Q:
                            Kernel.safeMode = true;
                            Kernel.useDisks = false;
                            useGUI = true;
                            return;
                        case ConsoleKey.Enter:
                            useGUI = true;
                            return;
                        case ConsoleKey.C:
                            useGUI = false;
                            return;
                    }
                }
                catch (Exception e)
                {
                    err = Convert.ToString(e);
                }
            }
        }
        public static void ConsoleMode()
        {
            if(shell == null)
            {
                shell = new CommandShell { crashClient = ExecuteError, update = Result, clearScreen = Clear };
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.ResetColor();
                //Console.CursorVisible = true;
                Console.WriteLine(Kernel.OSVERSION + "\n");
            }
            try { if (shell.Execute(Input()) == 2) { shell.Print += "Wrong parameter"; } }
            catch (Exception e) { shell = null; Console.WriteLine("Command Shell crashed: " + e.Message + "; Press Enter to restart."); Console.ReadLine(); }
            Heap.Collect();
        }
        private static void ExecuteError()
        {
            throw new Exception("Manual crash");
        }
        private static void Result()
        {
            string[] strings = shell.Print.Split('\n');

            if (strings.Length > Console.WindowHeight - 1)
            {
                int i = 0;
                foreach(string s in strings)
                {
                    i++;
                    Console.WriteLine(s);
                    if (i + 4 > Console.WindowHeight)
                    {
                        ConsoleColor consoleColor = Console.ForegroundColor;
                        ConsoleColor consoleColorBg = Console.BackgroundColor;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("Press Enter to continue...");
                        Console.ReadLine();
                        Console.CursorTop--;
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("---                       ");
                        Console.ForegroundColor = consoleColor;
                        Console.BackgroundColor = consoleColorBg;
                        i = 0;
                    }
                }
            }
            else Console.WriteLine(shell.Print);
        }
        private static void Clear()
        {
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.ResetColor();
            //Console.CursorVisible = true;
            Console.WriteLine(Kernel.OSVERSION + "\n");
        }
        private static string Input()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + shell.prompt + " ");
            input = "";
            int position = 0;
            while (true)
            {
                ConsoleKeyInfo name = Console.ReadKey();
                switch (name.Key)
                {
                    case ConsoleKey.Delete:
                        if (Sys.KeyboardManager.ControlPressed && Sys.KeyboardManager.AltPressed) { Cosmos.System.Power.Reboot(); }
                        break;
                    case ConsoleKey.Escape:
                        Console.ResetColor();
                        Console.WriteLine();
                        return null;
                    case ConsoleKey.Enter:
                        Console.ResetColor();
                        Console.WriteLine();
                        history.Add(input);
                        return input;
                    case ConsoleKey.UpArrow:
                        if (history.Count - position > 0)
                        {
                            position++;
                            Console.CursorLeft = 0;
                            Console.Write(new string(' ', Console.WindowWidth - 1));
                            Console.CursorTop--;
                            Console.Write("\n" + shell.prompt + " ");
                            input = history[history.Count - position];
                            Console.Write(input);
                        }
                        break;
                    case ConsoleKey.F1:
                        Console.ResetColor();
                        Console.WriteLine();
                        return "help";
                    case ConsoleKey.DownArrow:
                        if (position > 1)
                        {
                            position--;
                            Console.CursorLeft = 0;
                            Console.Write(new string(' ', Console.WindowWidth - 1));
                            Console.CursorTop--;
                            Console.Write("\n" + shell.prompt + " ");
                            input = history[history.Count - position];
                            Console.Write(input);
                        }
                        break;
                    case ConsoleKey.Backspace:
                        if (Console.CursorLeft > shell.prompt.Length + 1)
                        {
                            Console.CursorLeft--;
                            Console.Write(" ");
                            input = input.Remove(input.Length - 1);
                            Console.CursorLeft--;
                        }
                        break;
                    default:
                        input += name.KeyChar;
                        break;
                }
            }
        }
        public static void MsgInCenter(string msg)
        {
            Console.SetCursorPosition((Console.WindowWidth - msg.Length) / 2, Console.WindowHeight / 2 - 1);
            Console.Write(msg);
        }
    }
}