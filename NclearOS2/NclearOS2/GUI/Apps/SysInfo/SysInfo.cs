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
        public static uint installedRAM
        {
            get { return CPU.GetAmountOfRAM(); }
        }
        public static uint reservedRAM
        {
            get { return installedRAM - (uint)GCImplementation.GetAvailableRAM(); }
        }
        public static double usedRAM
        {
            get { return (GCImplementation.GetUsedRAM() / (1024.0 * 1024.0)) + reservedRAM; }
        }
        public static uint availableRAM
        {
            get { return installedRAM - (uint)usedRAM; }
        }
        public static string Main()
        {
            return Kernel.GUIenabled ? Kernel.OSVERSION + "\nDisplay: " + GUI.GUI.canvas.Mode
                //+ "\nCPU: " + CPU.GetCPUBrandString()                                                                                                              doesnt want to work
                : Kernel.OSVERSION + "\nConsole Display: " + System.Console.WindowWidth + "x" + System.Console.WindowHeight + "\nCPU: " + CPU.GetCPUBrandString(); //but with canvas disabled it does work
        }
        public static string Ram()
        {
            return "RAM: " + usedRAM.ToString("0.00") + "/" + installedRAM.ToString("0.00") + " MB (" + (int)((usedRAM / installedRAM) * 100) + "%)";
        }
    }
}
namespace NclearOS2.GUI
{
    internal class Sysinfo: Window
    {
        internal Sysinfo() : base("System Info", 500, 100, new Bitmap(Resources.SysInfo), Priority.High) { }
        internal override int Start()
        {
            MemoryOperations.Fill(appCanvas.rawData, GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            DrawString(NclearOS2.Sysinfo.Main(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 10);
            DrawString(NclearOS2.Sysinfo.Ram(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 70);
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