using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Threading;
using F = NclearOS2.FileManager;

namespace NclearOS2.GUI
{
    public static class Profiles
    {
        public static Mode LoadSystem()
        {
            try
            {
                if (Kernel.useDisks && !Kernel.safeMode && VFSManager.DirectoryExists(Kernel.SYSTEMPATH) && VFSManager.FileExists(Kernel.SYSTEMCONFIG))
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
            return GUI.displayMode;
        }

        public static int LoadUser()
        {
            int i = 0;
            try
            {
                if (Kernel.useDisks && !Kernel.safeMode && VFSManager.DirectoryExists(Kernel.SYSTEMPATH) && VFSManager.FileExists(Kernel.USERCONFIG))
                {
                    string[] lines = System.IO.File.ReadAllLines(Kernel.USERCONFIG);
                    foreach (string line in lines)
                    {
                        if (line.Contains("WallpaperNum: "))
                        {
                            i = Convert.ToInt32(line.Replace("WallpaperNum: ", ""));
                        }
                        else if (line.Contains("CursorType: "))
                        {
                            if (line == "CursorType: 1")
                            {
                                Icons.cursor = Settings.cursor2;
                                Icons.cursorload = Settings.cursor2L;
                            }
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
            }
            catch { }
            return i;
        }
        public static void Save()
        {
            try
            {
                if (Kernel.useDisks && !Kernel.safeMode)
                {
                    F.Save(Kernel.SYSTEMCONFIG, "NclearOS 2 System Config\nScreenRes: " + GUI.displayMode.Columns + "x" + GUI.displayMode.Rows + "\n              ");
                    F.Save(Kernel.USERCONFIG, "NclearOS 2 User Config\nWallpaperNum: " + Convert.ToString(Settings.wallpapernum) + "\nCursorType: " + (Settings.cursorWhite ? 1 : 0) + "\nColorTheme: " + Convert.ToString(GUI.SystemPen) + "\n              ");
                }
            }
            catch (Exception e) { Toast.Force("Failed saving user settings; " + e); System.Console.ReadKey(); }
        }
    }
}