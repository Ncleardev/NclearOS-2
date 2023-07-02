using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace NclearOS2.GUI
{
    internal class Notepad : Window
    {
        private string Input;
        private string input
        {
            get { return Input; }
            set
            {
                if (value.Length < Input.Length) { Background(GUI.DarkGrayPen.ValueARGB); }
                Input = value;
                DrawString(Input, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 10);
            }
        }
        internal Notepad(int x, int y) : base("Notepad", x, y, new Bitmap(Resources.Notepad), Priority.None) { OnKeyPressed = Key; }
        internal override void Update() { }
        internal override int Start() { Background(GUI.DarkGrayPen.ValueARGB); return 0; }
        internal override int Stop() { return 0; }
        private void Key(KeyEvent key)
        {
            switch (key.Key)
            {
                case ConsoleKeyEx.Enter:
                    input += '\n';
                    break;
                case ConsoleKeyEx.Backspace:
                    if (input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1);
                    }
                    break;
                default:
                    if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || (key.KeyChar == ' '))
                    { if ((input.Length - input.LastIndexOf('\n') - 1) * Font.fontX + 30 > x) { input += '\n'; } input += key.KeyChar; }
                    break;
            }
        }
    }
}