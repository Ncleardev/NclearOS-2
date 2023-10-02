using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Drawing;

namespace NclearOS2.GUI
{
    internal class Notepad : Window
    {
        private string Input;
        private string open = "";
        private string input
        {
            get { return Input; }
            set
            {
                if (value.Length < Input.Length) { Background(GUI.DarkGrayPen.ValueARGB); DrawString("New  Open  Save", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 5); DrawHorizontalLine(Color.White.ToArgb(), "New  Open  Save".Length * Font.fontX + 15, 12, x - 10); }
                Input = value;
                DrawString(Input, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 30);
            }
        }
        internal Notepad(int x, int y, string path = "") : base("Notepad", x, y, new Bitmap(Resources.Notepad), Priority.None) { OnKeyPressed = Key; OnClicked = OnPressed; open = path; }
        internal override void Update() { }
        internal override int Start()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            DrawString("New  Open  Save", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 5);
            DrawHorizontalLine(Color.White.ToArgb(), "New  Open  Save".Length * Font.fontX + 15, 12, x - 10);
            if (!string.IsNullOrWhiteSpace(open)) { name = "Notepad - " + open; RefreshBorder(); input = FileManager.Open(open); }
            return 0;
        }
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
        private void OnPressed(int x, int y)
        {
            if (y < 20)
            {
                if (x < 33) { input = ""; }
                else if (x < 80) { ProcessManager.RemoveAt(id); ProcessManager.Run(new Files(640, (int)(GUI.screenY - 170))); }
                else if (x < 130)
                {
                    if (string.IsNullOrWhiteSpace(open)) { FileManager.Save(FileManager.NewFile("0:\\New Text Document"), input); }
                    else { FileManager.Save(open, input); }
                    
                }
            }
        }
    }
}