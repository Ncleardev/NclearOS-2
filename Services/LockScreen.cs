using Cosmos.System;
using Cosmos.HAL;

namespace NclearOS2
{
    public class LockScreen
    {
        public static void Update()
        {
            KeyEvent keyEvent = null;
            if (KeyboardManager.TryReadKey(out keyEvent)) { Kernel.Lock = false; }
            if (Kernel.Pressed)
            {
                if(MouseManager.Y > (int)Kernel.screenY - 40 && MouseManager.X < 40)
                {
                    Kernel.ShutdownPC(false);
                }
                else
                {
                    Msg.Main("Welcome", false);
                    Kernel.Lock = false;
                }
            }
            Kernel.canvas.DrawImage(Kernel.wallpaperlock, 0, 0);
            Kernel.canvas.DrawImageAlpha(Kernel.shutdown, 10, (int)Kernel.screenY - 34);
            Kernel.canvas.DrawString(Date.CurrentTime(false), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX / 2 - 30, (int)Kernel.screenY / 3 - 30);
            Kernel.canvas.DrawString(Date.CurrentDate(true, false), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX / 2 - 80, (int)Kernel.screenY / 3 - 10);
        }
    }
}