using Cosmos.System;
using System;

namespace NclearOS2
{
    public class Notepad
    {
        public static string filePath;
        public static void Update(int StartX, int StartY, int x, int y)
        {
            if (!String.IsNullOrWhiteSpace(filePath)) { Window.name = "Notepad - " + filePath; }
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY+20, x, y);
            Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, StartX, StartY, x, 20);
            Graphic.DisplayText(Input.input, StartX + 10, StartY + 30, Kernel.WhitePen, true);
            Kernel.canvas.DrawString("New  Open  Save  Save as", Kernel.font, Kernel.WhitePen, StartX + 10, StartY + 1);
            if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 10 && MouseManager.X < StartX + x)
            {
                if (MouseManager.X < StartX+33)
                {
                    Kernel.canvas.DrawString("New                     ", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed) { Input.input = ""; }
                }else if (MouseManager.X < StartX + 80)
                {
                    Kernel.canvas.DrawString("     Open               ", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed) { Process.Run(Process.Apps.files); }
                }
                else if (MouseManager.X < StartX + 130)
                {
                    Kernel.canvas.DrawString("           Save         ", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed)
                    {
                        if (String.IsNullOrWhiteSpace(filePath))
                        {
                            Files.Save(Files.NewFile("0:\\New Text Document"), Input.input);
                            return;
                            Files.SaveContent = Input.input;
                            Process.Run(Process.Apps.files);
                            Input.Register(false);
                        }
                        else
                        {
                            Files.Save(filePath, Input.input);
                        } 
                    }
                }
                else if (MouseManager.X < StartX + 200)
                {
                    Kernel.canvas.DrawString("                 Save as", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                    if (Kernel.Pressed)
                    {
                        Msg.Main("Not implemented yet");
                        return;
                        Files.SaveContent = Input.input;
                        Process.Run(Process.Apps.files);
                        Input.Register(false);
                    }
                }
            }
        }
    }
}