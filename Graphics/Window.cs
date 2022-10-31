using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Threading;

namespace NclearOS2
{
    public class Window
    {
        public static string name;
        public static int x;
        public static int y;
        public static int StartWindowX = 100;
        public static int StartWindowY = 70;
        public static int StartWindowXOld;
        public static int StartWindowYOld;
        public static bool display;
        public static bool windowlock;
        public static Bitmap icon = Kernel.program;
        public static void Update()
        {
            if (display)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, StartWindowX, StartWindowY, x, 30);
                Kernel.canvas.DrawString(name, Kernel.font, Kernel.WhitePen, StartWindowX+30, StartWindowY+10);
                Process.UpdateCanvas(x, y);
                if(MouseManager.X > StartWindowX-23 + x && MouseManager.X < StartWindowX+1 + x && MouseManager.Y > StartWindowY && MouseManager.Y < StartWindowY+30)
                {
                    Kernel.canvas.DrawImageAlpha(Kernel.closered, StartWindowX-20 + x, StartWindowY+7);
                    if (Kernel.Pressed) { display = false; Process.Reset(); }
                }
                else
                {
                    Kernel.canvas.DrawImageAlpha(Kernel.close, StartWindowX-20 + x, StartWindowY+7);
                    if (Kernel.Pressed && MouseManager.X > StartWindowX && MouseManager.X < StartWindowX + x - 23 && MouseManager.Y > StartWindowY && MouseManager.Y < StartWindowY + 30)
                    {
                        StartWindowXOld = StartWindowX - (int)MouseManager.X;
                        StartWindowYOld = StartWindowY - (int)MouseManager.Y;
                        windowlock = true;
                    }
                    if (windowlock)
                    {
                        if (Kernel.LongPress)
                        {
                            StartWindowX = StartWindowXOld + (int)MouseManager.X;
                            StartWindowY = StartWindowYOld + (int)MouseManager.Y;
                            if (StartWindowY < 0)
                            {
                                StartWindowY = 0;
                            }else if (StartWindowY > Kernel.screenY - 24) { StartWindowY = (int)Kernel.screenY - 24; }
                            
                        }
                        else
                        {
                            windowlock = false;
                        }
                    }
                }
            }
        }
        public static void DisplayText(string text, int x, int y, Pen pen)
        {
            if (text.Contains("\n"))
            {
                string[] lines = text.Split('\n');
                foreach (string line in lines)
                {
                    Kernel.canvas.DrawString(line, Kernel.font, pen, x, y);
                    y += 16;
                }

            }
            else
            {
                Kernel.canvas.DrawString(text, Kernel.font, pen, x, y);
            }
        }
        public static void Init(string name2, int x2, int y2, Bitmap icon2, bool StopLoadingCursor)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            name = name2;
            x = x2;
            y = y2;
            icon = icon2;
            Window.display = true;
            if (StopLoadingCursor) { Kernel.Loading = false; }
        }
    }
}