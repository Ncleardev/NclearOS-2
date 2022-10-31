using Cosmos.System;

namespace NclearOS2
{
    public class ConsoleApp
    {
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkPen, StartX, StartY, x, y);
            Kernel.canvas.DrawChar('>', Kernel.font, Kernel.WhitePen, StartX+10, StartY + 10);
            Window.DisplayText(CommandExecute.result, StartX + 10, StartY + 34, Kernel.WhitePen);
            Input.wX = StartX + 24;
            Input.wY = StartY + 10;
            Input.TextPen = Kernel.WhitePen;
            if (Input.ready)
            {
                Input.ready = false;
                CommandExecute.Main(Input.input);
                Input.input = "";
                Input.position = 0;
            }
            else
            {
                Input.Main();
            }
        }
    }
}