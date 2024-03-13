using Cosmos.System;
using NclearOS2.GUI;
using System.Linq;

namespace NclearOS2.GUI
{
    internal class GlobalInput : Process
    {
        public static KeyEvent keyEvent;
        public static bool clickAlternative;
        public GlobalInput() : base("Input Manager", ProcessManager.Priority.Realtime) { }
        internal override void Update()
        {
            if (KeyboardManager.TryReadKey(out keyEvent))
            {
                GUI.wasClicked = true;
                switch (keyEvent.Key)
                {
                    case ConsoleKeyEx.LWin:
                    case ConsoleKeyEx.RWin:
                        Menu.Opened = !Menu.Opened;
                        return;
                    case ConsoleKeyEx.F12:
                        Notify("Quick Launch Process Manager");
                        ProcessManager.Run(new TaskManager());
                        return;
                    case ConsoleKeyEx.F10:
                        Notify("Quick Launch Console");
                        ProcessManager.Run(new ConsoleApp((int)(GUI.ScreenX - 200), (int)(GUI.ScreenY - 100)));
                        return;
                    case ConsoleKeyEx.F4:
                        if (KeyboardManager.AltPressed)
                        {
                            if (Menu.Opened) { Menu.Opened = false; return; }
                            else if (ProcessManager.running.Count(p => p is Window) == 0) { GUI.Lock = true; return; }
                        }
                        ProcessManager.Key(keyEvent);
                        return;
                    case ConsoleKeyEx.Escape:
                        if (KeyboardManager.ControlPressed) { Menu.Opened = !Menu.Opened; return; }
                        if (Menu.Opened) { Menu.Opened = false; }
                        else { ProcessManager.Key(keyEvent); }
                        return;
                    case ConsoleKeyEx.Delete:
                        if (KeyboardManager.ControlPressed && KeyboardManager.AltPressed) { Power.Reboot(); }
                        else { ProcessManager.Key(keyEvent); }
                        return;
                    case ConsoleKeyEx.Tab:
                        if (KeyboardManager.AltPressed) { WindowManager.FocusAtWindow(1); }
                        return;
                    case ConsoleKeyEx.Num6: if (GUI.keyCursor) { MouseManager.X += (uint)MouseManager.MouseSensitivity * 12; } else { ProcessManager.Key(keyEvent); } break;
                    case ConsoleKeyEx.Num4: if (GUI.keyCursor) { MouseManager.X -= (uint)MouseManager.MouseSensitivity * 12; } else { ProcessManager.Key(keyEvent); } break;
                    case ConsoleKeyEx.Num8: if (GUI.keyCursor) { MouseManager.Y -= (uint)MouseManager.MouseSensitivity * 12; } else { ProcessManager.Key(keyEvent); } break;
                    case ConsoleKeyEx.Num2: if (GUI.keyCursor) { MouseManager.Y += (uint)MouseManager.MouseSensitivity * 12; } else { ProcessManager.Key(keyEvent); } break;
                    case ConsoleKeyEx.Num5: clickAlternative = true; break;
                    default:
                        ProcessManager.Key(keyEvent);
                        return;
                }
            }
            else if (clickAlternative) { clickAlternative = false; }
        }
    }
}