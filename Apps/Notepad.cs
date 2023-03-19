using Cosmos.System;
using System;
using System.Collections.Generic;
using F = NclearOS2.FileManager;

namespace NclearOS2
{
    public class Notepad : Window
    {
        public Notepad(int x, int y, string path = null) : base("Notepad", x, y, Resources.notepad)
        { this.filePath = path; }
        string filePath;
        bool exit;
        public Input inputInstance;
        static List<string> buttons = new()
        {
                "Yes",
                "No"
        };
        internal override bool Start()
        {
            inputInstance = new Input("Notepad", false);
            ProcessManager.Run(inputInstance);
            if (filePath != null) { inputInstance.input = F.Open(filePath); name = "Notepad - " + filePath; }
            return true;
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY + 20, x, y);
            Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, StartX, StartY, x, 20);
            Graphic.EditText(inputInstance, StartX + 10, StartY + 30, Kernel.WhitePen, true);
            Kernel.canvas.DrawString("New  Open  Save  Save as", Kernel.font, Kernel.WhitePen, StartX + 10, StartY + 1);
            if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 10 && MouseManager.X < StartX + x)
            {
                if (MouseManager.X < StartX + 33)
                {
                    Kernel.canvas.DrawString("New                     ", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed) { inputInstance.input = null; }
                }
                else if (MouseManager.X < StartX + 80)
                {
                    Kernel.canvas.DrawString("     Open               ", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed) { ProcessManager.Add(new Files(630, 400)); return false; }
                }
                else if (MouseManager.X < StartX + 130)
                {
                    Kernel.canvas.DrawString("           Save         ", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed)
                    {
                        if (String.IsNullOrWhiteSpace(filePath))
                        {
                            F.Save(F.NewFile("0:\\New Text Document"), inputInstance.input);
                            //Files.SaveContent = Input.input;
                            //Process.Run(Process.Apps.files);
                            //Input.Register(false);
                        }
                        else
                        {
                            F.Save(filePath, inputInstance.input);
                        }
                    }
                }
                else if (MouseManager.X < StartX + 200)
                {
                    Kernel.canvas.DrawString("                 Save as", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed)
                    {
                        Toast.msg = "Not implemented yet";
                        //Files.SaveContent = Input.input;
                        //Process.Run(Process.Apps.files);
                        //Input.Register(false);
                    }
                }
            }
            if (exit) { if (!Exit(id)) { inputInstance.input = null; return false; }; }
            return true;
        }
        internal override int Stop()
        {
            if (inputInstance.input != null) { exit = true; return 1; } else { ProcessManager.RemoveAtService(inputInstance.id); return 0; }
        }
        bool Exit(int id)
        {
            switch(MsgBox.Main(StartWindowX + x / 4, StartWindowY + y / 4, "Are you sure you want to exit?", buttons))
            {
                case 1:
                    return false;
                case 2:
                    exit = false;
                    return true;
            }
            return true;
        }
        internal override void Key(ConsoleKeyEx key) { }
    }
}