using Cosmos.System;
using Cosmos.HAL;
using Cosmos.Core.Memory;

namespace NclearOS2
{
    public class ScreenSaver
    {
        private static int x = 0;
        private static int y = 0;
        private static int step = 2;
        private static System.Random rnd = new System.Random();
        public static void Update()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent keyEvent) || Kernel.Pressed)
            {
                Kernel.screenSaver = false;
                Kernel.HideCursor = false;
            }
            x += step;
            y++;
            if(y % 100 == 0) { step = rnd.Next(1, 5); }
            Kernel.canvas.DrawPoint(Kernel.SystemPen, x, y);
            Kernel.canvas.DrawPoint(Kernel.SystemPen, x-1, y-1);
            Kernel.canvas.DrawPoint(Kernel.SystemPen, x+1, y+1);
            if (x > Kernel.screenX) { x = 0; y = 0;}
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkPen, (int)Kernel.screenX - 200, (int)Kernel.screenY - 36, 200, 36);
            Kernel.canvas.DrawString(Date.CurrentTime(true), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX - 200, (int)Kernel.screenY - 36);
            Kernel.canvas.DrawString(Date.CurrentDate(true, false), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX - 200, (int)Kernel.screenY - 20);
        }
    }
}