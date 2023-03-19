using Cosmos.System;
using System.Collections.Generic;

namespace NclearOS2
{
    public class GlobalInput : Service
    {
        public static KeyEvent keyEvent;
        public GlobalInput() : base("Global Input Manager", Priority.Realtime) { }
        internal override bool Start()
        {
            return true;
        }
        internal override bool Update()
        {
            if (KeyboardManager.TryReadKey(out keyEvent))
            {
                Kernel.userInactivity = false;
                Kernel.userInactivityTime = -1;
                Input.newKey = true;
                switch (keyEvent.Key)
                {
                    case ConsoleKeyEx.LWin:
                    case ConsoleKeyEx.RWin:
                        Menu.Opened = !Menu.Opened;
                        return true;
                    case ConsoleKeyEx.F12:
                        ProcessManager.Add(new TaskManager());
                        return true;
                    case ConsoleKeyEx.F11:
                        ProcessManager.Add(new ConsoleApp((int)(Kernel.screenX - 250), (int)(Kernel.screenY - 170)));
                        return true;
                    case ConsoleKeyEx.F4:
                        if (KeyboardManager.AltPressed)
                        {
                            if (Menu.Opened) { Menu.Opened = false; }
                            else
                            {
                                if (ProcessManager.running.Count > 0) { ProcessManager.RemoveAt(0); }
                                else { Kernel.Lock = true; }
                            }
                        }
                        return true;
                    case ConsoleKeyEx.Escape:
                        Menu.Opened = false;
                        return true;
                    case ConsoleKeyEx.Delete:
                        if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed) { Cosmos.System.Power.Reboot(); } else if (ProcessManager.running.Count > 0) { ProcessManager.running[0].Key(ConsoleKeyEx.Delete); }
                        return true;
                    default:
                        if (ProcessManager.running.Count > 0) { ProcessManager.running[0].Key(keyEvent.Key); }
                        return true;
                }
            }
            return true;
        }
        internal override int Stop() { return 0; }
    }
    public class Input : Service
    {
        public static bool newKey;
        private List<string> history = new();
        private int position = 0;
        public bool historyOn;
        public string input;
        public bool ready;
        public Input(string appName, bool ConsoleType) : base("Input Manager (" + appName + ")", Priority.Realtime)
        {
            this.historyOn = ConsoleType;
            this.input = null;
        }
        internal override bool Start()
        {
            input = null;
            return true;
        }
        internal override bool Update()
        {
            if (newKey)
            {
                newKey = false;
                switch (GlobalInput.keyEvent.Key)
                {
                    case ConsoleKeyEx.Escape:
                        input = null;
                        ready = true;
                        break;
                    case ConsoleKeyEx.Enter:
                        if (historyOn) { history.Add(input); ready = true; }
                        else
                        {
                            input += "\n";
                        }
                        break;
                    case ConsoleKeyEx.UpArrow:
                        if (historyOn && history.Count - position > 0)
                        {
                            ++position;
                            input = history[history.Count - position];
                        }
                        break;
                    case ConsoleKeyEx.DownArrow:
                        if (historyOn && position > 1)
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
                        if (historyOn)
                        {
                            input = "help";
                            ready = true;
                        }
                        break;
                    default:
                        if (char.IsLetterOrDigit(GlobalInput.keyEvent.KeyChar) || char.IsPunctuation(GlobalInput.keyEvent.KeyChar) || char.IsSymbol(GlobalInput.keyEvent.KeyChar) || (GlobalInput.keyEvent.KeyChar == ' '))
                        { input += GlobalInput.keyEvent.KeyChar; }
                        break;
                }
            }
            return true;
        }
        internal override int Stop() { return 0; }
    }
}