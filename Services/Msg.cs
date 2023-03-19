using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NclearOS2
{
    public class Msg : Window
    {
        public string param;
        public Msg(string title, string param, Bitmap icon) : base(title, param.Length*10, 70, icon)
        {
            this.param = param;
        }
        internal override bool Start()
        {
            return true;
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY, x, y);
            Graphic.TextView(param, StartX + 5, StartY + 5, Kernel.WhitePen);
            if(Graphic.Button("OK", StartX + x - 72, StartY + y - 30)) { return false; }
            return true;
        }
        internal override int Stop() { return 0; }
        internal override void Key(ConsoleKeyEx key) { }
        public static void Main(string title, string text, Bitmap icon)
        {
            ProcessManager.Add(new Msg(title, text, icon));
        }
    }
    public class MsgBox
    {
        public static int Main(int StartX, int StartY, string param, List<string> buttons)
        {
            Kernel.canvas.DrawFilledRectangle(Color.FromArgb(50, 50, 50), StartX, StartY, param.Length*8+10, 50);
            Graphic.TextView(param, StartX + 5, StartY + 5, Kernel.WhitePen);
            for (int i = 0; i < buttons.Count; i++)
            {
                if (Graphic.Button(buttons[i], (StartX + param.Length * 8) - i*70 -50, StartY + 20)) { return i+1; }
            }
            return 0;
        }
    }
    public static class Toast
    {
        public static string msg;
        public static void Update()
        {
            if(msg != null)
            {
                Display(msg);
            }
        }
        public static void Display(string msg)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkPen, ((int)(Kernel.screenX - msg.Length * 8) / 2) - 10, (int)(Kernel.screenY - 12) / 2, msg.Length * 8 + 20, 24);
            Kernel.canvas.DrawString(msg, Kernel.font, Kernel.WhitePen, (int)(Kernel.screenX - msg.Length * 8) / 2, (int)(Kernel.screenY - 1) / 2);
        }
        public static void Force(string msg)
        {
            Display(msg); Kernel.canvas.Display(); Toast.msg = msg;
        }
    }
}