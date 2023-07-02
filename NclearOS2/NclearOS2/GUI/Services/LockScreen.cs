using Cosmos.System;
using System.Drawing;

namespace NclearOS2.GUI
{
    public class LockScreen
    {
        private static bool Menu;
        public static void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent keyEvent))
            {
                if (keyEvent.Key == ConsoleKeyEx.Delete && KeyboardManager.ControlPressed && KeyboardManager.AltPressed) { Cosmos.System.Power.Reboot(); }
                GUI.userInactivity = false; GUI.userInactivityTime = -1;
            }
            if (GUI.Pressed)
            {
                if (Menu)
                {
                    if (MouseManager.X < 111)
                    {
                        if (MouseManager.Y < (int)GUI.displayMode.Rows - 70 && MouseManager.Y > (int)GUI.displayMode.Rows - 100)
                        {
                            Kernel.Shutdown(true);
                        }
                        else if (MouseManager.Y < (int)GUI.displayMode.Rows - 40 && MouseManager.Y > (int)GUI.displayMode.Rows - 70)
                        {
                            Kernel.Shutdown(false);
                        }
                    }
                    Menu = false;
                }
                else
                {
                    if (MouseManager.Y > (int)GUI.displayMode.Rows - 40 && MouseManager.X < 40)
                    {
                        Menu = !Menu;
                    }
                    else
                    {
                        GUI.Lock = false;
                        Menu = false;
                    }
                }
            }
            if (Kernel.safeMode) { GUI.canvas.Clear(Color.CadetBlue); } else { GUI.canvas.DrawImage(Images.wallpaperLock, 0, 0); }
            GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.displayMode.Rows - 35);
            Font.DrawString(NclearOS2.Date.CurrentTime(false),Color.White, (int)GUI.displayMode.Columns / 2 - 30, (int)GUI.displayMode.Rows / 3 - 80);
            Font.DrawString(NclearOS2.Date.CurrentDate(true, false),Color.White, (int)GUI.displayMode.Columns / 2 - 80, (int)GUI.displayMode.Rows / 3 - 60);
            if (Menu)
            {
                GUI.canvas.DrawImageAlpha(Icons.reboot, 10, (int)GUI.displayMode.Rows - 95);
                Font.DrawString("Restart",Color.White, 40, (int)GUI.displayMode.Rows - 90);

                GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.displayMode.Rows - 65);
                Font.DrawString("Shutdown",Color.White, 40, (int)GUI.displayMode.Rows - 60);
            }
        }
    }
}