namespace NclearOS2.GUI
{
    internal class PerformanceWatchdog : Process
    {
        public PerformanceWatchdog() : base("Performance Monitor", ProcessManager.Priority.Low) { }
        private static bool acknowledged1;
        private static bool acknowledged2;
        private static bool acknowledged3;
        internal override int Start()
        {
            if (NclearOS2.Sysinfo.InstalledRAM < 120) { Notify("Less than 128 MB RAM detected; blur effects disabled. System stability may be impacted.", Icons.warn); }
            else if (NclearOS2.Sysinfo.InstalledRAM < 250) { Notify("Less than 256 MB RAM detected; System stability may be impacted.", Icons.info); }
            return 0;
        }
        internal override void Update()
        {
            if ((int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100) > 79)
            {
                if (!acknowledged1)
                {
                    Notify("Over 80% of system memory is being used. Close unused apps to maintain stability and performance.", Icons.info); acknowledged1 = true;
                }
                if ((int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100) > 89)
                {
                    if (!acknowledged2)
                    {
                        this.priority = ProcessManager.Priority.Realtime;
                        Notify("Over 90% of system memory is being used and the system may crash unexpectedly. Save all work in progress and restart the system.", Icons.warn); acknowledged2 = true;
                        Images.wallpaper = new((uint)GUI.DisplayMode.Columns, (uint)GUI.DisplayMode.Rows, GUI.DisplayMode.ColorDepth);
                        Images.wallpaperBlur = new((uint)GUI.DisplayMode.Columns, (uint)GUI.DisplayMode.Rows, GUI.DisplayMode.ColorDepth);
                    }
                    if ((int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100) > 94)
                    {
                        if (!acknowledged3)
                        {
                            Toast.Debug("Over 95% of system memory is being used. The system may crash unexpectedly soon. GUI will be disabled.");
                            Kernel.GUIenabled = false;
                            acknowledged3 = true;
                        }
                    }
                }
            }
        }

    }
}