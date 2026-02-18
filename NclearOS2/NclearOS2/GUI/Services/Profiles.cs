using Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Threading;
using F = NclearOS2.FileManager;

namespace NclearOS2.GUI
{
    public static class Profiles
    {
        public static Mode Load()
        {
            try
            {
                if (System.IO.File.Exists(Kernel.USERCONFIG))
                {
                    string[] lines = System.IO.File.ReadAllLines(Kernel.USERCONFIG);
                    foreach (string line in lines)
                    {
                        if (line.Contains("WallpaperNum: "))
                        {
                            Settings.wallpapernum = Convert.ToInt32(line.Replace("WallpaperNum: ", "").Trim());
                        }
                        else if (line.Contains("CursorType: "))
                        {
                            if (line == "CursorType: 1") { Settings.cursorWhite = true; }
                        }
                        else if (line.Contains("ColorTheme: "))
                        {
                            if (line.Contains("MidnightBlue")) { GUI.SystemPen = GUI.DarkBluePen; }
                            else if (line.Contains("Goldenrod")) { GUI.SystemPen = GUI.YellowPen; }
                            else if (line.Contains("Green")) { GUI.SystemPen = GUI.GreenPen; }
                            else if (line.Contains("DarkRed")) { GUI.SystemPen = GUI.RedPen; }
                            else if (line.Contains("Black")) { GUI.SystemPen = GUI.DarkPen; }
                            else if (line.Contains("R=40, G=40, B=40")) { GUI.SystemPen = GUI.DarkGrayPen; }
                        }
                    }
                }
                if (System.IO.File.Exists(Kernel.SYSTEMCONFIG))
                {
                    string[] lines = System.IO.File.ReadAllLines(Kernel.SYSTEMCONFIG);
                    foreach (string line in lines)
                    {
                        if (line.Contains("ScreenRes: "))
                        {
                            return GUI.ResParse(line.Replace("ScreenRes: ", ""));
                        }
                    }
                }
            }
            catch { }
            return GUI.DisplayMode;
        }
        public static void Save()
        {
            try
            {
                if (Kernel.useDisks && !Kernel.safeMode)
                {
                    if (!VFSManager.DirectoryExists(Kernel.PROGRAMSDATAPATH)) { F.NewFolder(Kernel.PROGRAMSDATAPATH, true); }
                    F.Save(Kernel.SYSTEMCONFIG, "NclearOS 2 System Config\nScreenRes: " + GUI.ScreenX + "x" + GUI.ScreenY + "\n              ", true);
                    if (!VFSManager.DirectoryExists(Kernel.USERPROGRAMSDATAPATH)) { F.NewFolder(Kernel.USERPROGRAMSDATAPATH, true); }
                    F.Save(Kernel.USERCONFIG, "NclearOS 2 User Config\nWallpaperNum: " + Convert.ToString(Settings.wallpapernum) + "\nCursorType: " + (Settings.cursorWhite ? 1 : 0) + "\nColorTheme: " + Convert.ToString(GUI.SystemPen) + "\n              ", true);
                }
            }
            catch (Exception e) { Toast.Force("Failed saving user settings; " + e); Thread.Sleep(1000); }
        }
    }
}