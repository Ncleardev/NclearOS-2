using Cosmos.System;
using Cosmos.HAL;
using Cosmos.Core.Memory;
using Cosmos.System.Graphics;
using System;

namespace NclearOS2.GUI
{
    public class ScreenSaver
    {
        private static Bitmap logo = new(Resources.Logo);
        private static float x = 0;
        private static float y = 0;
        private static float stepX = 1;
        private static float stepY = 1;
        private static bool i;
        public static void Update()
        {
            GUI.canvas.Clear();
            if (!i)
            {
                GUI.Lock = true;
                GUI.DisplayCursor = false;
                i = true;
            }
            if (KeyboardManager.TryReadKey(out KeyEvent keyEvent) || GUI.Pressed)
            {
                i = false;
                GUI.screenSaver = false;
                GUI.DisplayCursor = true;
            }
            GUI.canvas.DrawImage(logo, (int)x, (int)y);
            if (x >= GUI.ScreenX-logo.Width) { stepX = -1; } else if (x <= 0) { stepX = 1; }
            if (y >= GUI.ScreenY-logo.Height) { stepY = -1; } else if (y <= 0) { stepY = 1; }
            x += stepX * (1.0f / GUI.fps) * 100;
            y += stepY * (1.0f / GUI.fps) * 100;
            Font.DrawString(NclearOS2.Date.CurrentTime(true),System.Drawing.Color.White, (int)GUI.ScreenX - 200, (int)GUI.ScreenY - 36);
            Font.DrawString(NclearOS2.Date.CurrentDate(true, false), System.Drawing.Color.White, (int)GUI.ScreenX - 200, (int)GUI.ScreenY - 20);
        }
    }
    internal class ScreenSaverService : Process
    {
        public ScreenSaverService() : base("Screensaver Service", ProcessManager.Priority.Low) { }
        private int counter;
        private uint oldXMouse;
        private uint oldYMouse;

        internal override int Start()
        {
            return 0;
        }
        internal override void Update()
        {
            if(GUI.wasClicked) { GUI.wasClicked = false; counter = 0; return; }
            if (MouseManager.X == oldXMouse || MouseManager.Y == oldYMouse)
            {
                counter++;
                if(counter > 5)
                {
                    counter = 0;
                    GUI.screenSaver = true;
                    return;
                }
            }
            else { counter = 0; oldXMouse = MouseManager.X; oldYMouse = MouseManager.Y; }
        }
    }
}