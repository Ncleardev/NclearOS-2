using Cosmos.System;

namespace NclearOS2
{
    public class ConsoleApp
    {
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkPen, StartX, StartY, x, y);
            Kernel.canvas.DrawChar('>', Kernel.font, Kernel.WhitePen, StartX+10, StartY + 10);
            if (Input.Listener()) { CommandExecute.Main(Input.input); Input.input = null; }
            Graphic.DisplayText(CommandExecute.result, StartX + 10, StartY + 34, Kernel.WhitePen);
            Graphic.DisplayText(Input.input, StartX + 24, StartY + 10, Kernel.WhitePen, true);
        }
    }
}