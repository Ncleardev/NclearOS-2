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
            
            if (useGUI)
            {
                err = Display.Init();
                if (err != "OK") { useGUI = false; BootMenu(); }
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
                            while (true) ;
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
                shell = new CommandShell { crashClient = ExecuteError, update = Result };
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.ResetColor();
                //Console.CursorVisible = true;
                Console.WriteLine(Kernel.OSVERSION + "\n");
            }
            //yPos = Console.CursorTop + 1;
            shell.Execute(Input());
            Heap.Collect();
        }
        private static void ExecuteError()
        {
            throw new Exception("Manual crash");
        }
        private static void Result()
        {
            Console.WriteLine(shell.print);
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
                            Console.Write("\n" + shell.prompt);
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
    }
}