using Cosmos.System;
using System.Drawing;

namespace NclearOS2.GUI
{
    public class LockScreen
    {
        private static bool Menu;
        private static char second10;

        private static void Unlock()
        {
            Menu = false;
            GUI.Lock = false;

            var anim = Animation2.Animate(Images.wallpaperBlur)
                .StartAt(0, 0)
                .MoveTo(0, GUI.ScreenY - 30);
            Animation2.Queue.Insert(0, anim);
        }

        public static void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent keyEvent))
            {
                if (keyEvent.Key == ConsoleKeyEx.Delete && KeyboardManager.ControlPressed && KeyboardManager.AltPressed) { Cosmos.System.Power.Reboot(); }
                else if (keyEvent.Key == ConsoleKeyEx.Spacebar) {
                    Unlock();
                }
                GUI.wasClicked = true;
            }
            if (GUI.Pressed)
            {
                if (Menu)
                {
                    if (MouseManager.X < 111)
                    {
                        if (MouseManager.Y < (int)GUI.ScreenY - 70 && MouseManager.Y > (int)GUI.ScreenY - 100)
                        {
                            Kernel.Shutdown(true);
                        }
                        else if (MouseManager.Y < (int)GUI.ScreenY - 40 && MouseManager.Y > (int)GUI.ScreenY - 70)
                        {
                            Kernel.Shutdown(false);
                        }
                    }
                    Menu = false;
                }
                else
                {
                    if (MouseManager.Y > (int)GUI.ScreenY - 40 && MouseManager.X < 40)
                    {
                        Menu = !Menu;
                    }
                    else
                    {
                        Unlock();
                    }
                }
            }
            GUI.canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.ScreenY - 35);
            Font.DrawString(NclearOS2.Date.CurrentTime(false),Color.White, (int)GUI.ScreenX / 2 - 30, (int)GUI.ScreenY / 3 - 80);
            Font.DrawString(NclearOS2.Date.CurrentDate(true, false),Color.White, (int)GUI.ScreenX / 2 - 80, (int)GUI.ScreenY / 3 - 60);
            if (Menu)
            {
                GUI.canvas.DrawImageAlpha(Icons.reboot, 10, (int)GUI.ScreenY - 95);
                Font.DrawString("Restart",Color.White, 40, (int)GUI.ScreenY - 90);

                GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.ScreenY - 65);
                Font.DrawString("Shutdown",Color.White, 40, (int)GUI.ScreenY - 60);
            }
            if(GUI.screenSaverProcess != null)
            {
                if (NclearOS2.Date.CurrentSecond()[0] != second10)
                {
                    second10 = NclearOS2.Date.CurrentSecond()[0];
                    GUI.screenSaverProcess.Update();
                }
            }
        }
    }
}