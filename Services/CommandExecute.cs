using Cosmos.Core;
using System;
using Cosmos.HAL;

namespace NclearOS2
{
    public static class CommandExecute
    {
        public static string result;
        public static void Main(string command)
        {
            switch (command)
            {
                case "help":
                    result = ("NclearOS Help --------------- GENERAL SHORTCUTS\nF1 - display help\nESC - cancel input\nArrows Up/Down - browse command history");
                    result += ("\nNclearOS Help --------------- GENERAL  COMMANDS\nhelp - display help\nsd / shutdown - turn off computer\nrb / reboot - restart computer\nsettings - change various system settings\ninfo - information about OS and system components\nabout - display information about system\nver / vesrion - display system version\nerr - check Kernel error handling\nerrr - exploit Kernel error handling\necho - echo message\nsound - play beep\ndate / time - dipslay current date");
                    break;
                case "about":
                    result = ("About NclearOS\n--------\n" + Kernel.CurrentVersion + "\nBased on CosmosOS" + "\nCreated by Nclear\nGithub: https://github.com/Ncleardev/NclearOS \nWebsite: https://ncleardev.github.io");
                    break;
                case "shutdown" or "turnoff" or "sd":
                    Kernel.ShutdownPC(false);
                    break;
                case "reboot" or "restart" or "rb":
                    Kernel.ShutdownPC(true);
                    break;
                case "err":
                    if (Kernel.debug) { Kernel.ExecuteError = 1; } else { result = "Debug Mode is required to use this function\nTo turn on Debug Mode type 'debug'"; }
                    break;
                case "errr":
                    if (Kernel.debug) { Kernel.ExecuteError = 2; } else { result = "Debug Mode is required to use this function\nTo turn on Debug Mode type 'debug'"; }
                    break;
                case "echo":
                    result = ("Usage: echo 'message'");
                    break;
                case { } when command.StartsWith("echo "):
                    result = command[5..];
                    break;
                case { } when command.StartsWith("cd "):
                    Files.CD = command[3..];
                    result = "CD: " + Files.CD;
                    break;
                case "sound":
                    System.Console.Beep();
                    break;
                case "ver" or "version":
                    result = (Kernel.CurrentVersion);
                    break;
                case "time" or "date":
                    try { result = Date.CurrentTime(true) + " | " + Date.CurrentDate(true, true); }
                    catch (Exception e) { result = ("Service 'Date' crashed\n" + e); }
                    break;
                case "settings":
                    result = "Usage: set 'option'\n\nAvailable options:\nperf - turn off wallpaper for improved performance\ndefault, red, green, yellow, darkblue, gray, black - choose system color";
                    break;
                case { } when command.StartsWith("set "):
                    Settings.Change(command[4..]);
                    break;
                case "debug":
                    Kernel.debug = !Kernel.debug;
                    result = "Debug: " + Kernel.debug;
                    break;
                case "info":
                    try { result = "System Info\n-----------\n" + Sysinfo.Main(); }
                    catch (Exception e) { result = ("Service 'System Info' crashed\n" + e); }
                    break;
                case { } when command.StartsWith("ping "):
                    break;
                case "" or null:
                    break;
                default:
                    result = ("Unknown command '" + command + "', type help for list of commands.");
                    break;
            }
        }
    }
}