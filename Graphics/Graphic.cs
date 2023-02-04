using Cosmos.HAL;
using Cosmos.System.Graphics;

namespace NclearOS2
{
    public static class Graphic
    {
        public static void DisplayText(string text, int x, int y, Pen pen, bool Cursor = false, bool forceDisableOptimizations = false)
        {
            if (text.Contains('\n'))
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (Cursor && RTC.Second % 2 == 0 && i == lines.Length - 1)
                    {
                        Kernel.canvas.DrawString(lines[i] + '_', Kernel.font, pen, x, y);
                    }
                    else
                    {
                        Kernel.canvas.DrawString(lines[i], Kernel.font, pen, x, y);
                    }
                    y += 16;
                }
            }
            else
            {
                if (Cursor && RTC.Second % 2 == 0)
                {
                    Kernel.canvas.DrawString(text + '_', Kernel.font, pen, x, y);
                }
                else { Kernel.canvas.DrawString(text, Kernel.font, pen, x, y); }

            }
        }
    }
}