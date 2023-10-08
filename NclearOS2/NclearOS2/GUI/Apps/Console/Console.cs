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
                    if (value.Length < Input.Length) { UI(); }
                    Input = value;
                    DrawString(Input, Color.White.ToArgb(), Color.Black.ToArgb(), shell.prompt.Length * GUI.font.Width + 12, 10);
                }
            }
        }
        private List<string> history = new();
        private int position = 0;
        private CommandShell shell;
        internal ConsoleApp(int x, int y) : base("Console", x, y, new Bitmap(Resources.ConsoleIcon), Priority.None) { OnKeyPressed = Key; }
        internal override void Update() { throw new Exception("Manual Crash"); }
        internal override int Start()
        {
            shell = new CommandShell { crashClient = ExecuteError, update = Result };
            UI();
            return 0;
        }
        internal override int Stop() { shell = null; return 0; }
        private void Key(KeyEvent key)
        {
            switch (key.Key)
            {
                case ConsoleKeyEx.Escape:
                    input = null;
                    break;
                case ConsoleKeyEx.Enter:
                    GUI.Loading = true;
                    GUI.Refresh();
                    history.Add(input);
                    try { if(shell.Execute(input) == 2) { shell.print += "Wrong parameter"; } }
                    catch (Exception e) { shell = null; HandleShellCrash(e.Message); }
                    input = null;
                    GUI.Loading = false;
                    break;
                case ConsoleKeyEx.UpArrow:
                    if (history.Count - position > 0)
                    {
                        ++position;
                        input = history[history.Count - position];
                    }
                    break;
                case ConsoleKeyEx.DownArrow:
                    if (position > 1)
                    {
                        --position;
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
            this.priority = Priority.Realtime;
        }
        private void HandleShellCrash(string err)
        {
            Background();
            DrawString("Command Shell crashed: " + err + "; Restart Console.", Color.White.ToArgb(), Color.Black.ToArgb(), 10, 10);
        }
        private void UI()
        {
            Background();
            DrawString(shell.prompt, Color.White.ToArgb(), 0, 10, 10);
            DrawString(shell.print, Color.White.ToArgb(), 0, 10, 35);
        }
    }
}