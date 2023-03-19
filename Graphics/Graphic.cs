using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System.Collections.Generic;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace NclearOS2
{
    public class Graphic
    {
        public static void TextView(string text, int x, int y, Color pen)
        {
            foreach (string line in text.Split('\n'))
            {
                Kernel.canvas.DrawString(line, Kernel.font, pen, x, y);
                y += 16;
            }
        }
        public static string EditText(Input inputService, int x, int y, Color pen, bool onEnter = false)
        {
            string[] lines = inputService.input.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (RTC.Second % 2 == 0 && i == lines.Length - 1)
                {
                    Kernel.canvas.DrawString(lines[i] + '_', Kernel.font, pen, x, y);
                }
                else
                {
                    Kernel.canvas.DrawString(lines[i], Kernel.font, pen, x, y);
                }
                y += 16;
            }
            if (onEnter) { return null; } else { return inputService.input; }
        }
        public static bool Button(string text, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, x, y, text.Length * 8 + 10, 25);
            if (MouseManager.X > x && MouseManager.Y > y && MouseManager.X < x + text.Length * 8 + 20 && MouseManager.Y < y + 25)
            {
                Kernel.canvas.DrawString(text, Kernel.font, Kernel.WhitePen, x + 6, y + 6);
                if (Kernel.Pressed) { return true; }
            }
            else { Kernel.canvas.DrawString(text, Kernel.font, Kernel.WhitePen, x + 5, y + 5); }
            return false;
        }
        public static bool CheckBox(string text, int x, int y, Color pen, bool isCheckedByDefault = false)
        {
            if (MouseManager.X > x && MouseManager.Y > y && MouseManager.X < x + 50 && MouseManager.Y < y + 25)
            {
                Kernel.canvas.DrawString(text, Kernel.font, Kernel.WhitePen, x + 6, y + 6);
                if (Kernel.Pressed) { return true; }
            }
            else { Kernel.canvas.DrawString(text, Kernel.font, Kernel.WhitePen, x + 5, y + 5); }
            return false;
        }
        public static (int, int, int) Listview(List<string> list, int StartX, int StartY, int x, int y, int clickedOn, int oneClick, int selectionY)
        {
            if (selectionY != 0) { Kernel.canvas.DrawFilledRectangle(Kernel.GrayPen, StartX, selectionY + StartY, x, 16); }
            if (MouseManager.Y > StartY && MouseManager.Y < StartY + y && MouseManager.X > StartX && MouseManager.X < StartX + x)
            {
                clickedOn = ((int)MouseManager.Y - StartY) / 16;
                Kernel.canvas.DrawString("Position: " + clickedOn, Kernel.font, Kernel.WhitePen, StartX + x - 100, StartY + y - 10);
                if (Kernel.Pressed)
                {
                    if (clickedOn < list.Count)
                    {
                        if (oneClick == clickedOn)
                        {
                            return (clickedOn, 0, 0);
                        }
                        else
                        {
                            oneClick = clickedOn;
                            selectionY = clickedOn * 16;
                        }
                    }
                }
            }
            TextView(string.Join('\n', list.ToArray()), StartX, StartY, Kernel.WhitePen);
            return (-1, oneClick, selectionY);
        }
    }
    public class Animation
    {
        public static (string, float, float, float, Bitmap) hideWindow;
        static float x = 10;
        static float y = Kernel.screenY - 10;
        static float x2 = 100;
        public static Window newWindow;
        public static int unMinimize = -1;
        public static void Refresh()
        {
            if (hideWindow.Item1 != null)
            {
                if (hideWindow.Item2 > 1 && hideWindow.Item3 < Kernel.screenY - 1)
                {
                    hideWindow.Item2 /= 1.05f;
                    hideWindow.Item3 *= 1.05f;
                    hideWindow.Item4 /= 1.03f;
                    Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, (int)hideWindow.Item2, (int)hideWindow.Item3, (int)hideWindow.Item4, 30);
                    Kernel.canvas.DrawString(hideWindow.Item1, Kernel.font, Kernel.WhitePen, (int)hideWindow.Item2 + 36, (int)hideWindow.Item3 + 10);
                    Kernel.canvas.DrawImageAlpha(hideWindow.Item5, (int)(hideWindow.Item2 + 5), (int)hideWindow.Item3 + 3);
                }
                else { hideWindow.Item1 = null; }
            }
            if (newWindow != null)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, (int)x, (int)y, (int)x2, 30);
                Kernel.canvas.DrawString(newWindow.name, Kernel.font, Kernel.WhitePen, (int)x + 36, (int)y + 10);
                Kernel.canvas.DrawImageAlpha(newWindow.icon, (int)(x + 5), (int)y + 3);
                if (x < ProcessManager.running.Count * 20 + 50 && y > ProcessManager.running.Count * 20 + 50)
                {
                    x *= 1.05f;
                    y /= 1.05f;
                    x2 *= 1.03f;
                }
                else
                {
                    ProcessManager.running.Insert(0, newWindow); ProcessManager.running[0].id = 0; newWindow = null; x = 10; y = Kernel.screenY - 10; x2 = 100; unMinimize = -1;
                }
            }
            if (unMinimize != -1)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, (int)x, (int)y, (int)x2, 30);
                Kernel.canvas.DrawString(ProcessManager.running[unMinimize].name, Kernel.font, Kernel.WhitePen, (int)x + 36, (int)y + 10);
                Kernel.canvas.DrawImageAlpha(ProcessManager.running[unMinimize].icon, (int)(x + 5), (int)y + 3);
                if (x < ProcessManager.running[unMinimize].StartWindowX && y > ProcessManager.running[unMinimize].StartWindowY)
                {
                    x *= 1.05f;
                    y /= 1.05f;
                    x2 *= 1.03f;
                }
                else
                {
                    ProcessManager.running[unMinimize].minimized = false; newWindow = null; x = 10; y = Kernel.screenY - 10; x2 = 100; unMinimize = -1;
                }
            }
        }
    }
    public class Buffer // to do
    {
        public static Color[] GetBuffer(int StartX, int StartY, int x2, int y2)
        {
            Color[] pixelBuffer = new Color[(x2 * y2) + x2];

            for (int y = 0; y < y2; y++)
            {
                for (int x = 0; x < x2; x++)
                {
                    pixelBuffer[(x*y) + x] = Kernel.canvas.GetPointColor(x + StartX, y + StartY);
                }
            }

            return pixelBuffer;
        }
    }
}