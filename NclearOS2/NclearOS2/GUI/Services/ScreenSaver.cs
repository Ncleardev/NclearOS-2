using Cosmos.System;
using Cosmos.HAL;
using Cosmos.Core.Memory;
using Cosmos.System.Graphics;

namespace NclearOS2.GUI
{
    public class ScreenSaver
    {
        private static int x = 0;
        private static int y = 0;
        private static int step = 2;
        private static System.Random rnd = new System.Random();
        private static bool i;
        public static void Update()
        {
            if (!i)
            {
                GUI.Lock = true;
                GUI.DisplayCursor = false;
                GUI.canvas.Clear();
                i = true;
            }
            if (KeyboardManager.TryReadKey(out KeyEvent keyEvent) || GUI.Pressed)
            {
                i = false;
                GUI.screenSaver = false;
                GUI.DisplayCursor = true;
            }
            x += step;
            y++;
            if(y % 100 == 0) { step = rnd.Next(1, 5); }
            GUI.canvas.DrawPoint(GUI.SystemPen, x, y);
            GUI.canvas.DrawPoint(GUI.SystemPen, x-1, y-1);
            GUI.canvas.DrawPoint(GUI.SystemPen, x+1, y+1);
            if (x > GUI.screenX-5) { x = 0; y = 0;}
            GUI.canvas.DrawFilledRectangle(GUI.DarkPen, (int)GUI.screenX - 200, (int)GUI.screenY - 36, 200, 36);
            Font.DrawString(NclearOS2.Date.CurrentTime(true),System.Drawing.Color.White, (int)GUI.screenX - 200, (int)GUI.screenY - 36);
            Font.DrawString(NclearOS2.Date.CurrentDate(true, false), System.Drawing.Color.White, (int)GUI.screenX - 200, (int)GUI.screenY - 20);
        }
    }
    internal class ScreenSaverService : Process
    {
        public ScreenSaverService() : base("Screensaver Service", Priority.Low) { }
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
        internal override int Stop() { return 0; }
    }
}