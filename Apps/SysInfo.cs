using Cosmos.Core;

namespace NclearOS2
{
    public static class Sysinfo
    {
        public static string Main()
        {
            return Kernel.CurrentVersion
            + "\nDisplay: " + Kernel.canvas.Mode
            + "\nRAM: " + CPU.GetEndOfKernel() / 1000000 + " MB / " + CPU.GetAmountOfRAM() + " MB";
            //+"\nCPU: " + CPU.GetCPUBrandString()
            //+ "\nCPU Vender: " + CPU.GetCPUVendorName()
            //+ "\nCPU Uptime: " + CPU.GetCPUUptime();
        }
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY, x, y);
            Graphic.DisplayText(Main(), StartX + 10, StartY + 10, Kernel.WhitePen);
        }
    }
}