using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Microsoft.VisualBasic;
using NclearOS2.Commands;
using System;
using System.Drawing;
using System.Threading;
using Display = NclearOS2.GUI.GUI;

namespace NclearOS2
{
    public class TextMode
    {
        private static CommandShell shell;
        public static void BootMenu(string err = "ok")
        {
            Console.ResetColor();
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.CursorLeft = (Console.WindowWidth - Kernel.OSVERSION.Length + 4) / 2;
                Console.WriteLine("  " + Kernel.OSVERSION + "  ");
                Console.ResetColor();
                if (err != "ok") { Console.WriteLine("Failed to initialize canvas\n" + err); }

                Console.Write("\nESC - Shutdown System\n\nQ - Safe Mode\nA - Safe Mode with File System\nZ - Normal Mode without filesystem\nEnter - Start System Normally\nC - Console (Text) Mode\n\nSelect option: ");
                try
                {
                    ConsoleKeyInfo cki = Console.ReadKey();
                    switch (cki.Key)
                    {
                        case ConsoleKey.Escape:
                            Kernel.Shutdown();
                            while (true);
                        case ConsoleKey.Q:
                            Kernel.useDisks = false;
                            Kernel.safeMode = true;
                            return;
                        case ConsoleKey.A:
                            Kernel.useDisks = true;
                            Kernel.safeMode = true;
                            return;
                        case ConsoleKey.Z:
                            Kernel.useDisks = false;
                            return;
                        case ConsoleKey.Enter:
                            return;
                        case ConsoleKey.C:
                            ConsoleMode();
                            break;
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
            shell = new CommandShell { crashClient = ExecuteError, update = Result };
            Console.ResetColor();
            Console.Clear();
            //Console.CursorVisible = true;
            Console.WriteLine(Kernel.OSVERSION + "\n");
            while (true)
            {
                Console.Write("> ");
                shell.Execute(Console.ReadLine());
                Heap.Collect();
            }
        }
        private static void ExecuteError()
        {
            throw new Exception("Manual crash");
        }
        private static void Result()
        {
            Console.WriteLine(shell.print);
        }
    }
}