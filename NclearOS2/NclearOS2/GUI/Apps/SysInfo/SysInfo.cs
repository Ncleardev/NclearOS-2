using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using NclearOS2.Commands;
using System;
using System.Collections;
using System.Drawing;

namespace NclearOS2
{
    public static class Sysinfo
    {
        public static string CPUname;
        public static uint InstalledRAM
        {
            get { return CPU.GetAmountOfRAM(); }
        }
        public static uint ReservedRAM
        {
            get { return InstalledRAM - (uint)GCImplementation.GetAvailableRAM(); }
        }
        public static double UsedRAM
        {
            get { return (GCImplementation.GetUsedRAM() / (1024.0 * 1024.0)) + ReservedRAM; }
        }
        public static uint AvailableRAM
        {
            get { return InstalledRAM - (uint)UsedRAM; }
        }
        public static string CPUuptime
        {
            get { return TimeSpan.FromSeconds(CPU.GetCPUUptime() / 3200000000).ToString(@"hh\:mm\:ss"); }
        }
        public static string DisplayRes
        {
            get { return Kernel.GUIenabled ? "Display: " + GUI.GUI.canvas.Mode : "Console Display: " + System.Console.WindowWidth + "x" + System.Console.WindowHeight; }
        }
        public static string Main()
        {
            return "OS: " + Kernel.OSVERSION + "\n" + DisplayRes +"\nCPU: " + CPUname + "\nCPU Uptime: " + CPUuptime;
        }
        public static string Ram()
        {
            return "RAM: " + UsedRAM.ToString("0.00") + "/" + InstalledRAM.ToString("0.00") + " MB (" + (int)((UsedRAM / InstalledRAM) * 100) + "%)";
        }
    }
}
namespace NclearOS2.GUI
{
    internal class InfoSystem: Window
    {
        internal InfoSystem() : base("System Info", 500, 200, new Bitmap(Resources.InfoSystemIcon), ProcessManager.Priority.High) { }
        internal override int Start()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            DrawString(NclearOS2.Sysinfo.Main(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 10);
            DrawString("FPS: " + GUI.fps + " | Frametime: " + (1000.0f / GUI.fps).ToString("0.0") + " ms", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 90);
            DrawString(NclearOS2.Sysinfo.Ram(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 110);
        }
    }

    internal class InfoDisplay : Process
    {
        public InfoDisplay() : base("Performance Display", ProcessManager.Priority.High) { }
        public static Bitmap bg;
        public static Bitmap display;
        public static Process infoDisplay;
        internal override int Start()
        {
            bg = PostProcess.CropBitmap(Images.wallpaperBlur, 0, 0, 240, 35);
            display = new(240, 35, GUI.DisplayMode.ColorDepth);
            return 0;
        }
        internal override void Update()
        {
            MemoryOperations.Copy(display.rawData, bg.rawData);
            Font.DrawString(Sysinfo.Ram(), Color.White.ToArgb(), 2, 2, display.rawData, 240);
            Font.DrawString(GUI.fps + " FPS | " + (1000.0f / GUI.fps).ToString("0.0") + " ms", Color.White.ToArgb(), 2, 18, display.rawData, 240);
        }
        internal override int Stop(bool force = false)
        {
            display = null;
            return 0;
        }
    }
}
namespace NclearOS2.Commands
{
    internal class SystemInfo : CommandsTree
    {
        internal SystemInfo() : base
            ("System Info", "Provides info about system",
            new Command[] {
            new Command(new string[] { "sysinfo", "systeminfo", "sys", "system"}, "Display info about system")
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell, string rawInput)
        {
            if (args[0] == "sysinfo" || args[0] == "systeminfo" || args[0] == "sys" || args[0] == "system")
            {
                shell.Print = NclearOS2.Sysinfo.Main() + "\n" + NclearOS2.Sysinfo.Ram();
                return 0;
            }
            return 1;
        }
    }
}