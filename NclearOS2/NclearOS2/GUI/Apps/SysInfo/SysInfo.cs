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
        public static string Main()
        {
            TimeSpan uptime = TimeSpan.FromSeconds(CPU.GetCPUUptime() / 3200000000);
            return Kernel.GUIenabled ? Kernel.OSVERSION + "\nDisplay: " + GUI.GUI.canvas.Mode                                                    + "\nCPU: " + CPUname + "\nCPU Uptime: " + uptime.ToString(@"hh\:mm\:ss")
                                     : Kernel.OSVERSION + "\nConsole Display: " + System.Console.WindowWidth + "x" + System.Console.WindowHeight + "\nCPU: " + CPUname + "\nCPU Uptime: " + uptime.ToString(@"hh\:mm\:ss");
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
        internal InfoSystem() : base("SysInfo", 500, 200, new Bitmap(Resources.InfoSystemIcon), Priority.High) { }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            DrawString(NclearOS2.Sysinfo.Main(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 10);
            DrawString("FPS: " + GUI.fps, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 90);
            DrawString(NclearOS2.Sysinfo.Ram(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 110);
        }

        internal override int Stop() { return 0; }
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
        internal override int Execute(string[] args, CommandShell shell)
        {
            if (args[0] == "sysinfo" || args[0] == "systeminfo" || args[0] == "sys" || args[0] == "system")
            {
                shell.print = NclearOS2.Sysinfo.Main() + "\n" + NclearOS2.Sysinfo.Ram();
                return 0;
            }
            return 1;
        }
    }
}