using System;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Collections.Generic;

namespace NclearOS2
{
    public static class Input
    {
        private static List<string> history = new List<string>();
        public static string input;
        public static bool Inputing;
        public static bool ready;
        private static bool HistoryOn;
        public static int position = 0;

        public static void Register(bool HistoryEnabled)
        {
            input = "";
            Inputing = true;
            ready = false;
            HistoryOn = HistoryEnabled;
            position = 0;
        }

        public static bool Listener()
        {
            if (ready)
            {
                ready = false;
                position = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void Update(KeyEvent keyEvent)
        {
            switch (keyEvent.Key)
            {
                case ConsoleKeyEx.LWin:
                case ConsoleKeyEx.RWin:
                    Menu.Opened = !Menu.Opened;
                    return;
                case ConsoleKeyEx.F12:
                    Process.Run(Process.Apps.console);
                    return;
                case ConsoleKeyEx.F4:
                    if (KeyboardManager.AltPressed)
                    {
                        if (Menu.Opened) { Menu.Opened = false; }
                        else
                        {
                            if (Window.display) { Window.display = false; }
                            else
                            { Kernel.Lock = true; }
                        }
                    }
                    return;
                case ConsoleKeyEx.F5:
                    if(Process.currentApp == Process.Apps.files && Files.CD != "Computer")
                    {
                        Files.RefreshList(true);
                    }
                    return;
                case ConsoleKeyEx.Delete:
                    if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed) { Cosmos.System.Power.Reboot(); }
                    return;
                case ConsoleKeyEx.Escape:
                    Menu.Opened = false;
                    return;
            }
            if (Inputing)
            {
                switch (keyEvent.Key)
                {
                    case ConsoleKeyEx.Escape:
                        input = null;
                        ready = true;
                        break;
                    case ConsoleKeyEx.Enter:
                        ready = true;
                        if (HistoryOn) { history.Add(input); }
                        else
                        {
                            input += "\n";
                        }
                        break;
                    case ConsoleKeyEx.UpArrow:
                        if (HistoryOn && history.Count - position > 0)
                        {
                            ++position;
                            input = history[history.Count - position];
                        }
                        break;
                    case ConsoleKeyEx.DownArrow:
                        if (HistoryOn && position > 1)
                        {
                            --position;
                            input = history[history.Count - position];
                        }
                        break;
                    case ConsoleKeyEx.Backspace:
                        if (input.Length > 0)
                        {
                            input = input.Remove(input.Length - 1);
                        }
                        break;
                    case ConsoleKeyEx.F1:
                        if (HistoryOn)
                        {
                            input = "help";
                            ready = true;
                        }
                        break;
                    default:
                        if(char.IsLetterOrDigit(keyEvent.KeyChar) || char.IsPunctuation(keyEvent.KeyChar) || char.IsSymbol(keyEvent.KeyChar) || (keyEvent.KeyChar == ' '))
                        { input += keyEvent.KeyChar; }
                        break;
                }
            }
        }
    }
}