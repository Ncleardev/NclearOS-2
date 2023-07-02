using Cosmos.System;

namespace NclearOS2.GUI
{
    internal class GlobalInput : Process
    {
        public static KeyEvent keyEvent;
        public GlobalInput() : base("Input Manager", Priority.Realtime, true) { }
        internal override int Start() { return 0; }
        internal override void Update()
        {
            if (KeyboardManager.TryReadKey(out keyEvent))
            {
                GUI.userInactivity = false;
                GUI.userInactivityTime = -1;
                switch (keyEvent.Key)
                {
                    case ConsoleKeyEx.LWin:
                    case ConsoleKeyEx.RWin:
                        Menu.Opened = !Menu.Opened;
                        return;
                    case ConsoleKeyEx.F12:
                        ProcessManager.Run(new TaskManager());
                        return;
                    case ConsoleKeyEx.F11:
                        ProcessManager.Run(new ConsoleApp((int)(GUI.displayMode.Columns - 250), (int)(GUI.displayMode.Rows - 100)));
                        return;
                    case ConsoleKeyEx.F4:
                        if (KeyboardManager.AltPressed)
                        {
                            if (Menu.Opened) { Menu.Opened = false; }
                            else
                            {
                                if (ProcessManager.running.Count > 0 && ProcessManager.running[0] is Window) { ProcessManager.RemoveAt(0); }
                                else { GUI.Lock = true; }
                            }
                        }
                        else { SendKey(keyEvent); }
                        return;
                    case ConsoleKeyEx.Escape:
                        if (Menu.Opened) { Menu.Opened = false; }
                        else { SendKey(keyEvent); }
                        return;
                    case ConsoleKeyEx.Delete:
                        if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed) { Cosmos.System.Power.Reboot(); }
                        else { SendKey(keyEvent); }
                        return;
                    default:
                        SendKey(keyEvent);
                        return;
                }
            }
        }
        internal override int Stop() { return 0; }
        internal static void SendKey(KeyEvent keyEvent)
        {
            if (ProcessManager.running.Count > 0 && ProcessManager.running[0] is Window w)
            {
                w?.OnKeyPressed?.Invoke(keyEvent);
            }
        }
    }
}