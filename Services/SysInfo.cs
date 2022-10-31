using Cosmos.Core;

namespace NclearOS2
{
    public static class Sysinfo
    {
        public static string Main()
        {
            return "OS: " + Kernel.CurrentVersion
            + "\nDisplay: " + Kernel.canvas.Mode
            + "\nRAM: " + CPU.GetEndOfKernel() / 1000000 + " MB / " + CPU.GetAmountOfRAM() + " MB"
            + "\nCPU: " + CPU.GetCPUBrandString()
            + "\nCPU Vender: " + CPU.GetCPUVendorName()
            + "\nCPU Uptime: " + CPU.GetCPUUptime();
        }
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.GrayPen, StartX, StartY, x, y);
            Window.DisplayText(Main(), StartX+5, StartY+5, Kernel.WhitePen);
        }
    }
}