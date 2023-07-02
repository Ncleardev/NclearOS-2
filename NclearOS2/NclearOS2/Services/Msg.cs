using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NclearOS2
{
    public class Msg
    {
        public static void Main(string title, string text, Bitmap icon = null)
        {
            if (Kernel.GUIenabled)
            {
                GUI.ProcessManager.Run(new GUI.MsgWindow(title, text, icon));
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
        public static string msg
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
                if (GUI.GUI.StartClick) { text = null; }
            }
        }
        public static void Display(string value)
        {
            GUI.GUI.canvas.DrawFilledRectangle(GUI.GUI.DarkPen, ((int)(GUI.GUI.displayMode.Columns - value.Length * 8) / 2) - 10, (int)(GUI.GUI.displayMode.Rows - 12) / 2, value.Length * 8 + 20, 24);
            GUI.Font.DrawString(value, Color.White, (int)(GUI.GUI.displayMode.Columns - value.Length * 8) / 2, (int)(GUI.GUI.displayMode.Rows - 1) / 2);
        }
        public static void Force(string value)
        {
            if (Kernel.GUIenabled)
            {
                Display(value); GUI.GUI.canvas.Display(); Toast.msg = value;
            }
            else
            {
                System.Console.WriteLine(value);
            }
        }
    }
}
namespace NclearOS2.GUI
{
    internal class MsgWindow : Window
    {
        private string param;
        public MsgWindow(string title, string text, Bitmap icon) : base(title, text.Length*GUI.font.Width + 10, 70, icon, Priority.None)
        {
            param = text;
        }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            DrawString(param, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 5, 5);
            return 0;
        }
        internal override void Update() { }
        internal override int Stop() { return 0; }
    }
}