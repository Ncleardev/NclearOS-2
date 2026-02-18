using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NclearOS2
{
    public class Option
    {
        public Option(string text, Action action = null) { this.text = text; this.action = action; }
        public string text;
        public Action action;
    }
    public static class Msg
    {
        public static void Main(string title, string text, Bitmap icon = null, Option[] options = null)
        {
            if (Kernel.GUIenabled)
            {
                GUI.ProcessManager.Run(new GUI.MsgWindow(title, text, icon, options ?? (new Option[] { new("OK") })));
            }
            else
            {
                System.Console.WriteLine(title + ": " + text);
            }
        }
    }
    public static class Toast
    {
        private static string text;
        public static string Msg
        {
            set
            {
                if (Kernel.GUIenabled)
                {
                    text = value;
                }
                else { System.Console.WriteLine(value); }
            }
        }
        public static void Update()
        {
            if (text != null)
            {
                Display(text);
                if (GUI.GUI.LongPress) { text = null; }
            }
        }
        public static void Display(string value)
        {
            GUI.GUI.canvas.DrawFilledRectangle(GUI.GUI.DarkPen, ((int)(GUI.GUI.ScreenX - value.Length * 8) / 2) - 10, (int)(GUI.GUI.ScreenY - 12) / 2, value.Length * 8 + 20, 24);
            GUI.Font.DrawString(value, Color.White, (int)(GUI.GUI.ScreenX - value.Length * 8) / 2, (int)(GUI.GUI.ScreenY - 1) / 2);
        }
        public static void Force(string value)
        {
            if (Kernel.GUIenabled)
            {
                Display(value); GUI.GUI.canvas.Display(); Toast.Msg = value;
            }
            else
            {
                System.Console.WriteLine(value);
            }
        }
        public static void Debug(string value)
        {
            if (Kernel.GUIenabled)
            {
                Display(value); GUI.GUI.canvas.Display(); System.Console.ReadKey();
            }
            else
            {
                System.Console.WriteLine(value); System.Console.ReadKey();
            }
        }
    }
}
namespace NclearOS2.GUI
{
    internal class MsgWindow : Window
    {
        private string param;
        private Option[] options;
        public MsgWindow(string title, string text, Bitmap icon, Option[] options) : base(title, text.Length*GUI.font.Width + 10, 71, icon, ProcessManager.Priority.None)
        {
            int w = 10;
            foreach (Option option in options) { w += option.text.Length * Font.fontX + 20; }
            if (w > x) { NewSize(w, y); StartX = StartXOld = GUI.ScreenX / 2; StartY = StartYOld = GUI.ScreenY / 2; }
            param = text;
            this.options = options;
            OnClicked = Clicked;
        }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            DrawString(param, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 5, 5);
            int w = x;
            foreach (var option in options)
            {
                int w2 = option.text.Length * Font.fontX + 20;
                w -= w2;
                DrawFilledRectangle(GUI.SystemPen.ValueARGB, w, y - 30, w2-10, 25);
                DrawStringAlpha(option.text, -1, w+5, y - 25);
            }
            return 0;
        }


        void Clicked(int x, int y)
        {
            if (y >= this.y - 30 && y <= this.y - 5)
            {
                int w = this.x;
                foreach (var option in options)
                {
                    int w2 = option.text.Length * Font.fontX + 20;
                    w -= w2;
                    if (x >= w && x <= w + w2 - 10) { option.action?.Invoke(); Exit(); return; }
                }
            }
        }

    }
    public static class NotificationSystem
    {
        public static List<(string title, string text, Bitmap icon)> queue = new();
        private static Bitmap bg;
        public static void Notify(string title, string text, Bitmap icon = null)
        {
            if (Kernel.GUIenabled)
            {
                Bitmap bitmap = new(500, 100, GUI.DisplayMode.ColorDepth);
                MemoryOperations.Copy(bitmap.rawData, bg.rawData);
                if (icon == null) { Font.DrawString(title, Color.White.ToArgb(), 10, 10, bitmap.rawData, 500); }
                else { Font.DrawImageAlpha(icon, 10, 5, bitmap.rawData, 500); Font.DrawString(title, Color.White.ToArgb(), 20 + (int)icon.Width, 10, bitmap.rawData, 500); }
                Font.DrawString(text, Color.White.ToArgb(), 10, 35, bitmap.rawData, 500);
                int xFixed = (int)GUI.ScreenX - 510;
                int yHidden = (int)GUI.ScreenY;
                int yVisible = (int)GUI.ScreenY - 140;
                Animation2.Animate(bitmap)
                    .StartAt(xFixed, yHidden)
                    .MoveTo(xFixed, yVisible)
                    .SetDuration(200)
                    .Queue();
                Animation2.Animate(bitmap)
                    .StartAt(xFixed, yVisible)
                    .MoveTo(xFixed, yVisible)
                    .SetDuration(3000)
                    .Queue();
                Animation2.Animate(bitmap)
                    .StartAt(xFixed, yVisible)
                    .MoveTo(xFixed, yHidden)
                    .SetDuration(150)
                    .SetInterpolator(Animator2.InterpolationMode.EaseIn)
                    .Queue();
            }
            else
            {
                System.Console.WriteLine(title);
                System.Console.WriteLine(text);
            }
        }
        public static void RefreshBG()
        {
            bg = PostProcess.CropBitmap(Images.wallpaperBlur, GUI.ScreenX - 510, GUI.ScreenY - 140, 500, 100);
        }
    }
}