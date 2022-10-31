using Cosmos.System;

namespace NclearOS2
{
    public class Notepad
    {
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.WhitePen, StartX, StartY, x, y);
            Input.wX = StartX + 10;
            Input.wY = StartY + 10;
            Input.TextPen = Kernel.DarkPen;
            if (Input.ready)
            {
                Input.ready = false;
                Input.input += "\n";
                Input.position = 0;
            }
            else
            {
                Input.Main();
            }
        }
    }
}