using Cosmos.System;
using System.Collections.Generic;
using Cosmos.System.Graphics;

namespace NclearOS2
{
    public static class Input
    {
        private static List<string> history = new List<string>();
        public static string input;
        public static int wX;
        public static int wY;
        public static bool ready;
        public static int position = 0;
        public static Pen TextPen;

        public static void Main()
        {
            KeyEvent keyEvent = null;
            if (KeyboardManager.TryReadKey(out keyEvent))
            {
                switch (keyEvent.Key)
                {
                    case ConsoleKeyEx.Escape:
                        input = null;
                        ready = true;
                        return;
                    case ConsoleKeyEx.Enter:
                        history.Add(input);
                        ready = true;
                        return;
                    case ConsoleKeyEx.UpArrow:
                        if (history.Count - position > 0)
                        {
                            ++position;
                            input = history[history.Count - position];
                        }
                        break;
                    case ConsoleKeyEx.DownArrow:
                        if (position > 1)
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
                        input = "help";
                        ready = true;
                        return;
                    default:
                        input += keyEvent.KeyChar;
                        break;
                }
            }

        }
        public static void Update()
        {
            if (Window.display)
            {
                Window.DisplayText(input, wX, wY, TextPen);
            }
        }
    }
}