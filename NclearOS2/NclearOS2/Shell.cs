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
        private static int yPos;
        private static bool exitToGUI;
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

                Console.Write("\nESC - Shutdown System\n\nQ - Safe Mode\nEnter - Start System Normally\nC - Console (Text) Mode\n\nSelect option: ");
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
                            return;
                        case ConsoleKey.Enter:
                            return;
                        case ConsoleKey.C:
                            ConsoleMode();
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
            shell = new CommandShell { crashClient = ExecuteError, update = Result };
            Console.ResetColor();
            Console.Clear();
            //Console.CursorVisible = true;
            Console.WriteLine(Kernel.OSVERSION + "\n");
            while (true)
            {
                Console.Write("> ");
                yPos = Console.CursorTop + 1;
                shell.Execute(Console.ReadLine());
                Heap.Collect();
                if(exitToGUI) { goto Exit; }
            }
        Exit:
            exitToGUI = false;
            return;
        }
        private static void ExecuteError()
        {
            throw new Exception("Manual crash");
        }
        private static void Result()
        {
            if(shell.print.Contains("Successfully changed resolution to ")) { exitToGUI = true; }
            Console.CursorTop = yPos;
            Console.WriteLine(shell.print);
        }
    }
}