using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using F = NclearOS2.FileManager;

namespace NclearOS2
{
    public static class Profiles
    {
        public static string LoadSystem()
        {
            try
            {
                if (Kernel.UseDisks && !Kernel.safeMode && VFSManager.DirectoryExists("0:\\NclearOS") && VFSManager.FileExists("0:\\NclearOS\\systemSettings.cfg"))
                {
                    string[] lines = File.ReadAllLines("0:\\NclearOS\\systemSettings.cfg");
                    foreach (string line in lines)
                    {
                        if (line.Contains("ScreenRes: "))
                        {
                            return line.Replace("ScreenRes: ", "");
                        }
                    }
                }
                return null;
            }
            catch(Exception e)
            {
                Msg.Main("Error", "Failed to read system settings; " + e, Resources.warn);
                return null;
            }
        }

        public static void LoadUser()
        {
            try
            {
                if (Kernel.UseDisks && !Kernel.safeMode && VFSManager.DirectoryExists("0:\\NclearOS") && VFSManager.FileExists("0:\\NclearOS\\userSettings.cfg"))
                {
                    string[] lines = File.ReadAllLines("0:\\NclearOS\\userSettings.cfg");
                    foreach (string line in lines)
                    {
                        if (line.Contains("WallpaperNum: "))
                        {
                            int a = Convert.ToInt32(line.Replace("WallpaperNum: ", ""));
                            if (a == -1) { Kernel.WallpaperOn = false; }
                            else
                            {
                                for (int i = 0; i < a; i++)
                                {
                                    Settings.Change("wallpaper");
                                }
                            }
                        }
                        else if (line.Contains("CursorType: "))
                        {
                            if (line == "CursorType: 1")
                            {
                                Settings.Change("cursorwhite");
                            }
                        }
                        else if (line.Contains("ColorTheme: "))
                        {
                            if (line.Contains("MidnightBlue")) { Settings.Change("darkblue"); }
                            else if (line.Contains("Goldenrod")) { Settings.Change("yellow"); }
                            else if (line.Contains("Green")) { Settings.Change("green"); }
                            else if (line.Contains("DarkRed")) { Settings.Change("red"); }
                            else if (line.Contains("Black")) { Settings.Change("black"); }
                            else if (line.Contains("R=40, G=40, B=40")) { Settings.Change("gray"); }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed to read user settings; " + e, Resources.warn);
            }
        }

        public static void Save()
        {
            try
            {
                if (Kernel.UseDisks && !Kernel.safeMode)
                {
                    F.Save("0:\\NclearOS\\systemSettings.cfg", "NclearOS 2 System Config\nScreenRes: " + Kernel.screenX + "x" + Kernel.screenY + "@" + Kernel.colorDepth + "\n              ");
                    F.Save("0:\\NclearOS\\userSettings.cfg", "NclearOS 2 User Config\nWallpaperNum: " + Convert.ToString(Settings.wallpapernum - 1) + "\nCursorType: " + Settings.cursortype + "\nColorTheme: " + Convert.ToString(Kernel.SystemPen) + "\n              ");
                }
            }
            catch { Toast.Force("Failed saving user settings"); }
        }
    }
}