using Cosmos.System;

namespace NclearOS2
{
    public class Msg
    {
        private static string msg;
        private static string msg2;
        private static string title;
        public static bool displaymsg;
        public static bool displaymsg2;
        public static bool error;
        public static void Main(string msg1, bool error1)
        {
            error = error1;
            msg = msg1;
            displaymsg = true;
        }
        public static void Ver2(string msg12, string title2)
        {
            title = title2;
            msg2 = msg12;
            displaymsg2 = true;
        }
        public static void Update()
        {
            if (displaymsg)
            {
                if (error) { Kernel.canvas.DrawFilledRectangle(Kernel.Red2Pen, ((int)(Kernel.screenX - msg.Length * 8) / 2) - 10, (int)(Kernel.screenY - 12) / 2, msg.Length * 8 + 20, 24); }
                else { Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, ((int)(Kernel.screenX - msg.Length * 8) / 2) - 10, (int)(Kernel.screenY - 12) / 2, msg.Length * 8 + 20, 24); }
                Kernel.canvas.DrawString(msg, Kernel.font, Kernel.WhitePen, (int)(Kernel.screenX - msg.Length * 8) / 2, (int)(Kernel.screenY - 1) / 2);
            }
            if (displaymsg2)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, 150, 100, 200, 30);
                Kernel.canvas.DrawImageAlpha(Kernel.program, 155, 103);
                Kernel.canvas.DrawString(title, Kernel.font, Kernel.WhitePen, 184, 110);
                if (MouseManager.X > 77 && MouseManager.X < 101 && MouseManager.Y > 70 && MouseManager.Y < 100)
                {
                    Kernel.canvas.DrawImageAlpha(Kernel.closered, 80, 77);
                    if (Kernel.Pressed) { displaymsg2 = false; }
                }
                else
                {
                    Kernel.canvas.DrawImageAlpha(Kernel.close, 80, 77);
                }
                Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, ((int)(Kernel.screenX - msg.Length * 8) / 2) - 10, (int)(Kernel.screenY - 12) / 2, msg.Length * 8 + 20, 24);
                Kernel.canvas.DrawString(msg, Kernel.font, Kernel.WhitePen, (int)(Kernel.screenX - msg.Length * 8) / 2, (int)(Kernel.screenY - 1) / 2);
            }
        }
    }
}