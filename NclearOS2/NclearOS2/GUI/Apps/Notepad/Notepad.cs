using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using static System.Collections.Specialized.BitVector32;

namespace NclearOS2.GUI
{
    internal class Notepad : Window
    {
        private string Open = "new";

        private bool unSaved;
        private bool UnSaved
        {
            get { return unSaved; }
            set
            {
                unSaved = value;
                SetWindowName();
            }
        }
        private string input;
        private string Input
        {
            get { return input; }
            set
            {
                UnSaved = true;
                if (value.Length < input.Length) { input = value; UI(); }
                else { input = value; DrawString(input, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 30); }
            }
        }
        internal Notepad(int x, int y, string path = "") : base("Notepad - new", x, y, new Bitmap(Resources.Notepad), ProcessManager.Priority.None) { OnKeyPressed = Key; OnClicked = OnPressed; OnSizeChange = UI; if (!string.IsNullOrWhiteSpace(path)) Open = path; }

        internal override int Start()
        {
            UI();
            if (!string.IsNullOrWhiteSpace(Open) && Open != "new") { Input = FileManager.Open(Open); UnSaved = false; }
            SetWindowName();
            return 0;
        }
        internal override int Stop(bool f)
        {
            if (f || !(unSaved && !string.IsNullOrEmpty(input))) return 0;
            return Warning(new Action(() => { Exit(true); }));
        }
        private void Key(KeyEvent key)
        {
            switch (key.Key)
            {
                case ConsoleKeyEx.Enter:
                    Input += '\n';
                    break;
                case ConsoleKeyEx.Backspace:
                    if (Input.Length > 0)
                    {
                        Input = Input.Remove(Input.Length - 1);
                    }
                    break;
                default:
                    if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || (key.KeyChar == ' '))
                    { Input += key.KeyChar; }
                    break;
            }
        }
        private void OnPressed(int x, int y)
        {
            if (y < 20)
            {
                if (x < 33) { Warning(new Action(() => { ProcessManager.Run(new Notepad((int)(GUI.ScreenX - 200), (int)(GUI.ScreenY - 170))); Exit(true); })); }
                else if (x < 80) { Warning(new Action(() => { ProcessManager.Run(new Files(640, (int)(GUI.ScreenY - 170))); Exit(true); 
                })); }
                else if (x < 130) { Save(); }
            }
        }
        private void UI()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            DrawString("New  Open  Save", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 5);
            DrawHorizontalLine(Color.White.ToArgb(), "New  Open  Save".Length * Font.fontX + 15, 12, x - 10);
            DrawString(input, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 30);
        }
        private int Warning(Action action)
        {
            if (unSaved && !string.IsNullOrEmpty(input))
            {
                Msg.Main("Notepad", "Save unsaved changes?", Icons.warn, new Option[] { new("Cancel"), new("Don't save", action), new("Save", (new Action(() => { Save(); action.Invoke(); }))) });
                return 1;
            }
            action.Invoke();
            return 0;
        }
        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Open) || Open == "new") { Open = FileManager.Save(FileManager.NewFile("0:\\New Text Document"), Input); }
            else { FileManager.Save(Open, Input); }
            UnSaved = false;
            name = name.ToString();
        }
        private void SetWindowName()
        {
            name = unSaved ? "Notepad - *" + Open : "Notepad - " + Open;
        }
    }
}