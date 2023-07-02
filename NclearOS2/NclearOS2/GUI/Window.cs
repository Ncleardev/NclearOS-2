using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using System.Linq;

namespace NclearOS2.GUI
{
    internal abstract class Window : Process
    {
        internal Window(string name, int x, int y, Bitmap icon, Priority priority) : base(name, priority)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.icon = icon;
            this.StartX = this.StartY = this.StartXOld = this.StartYOld = ProcessManager.running.Count(p => p is Window) * 20 + 50;
            appCanvas = new Bitmap((uint)x, (uint)y, ColorDepth.ColorDepth32);
            borderCanvas = PostProcess.CropBitmap(Images.wallpaperBlur, StartX, StartY, x, 30);
            Font.DrawString(name, Color.White.ToArgb(), 36, 10, borderCanvas.rawData, x);
            Font.DrawImageAlpha(icon, 5, 3, borderCanvas.rawData, x);
            Font.DrawImageAlpha(Icons.minimize, x - 50, 7, borderCanvas.rawData, x);
            Font.DrawImageAlpha(Icons.close, x - 20, 7, borderCanvas.rawData, x);
        }
        internal int x;
        internal int y;
        internal int StartX;
        internal int StartY;
        internal int StartXOld;
        internal int StartYOld;
        internal bool windowlock;
        internal Bitmap icon;
        internal bool minimized;
        internal Bitmap appCanvas;
        internal Bitmap borderCanvas;
        //internal Action<int, int> OnClicked;
        internal Action<KeyEvent> OnKeyPressed;
        internal Action OnStartMoving;
        internal Action OnMoved;

        public void DrawChar(char c, int color, int bg, int[] canvas, int canvasWidth, int x2, int y2)
        {
            int fontY = Font.fontY;
            int fontX = Font.fontX;
            if (c == ' ')
            {
                for (int py = 0; py < fontY; py++)
                {
                    for (int px = 0; px < fontX; px++)
                    {
                        canvas[(y2 + py) * canvasWidth + (x2 + px)] = bg;
                    }
                }
                return;
            }
            bool[] cache = Font.charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    canvas[(y2 + py) * canvasWidth + (x2 + px)] = cache[py * fontX + px] ? color : bg;
                }
            }
        }
        public void DrawCharAlpha(char c, int color, int[] canvas, int canvasWidth, int x2, int y2)
        {
            int fontY = Font.fontY;
            int fontX = Font.fontX;
            if (c == ' ')
            {
                return;
            }
            bool[] cache = Font.charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    if (cache[py * fontX + px])
                    {
                        canvas[(y2 + py) * canvasWidth + (x2 + px)] = color;
                    }
                }
            }
        }
        public void DrawString(string str, int color, int bg, int x2, int y2)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                DrawChar(c, color, bg, appCanvas.rawData, x, x2, y2);
                x2 += Font.fontX;
            }
        }
        public void DrawStringAlpha(string str, int color, int x2, int y2)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                DrawCharAlpha(c, color, appCanvas.rawData, x, x2, y2);
                x2 += Font.fontX;
            }
        }
        public void DrawImageAlpha(Bitmap image, int x2, int y2)
        {
            for (int py = 0; py < image.Height; py++)
            {
                for (int px = 0; px < image.Width; px++)
                {
                    int temp = image.rawData[py * image.Width + px];
                    if (temp == 0) { continue; }
                    appCanvas.rawData[(int)((y2 + py) * appCanvas.Width + (x2 + px))] = temp;
                }
            }
        }
        internal void DrawHorizontalLine(int color, int x1, int y, int x2)
        {
            for (int x = x1; x <= x2; x++)
            {
                DrawPoint(color, x, y);
            }
        }
        internal void DrawPoint(int color, int x2, int y2)
        {
            appCanvas.rawData[y2 * x + x2] = color;
        }
        internal void DrawFilledRectangle(int color, int x, int y, int width, int height)
        {
            int x2 = x + width - 1;
            int y2 = y + height - 1;

            for (int currentY = y; currentY <= y2; currentY++)
            {
                for (int currentX = x; currentX <= x2; currentX++)
                {
                    DrawPoint(color, currentX, currentY);
                }
            }
        }
        internal void DrawFilledCircle(int color, int startX, int startY, int radius)
        {
            int x = 0;
            int y = radius;
            int decision = 1 - radius;

            while (x <= y)
            {
                DrawHorizontalLine(color, startX - x, startY - y, startX + x);
                DrawHorizontalLine(color, startX - x, startY + y, startX + x);
                DrawHorizontalLine(color, startX - y, startY - x, startX + y);
                DrawHorizontalLine(color, startX - y, startY + x, startX + y);

                if (decision < 0)
                {
                    decision += 2 * x + 3;
                }
                else
                {
                    decision += 2 * (x - y) + 5;
                    y--;
                }
                x++;
            }
        }
        internal void Background(int color = 0)
        {
            MemoryOperations.Fill(appCanvas.rawData, color);
        }
        internal void AlphaBackground()
        {
            appCanvas = PostProcess.CropBitmap(Images.wallpaperBlur, StartX, StartY + 30, x, y);
        }
    }
}