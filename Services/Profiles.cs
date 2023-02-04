using Cosmos.System.FileSystem.VFS;
using System;
using System.IO;
using System.Text;

namespace NclearOS2
{
    public static class Profiles
    {
        public static string Load()
        {
            try
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write("| .. |");
                System.Console.ResetColor();
                System.Console.Write(" Profile Settings");
                System.Console.CursorLeft = 0;
                if (Kernel.UseDisks && Kernel.UseProfile && VFSManager.DirectoryExists("0:\\NclearOS") && VFSManager.FileExists("0:\\NclearOS\\settings.cfg"))
                {
                    string[] lines = File.ReadAllLines("0:\\NclearOS\\settings.cfg");
                    foreach (string line in lines)
                    {
                        System.Console.WriteLine(line);
                        if (line.Contains("WallpaperNum: "))
                        {
                            int a = Convert.ToInt32(line.Replace("WallpaperNum: ", ""));
                            if(a == -1) { Kernel.WallpaperOn = false; }
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
                            else if(line.Contains("Goldenrod")) { Settings.Change("yellow"); }
                            else if(line.Contains("Green")) { Settings.Change("green"); }
                            else if(line.Contains("DarkRed")) { Settings.Change("red"); }
                            else if(line.Contains("Black")) { Settings.Change("black"); }
                            else if(line.Contains("R=40, G=40, B=40")) { Settings.Change("gray"); }
                        }
                        else if (line.Contains("ScreenRes: "))
                        {
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            System.Console.Write("| OK |");
                            System.Console.ResetColor();
                            System.Console.Write(" Profile Settings \n");
                            return line.Replace("ScreenRes: ", "");
                        }
                    }
                    System.Console.ForegroundColor = ConsoleColor.Green;
                    System.Console.Write("| OK |");
                    System.Console.ResetColor();
                    System.Console.Write(" Profile Settings \n");
                    return null;
                }
                System.Console.WriteLine("0:\\NclearOS\\settings.cfg not found");
                return null;
            }
            catch(Exception e)
            {
                Kernel.UseProfile = false;
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("| ERR |");
                System.Console.ResetColor();
                System.Console.Write(" Profile Settings \n" + e + "\nPress any key to continue");
                Console.ReadKey();
                return null;
            }
        }
        public static void Save()
        {
            try
            {
                if (Kernel.UseDisks)
                {
                    string content = "NclearOS 2 Profile Config\nWallpaperNum: " + Convert.ToString(Settings.wallpapernum - 1) + "\nCursorType: " + Settings.cursortype + "\nColorTheme: " + Convert.ToString(Kernel.SystemPen) + "\nScreenRes: " + Kernel.screenX + "x" + Kernel.screenY + "@" + Kernel.colorDepth + "\n" + Date.CurrentDate(false, true) + " " + Date.CurrentTime(true);
                    Files.Save("0:\\NclearOS\\settings.cfg", content);
                }
            }
            catch { Msg.Main("Failed saving user settings", true);}
        }
    }
}