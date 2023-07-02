using Cosmos.System;
using Cosmos.HAL;
using Cosmos.Core.Memory;

namespace NclearOS2.GUI
{
    public class ScreenSaver
    {
        private static int x = 0;
        private static int y = 0;
        private static int step = 2;
        private static System.Random rnd = new System.Random();
        public static void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent keyEvent) || GUI.Pressed)
            {
                GUI.screenSaver = false;
                GUI.HideCursor = false;
            }
            x += step;
            y++;
            if(y % 100 == 0) { step = rnd.Next(1, 5); }
            GUI.canvas.DrawPoint(GUI.SystemPen, x, y);
            GUI.canvas.DrawPoint(GUI.SystemPen, x-1, y-1);
            GUI.canvas.DrawPoint(GUI.SystemPen, x+1, y+1);
            if (x > GUI.displayMode.Columns) { x = 0; y = 0;}
            GUI.canvas.DrawFilledRectangle(GUI.DarkPen, (int)GUI.displayMode.Columns - 200, (int)GUI.displayMode.Rows - 36, 200, 36);
            Font.DrawString(NclearOS2.Date.CurrentTime(true),System.Drawing.Color.White, (int)GUI.displayMode.Columns - 200, (int)GUI.displayMode.Rows - 36);
            Font.DrawString(NclearOS2.Date.CurrentDate(true, false), System.Drawing.Color.White, (int)GUI.displayMode.Columns - 200, (int)GUI.displayMode.Rows - 20);
        }
    }
}