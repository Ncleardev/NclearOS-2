using Cosmos.System;
using Cosmos.System.Graphics.Fonts;
using NclearOS2.Commands;
using System;
using System.Threading;

namespace NclearOS2
{
    public class Boot
    {
        public static void Run()
        {
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.CursorTop = System.Console.WindowHeight - 1;
            System.Console.Write("Hold Shift for advanced options");
            //System.Console.CursorVisible = true;
            System.Console.Beep(300, 200);
            Thread.Sleep(100);
            System.Console.Beep(400, 100);
            Thread.Sleep(50);
            System.Console.Beep(500, 100);
            Thread.Sleep(50);
            System.Console.Beep(600, 100);
            Thread.Sleep(50);
            System.Console.Beep(700, 100);

            CommandManager.Register();

            if (KeyboardManager.ShiftPressed) { System.Console.Beep(); TextMode.BootMenu(); }
            //System.Console.CursorVisible = false;
            System.Console.CursorLeft = 0;
            System.Console.Write("                               ");
            System.Console.CursorLeft = 0;

            if (Kernel.useDisks) { Kernel.useDisks = false; FileManager.Start(); }
            GUI.GUI.Init();
        }
    }
}