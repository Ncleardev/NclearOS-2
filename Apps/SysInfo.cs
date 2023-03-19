using Cosmos.Core;
using Cosmos.System;

namespace NclearOS2
{
    public class Sysinfo : Window
    {
        public Sysinfo() : base("System Info", 500, 100, Resources.sysinfo) { }
        internal override bool Start()
        {
            return true;
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY, x, y);
            Graphic.TextView(Main(), StartX + 5, StartY + 5, Kernel.WhitePen);
            return true;
        }
        public static string Main()
        {
            return Kernel.CurrentVersion
            + "\nDisplay: " + Kernel.canvas.Mode
            + "\nCPU: " + CPU.GetCPUBrandString()
            + "\nRAM: " + GCImplementation.GetUsedRAM() / 1048576 + " MB / " + CPU.GetAmountOfRAM() + " MB";
        }
        internal override int Stop() { return 0; }
        internal override void Key(ConsoleKeyEx key) { }
    }
}