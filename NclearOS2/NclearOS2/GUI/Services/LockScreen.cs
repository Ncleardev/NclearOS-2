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
                GUI.wasClicked = true;
            }
            if (GUI.Pressed)
            {
                if (Menu)
                {
                    if (MouseManager.X < 111)
                    {
                        if (MouseManager.Y < (int)GUI.screenY - 70 && MouseManager.Y > (int)GUI.screenY - 100)
                        {
                            Kernel.Shutdown(true);
                        }
                        else if (MouseManager.Y < (int)GUI.screenY - 40 && MouseManager.Y > (int)GUI.screenY - 70)
                        {
                            Kernel.Shutdown(false);
                        }
                    }
                    Menu = false;
                }
                else
                {
                    if (MouseManager.Y > (int)GUI.screenY - 40 && MouseManager.X < 40)
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
            GUI.canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.screenY - 35);
            Font.DrawString(NclearOS2.Date.CurrentTime(false),Color.White, (int)GUI.screenX / 2 - 30, (int)GUI.screenY / 3 - 80);
            Font.DrawString(NclearOS2.Date.CurrentDate(true, false),Color.White, (int)GUI.screenX / 2 - 80, (int)GUI.screenY / 3 - 60);
            if (Menu)
            {
                GUI.canvas.DrawImageAlpha(Icons.reboot, 10, (int)GUI.screenY - 95);
                Font.DrawString("Restart",Color.White, 40, (int)GUI.screenY - 90);

                GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.screenY - 65);
                Font.DrawString("Shutdown",Color.White, 40, (int)GUI.screenY - 60);
            }
        }
    }
}