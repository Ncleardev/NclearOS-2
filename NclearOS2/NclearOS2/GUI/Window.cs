using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NclearOS2.GUI
{
    internal abstract class Window : Process
    {
        internal Window(string name, int x, int y, Bitmap icon = null, ProcessManager.Priority priority = ProcessManager.Priority.None, WindowManager.Resizable resizable = WindowManager.Resizable.Full) : base(name, priority)
        {
            this.name = name;
            this.StartX = this.StartY = this.StartXOld = this.StartYOld = ProcessManager.running.Count(p => p is Window) * 20 + 50;
            this.x = ogX = x;
            this.y = ogY = y;
            if (this.x > GUI.ScreenX) { this.x = ogX = GUI.ScreenX; }
            if (this.y > GUI.ScreenY) { this.y = ogY = GUI.ScreenY; }
            if (this.StartX + this.x > GUI.ScreenX) { this.StartX = 0; }
            this.icon = icon ?? Icons.program;
            this.resizable = resizable;
            appCanvas = new Bitmap((uint)x, (uint)y, GUI.DisplayMode.ColorDepth);
            RefreshBorder();
        }
        internal int x;
        internal int y;
        internal int ogX;
        internal int ogY;
        internal int StartX;
        internal int StartY;
        internal int StartXOld;
        internal int StartYOld;
        internal int ogStartX;
        internal int ogStartY;
        internal bool windowlock;
        internal WindowManager.Resizable resizable;
        internal Bitmap icon;
        internal bool minimized = true;
        internal Bitmap appCanvas;
        internal Bitmap borderCanvas;
        internal Bitmap bottomBorderCanvas;
        internal Pen leftBorder = GUI.SystemPen;
        internal Pen rightBorder = GUI.SystemPen;
        internal Action<int, int> OnClicked;
        internal Action<int, int> OnHover;
        internal Action<int, int> OnLongPressed;
        internal Action<KeyEvent> OnKeyPressed;
        internal Action OnStartMoving;
        internal Action OnMoved;
        internal Action OnSizeChange;

        public bool DrawChar(char c, int color, int bg, int[] canvas, int canvasWidth, int x2, int y2)
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
                return x2 > canvasWidth - fontX * 2;
            }
            bool[] cache = Font.charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    canvas[(y2 + py) * canvasWidth + (x2 + px)] = cache[py * fontX + px] ? color : bg;
                }
            }
            return x2 > canvasWidth - fontX * 2;
        }
        public bool DrawCharAlpha(char c, int color, int[] canvas, int canvasWidth, int x2, int y2)
        {
            int fontY = Font.fontY;
            int fontX = Font.fontX;
            if (c == ' ')
            {
                return x2 > canvasWidth - fontX * 2;
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
            return x2 > canvasWidth - fontX * 2;
        }
        public void DrawString(string str, int color, int bg, int x2, int y2)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                if (y2 + Font.fontY > y) { return; }
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                if (DrawChar(c, color, bg, appCanvas.rawData, x, x2, y2)) { y2 += 14; x2 = ogX; continue; }
                x2 += Font.fontX;
            }
        }
        public void DrawStringAlpha(string str, int color, int x2, int y2)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                if (y2 + Font.fontY > y) { return; }
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                if (DrawCharAlpha(c, color, appCanvas.rawData, x, x2, y2)) { y2 += 14; x2 = ogX; continue; }
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
        public void RefreshBorder(bool effects = true)
        {
            if (effects && !Kernel.safeMode) {
                borderCanvas = PostProcess.CropBitmap(Images.wallpaperBlur, StartX, StartY, x, 30);
                bottomBorderCanvas = PostProcess.CropBitmap(borderCanvas, 0, (int)borderCanvas.Height - 3, (int)borderCanvas.Width, 3);
                leftBorder = new(Color.FromArgb(borderCanvas.rawData[borderCanvas.rawData.Length - borderCanvas.Width]));
                rightBorder = new(Color.FromArgb(borderCanvas.rawData[borderCanvas.rawData.Length - 1]));
            }
            else {
                if (borderCanvas == null) { borderCanvas = new Bitmap((uint)x, 30, GUI.DisplayMode.ColorDepth); }
                if (bottomBorderCanvas == null) { bottomBorderCanvas = new Bitmap((uint)x, 3, GUI.DisplayMode.ColorDepth); }
                MemoryOperations.Fill(borderCanvas.rawData, GUI.SystemPen.ValueARGB);
                MemoryOperations.Fill(bottomBorderCanvas.rawData, GUI.SystemPen.ValueARGB);
                leftBorder = GUI.SystemPen;
                rightBorder = GUI.SystemPen;
            }
            
            if(ID == 0) { Font.DrawString(name, Color.White.ToArgb(), 36, 10, borderCanvas.rawData, x); }
            else { Font.DrawString(name, Color.Gray.ToArgb(), 36, 10, borderCanvas.rawData, x); }
            Font.DrawImageAlpha(icon, 5, 3, borderCanvas.rawData, x);

            if(resizable != WindowManager.Resizable.None)
            {
                Font.DrawImageAlpha(Icons.minimize, x - 80, 7, borderCanvas.rawData, x);
                if(x >= GUI.ScreenX && y >= GUI.ScreenY-60) { Font.DrawImageAlpha(Icons.min, x - 50, 7, borderCanvas.rawData, x); }
                else { Font.DrawImageAlpha(Icons.max, x - 50, 7, borderCanvas.rawData, x); }
            }
            else { Font.DrawImageAlpha(Icons.minimize, x - 50, 7, borderCanvas.rawData, x); }
            Font.DrawImageAlpha(Icons.close, x - 20, 7, borderCanvas.rawData, x);
        }
        internal void Notify(string text)
        {
            NotificationSystem.Notify(name, text, icon);
        }
        public void Click(int x2, int y2)
        {
            WindowManager.FocusAtWindow(ID);
            if (y2 - StartY > 30) { OnClicked?.Invoke(x2 - StartX, y2 - StartY - 30); }
        }
        public void Hover(int x2, int y2)
        {
            y2 -= StartY;
            if (y2 > 30) { OnHover?.Invoke(x2 - StartX, y2 - StartY - 30); return; }
            x2 -= StartX;
            if (x2 > x - 24)
            {
                GUI.canvas.DrawImageAlpha(Icons.close2, StartX - 20 + x, StartY + 7);
                if (GUI.Pressed) { Exit(); return; }
            }
            else if (x2 > x - 54)
            {
                if (resizable != WindowManager.Resizable.None) {
                    if (x >= GUI.ScreenX && y >= GUI.ScreenY - 60)
                    {
                        GUI.canvas.DrawImageAlpha(Icons.min2, StartX - 50 + x, StartY + 7);
                        if (GUI.Pressed) { NewSize(ogX, ogY); }
                    }
                    else
                    {
                        GUI.canvas.DrawImageAlpha(Icons.max2, StartX - 50 + x, StartY + 7);
                        if (GUI.Pressed) { NewSize(GUI.ScreenX, GUI.ScreenY); }
                    }
                }
                else
                {
                    GUI.canvas.DrawImageAlpha(Icons.minimize2, StartX - 50 + x, StartY + 7);
                    if (GUI.Pressed) { Minimize(); }
                }
            }
            else if (resizable != WindowManager.Resizable.None && x2 > x - 84)
            {
                GUI.canvas.DrawImageAlpha(Icons.minimize2, StartX - 80 + x, StartY + 7);
                if (GUI.Pressed) { Minimize(); }
            }
            else if(GUI.LongPress)
            {
                StartXOld = StartX - (int)MouseManager.X;
                StartYOld = StartY - (int)MouseManager.Y;
                windowlock = true;
                WindowManager.FocusAtWindow(ID);
                RefreshBorder(false);
                OnStartMoving?.Invoke();
            }
        }
        public void LongPress(int x2, int y2)
        {
            OnLongPressed?.Invoke(x2 - StartX, y2 - StartY - 30);
        }
        public void Minimize()
        {
            if (!minimized) {
                minimized = true;
                Animation2.Animate(borderCanvas).SetDuration(150).StartAt(StartX, StartY).MoveTo(ID * 40 + 50, GUI.ScreenY - 30).SetInterpolator(Animator2.InterpolationMode.EaseIn).Start();
                Animation2.Animate(appCanvas).SetDuration(150).StartAt(StartX, StartY + 30).MoveTo(ID * 40 + 50, GUI.ScreenY).SetInterpolator(Animator2.InterpolationMode.EaseIn).Start();
            }
        }
        public void Unminimize()
        {
            if (minimized) {
                Animation2.Animate(borderCanvas).SetDuration(200).MoveTo(StartX, StartY).StartAt(ID * 40 + 50, GUI.ScreenY - 30).WithEndAction(() => { minimized = false; WindowManager.Draw(this); }).Start();
                Animation2.Animate(appCanvas).SetDuration(200).MoveTo(StartX, StartY + 30).StartAt(ID * 40 + 50, GUI.ScreenY).Start();
            }
        }
        public void NewSize(int nX, int nY)
        {
            if (nY > GUI.ScreenY - 60) { nY = GUI.ScreenY - 60; }
            if (nX > GUI.ScreenX) { nX = GUI.ScreenX; }
            x = nX; y = nY;
            if (nX == GUI.ScreenX) { ogStartX = StartX; StartX = 0; }
            else { StartX = ogStartX; ogY = y; }
            if (nY == GUI.ScreenY - 60) { ogStartY = StartY; StartY = 0; }
            else { StartY = ogStartY; ogX = x; }
            RefreshBorder();
            if (resizable == WindowManager.Resizable.Scale) { return; }
            appCanvas = new Bitmap((uint)nX, (uint)y, GUI.DisplayMode.ColorDepth);
            if (OnSizeChange != null) { OnSizeChange.Invoke(); } else { Start(); }
        }
        public void OnKey(KeyEvent keyEvent)
        {
            switch (keyEvent.Key)
            {
                case ConsoleKeyEx.F4:
                    if (KeyboardManager.AltPressed) { Exit(); return; } break;
                case ConsoleKeyEx.F11:
                    if (resizable != WindowManager.Resizable.None)
                    {
                        if (x >= GUI.ScreenX && y >= GUI.ScreenY - 60) { NewSize(ogX, ogY); }
                        else { NewSize(GUI.ScreenX, GUI.ScreenY); }
                    }
                    break;
            }
            OnKeyPressed?.Invoke(keyEvent);

        }
    }
}