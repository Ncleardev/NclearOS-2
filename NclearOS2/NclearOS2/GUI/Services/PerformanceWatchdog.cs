namespace NclearOS2.GUI
{
    internal class PerformanceWatchdog : Process
    {
        public PerformanceWatchdog() : base("Performance Monitor", Priority.Low) { }
        private static bool acknowledged1;
        private static bool acknowledged2;
        private static bool acknowledged3;
        internal override int Start()
        {
            if (NclearOS2.Sysinfo.InstalledRAM < 250) { Toast.msg = "Installed system memory is lower than the recommended 256MB. System stability may be impacted."; } //to do - notification system
            return 0;
        }
        internal override void Update()
        {
            if ((int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100) > 79)
            {
                if (!acknowledged1)
                {
                    Msg.Main("Performance Monitor", "Over 80% of system memory is being used. Close unused apps to maintain stability and performance.", Icons.info); acknowledged1 = true;
                }
                if ((int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100) > 89)
                {
                    if (!acknowledged2)
                    {
                        Msg.Main("Warning", "Over 90% of system memory is being used and the system may crash unexpectedly. Save all work in progress and restart the system.", Icons.warn); acknowledged2 = true;
                    }
                    if ((int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100) > 94)
                    {
                        if (!acknowledged3)
                        {
                            Images.wallpaper = new((uint)GUI.displayMode.Columns, (uint)GUI.displayMode.Rows, GUI.displayMode.ColorDepth);
                            Images.wallpaperBlur = new((uint)GUI.displayMode.Columns, (uint)GUI.displayMode.Rows, GUI.displayMode.ColorDepth);
                            Msg.Main("Warning", "Over 95% of system memory is being used. The system may crash unexpectedly soon.", Icons.warn); acknowledged3 = true;
                        }
                    }
                }
            }
        }
        internal override int Stop() { return 0; }
    }
}