using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using NclearOS2.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NclearOS2.GUI
{
    internal class ConsoleApp : Window
    {
        private string Input;
        private string input
        {
            get { return Input; }
            set
            {
                if (shell != null)
                {
                    if (value.Length < Input.Length) { Input = value; UI(); }
                    Input = value;
                    DrawStringAlpha(Input, Color.LimeGreen.ToArgb(), shell.prompt.Length * GUI.font.Width + 20, 10);
                }
            }
        }
        private List<string> history = new();
        private int position = 0;
        private CommandShell shell;
        private Bitmap bg;
        private static bool blurBg = GUI.blurEffects;
        internal ConsoleApp(int x, int y) : base("Console", x, y, new Bitmap(Resources.ConsoleIcon), ProcessManager.Priority.None) { OnKeyPressed = Key; OnSizeChange = Moved; OnMoved = Moved; OnStartMoving = StartMoving; }
        internal override void Update() { throw new Exception("Manual Crash"); } //ExecuteError
        internal override int Start()
        {
            shell = new CommandShell { crashClient = ExecuteError, update = Result, exit = ExitConsole };
            Moved();
            return 0;
        }
        private void ExitConsole() { Exit(); }
        internal override int Stop(bool f) { shell = null; return 0; }
        private void Key(KeyEvent key)
        {
            if(shell == null) { return; }
            switch (key.Key)
            {
                case ConsoleKeyEx.Escape:
                    input = null;
                    break;
                case ConsoleKeyEx.Enter:
                    GUI.Loading = true;
                    GUI.Refresh();
                    history.Add(input);
                    try { if(shell.Execute(input) == 2) { shell.Print += "Wrong parameters usage, use 'help [command]' for a list of available parameters."; } }
                    catch (Exception e) { shell = null; HandleShellCrash(e.Message); }
                    position = 0;
                    input = null;
                    GUI.Loading = false;
                    break;
                case ConsoleKeyEx.UpArrow:
                    if (history.Count - position > 0)
                    {
                        position++;
                        UI();
                        input = history[history.Count - position];
                    }
                    break;
                case ConsoleKeyEx.DownArrow:
                    if (position > 1)
                    {
                        position--;
                        UI();
                        input = history[history.Count - position];
                    }
                    break;
                case ConsoleKeyEx.Backspace:
                    if (input.Length > 0)
                    {
                        input = input.Remove(input.Length - 1);
                    }
                    break;
                case ConsoleKeyEx.F1:
                    input = null;
                    position = 0;
                    shell.Execute("help");
                    break;
                default:
                    if (char.IsLetterOrDigit(key.KeyChar) || char.IsPunctuation(key.KeyChar) || char.IsSymbol(key.KeyChar) || (key.KeyChar == ' '))
                    { input += key.KeyChar; }
                    break;
            }
        }
        private void Result()
        {
            UI();
            GUI.Refresh();
        }
        private void ExecuteError()
        {
            this.priority = ProcessManager.Priority.Realtime;
        }
        private void HandleShellCrash(string err = null)
        {
            if(!string.IsNullOrEmpty(err)) { Input = err; }
            Background();
            DrawString("Command Shell crashed: " + Input + "; Restart Console.", Color.White.ToArgb(), Color.Red.ToArgb(), 10, 10);
        }
        private void UI(bool disableBlur = false)
        {
            if(shell == null) { HandleShellCrash(); return; }
            if (!disableBlur && blurBg) { MemoryOperations.Copy(appCanvas.rawData, bg.rawData); } else { Background(0); }
            DrawStringAlpha(shell.prompt, Color.LimeGreen.ToArgb(), 10, 10);
            DrawStringAlpha(Input, Color.LimeGreen.ToArgb(), shell.prompt.Length * GUI.font.Width + 20, 10);
            DrawStringAlpha(shell.Print, Color.White.ToArgb(), 10, 35);
        }
        private void Moved() {
            bg = PostProcess.CropBitmap(Images.wallpaperDark, StartX, StartY + 30, x, y);
            UI();
        }
        private void StartMoving() { UI(true); }
    }
}