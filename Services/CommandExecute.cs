using Cosmos.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.HAL;
using F = NclearOS2.FileManager;
using System.Collections.Generic;

namespace NclearOS2
{
    public class CommandExecute
    {
        public string result;
        public void Main(string command)
        {
            command = command.ToLower();
            switch (command)
            {
                case { } when string.IsNullOrWhiteSpace(command):
                    break;
                case "help":
                    result = "NclearOS Help --------------- COMMANDS\nhelp - display help\nshortcuts - display available shortcuts\nsd / shutdown / turnoff - turn off computer\nrb / reboot / restart - restart computer\nset - change various system settings\ninfo - information about OS and system components\nabout - display information about system\nver / vesrion - display system version\nerr - check Kernel error handling\nerrr - exploit Kernel error handling\nformat (disk)\nnet - display IP Address\necho - echo message\nbeep - play beep\ndate / time - dipslay current date";
                    break;
                case "shortcuts":
                    result = "NclearOS Help --------------- SHORTCUTS\nF1 - display help\nF5 - refresh list in Files\nF11 - open Console App\nF12 - open Task Manager\nAlt + F4 - close window\nWinKey - open/close Menu\nCtrl + Alt + Delete - restart PC\nESC - cancel input\nArrows Up/Down - browse command history";
                    break;
                case "about":
                    result = ("About NclearOS 2\n--------\n" + Kernel.CurrentVersion + "\nBased on CosmosOS" + "\nCreated by Nclear\nGithub: https://github.com/Ncleardev/NclearOS-2 \nWebsite: https://ncleardev.github.io/nclearos");
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
                case "net":
                    result = Net.GetInfo();
                    break;
                case { } when command.StartsWith("ping "):
                    result = Net.Ping(command[5..]);
                    break;
                case { } when command.StartsWith("format "):
                    result = "Now formatting " + command[7..] + ":";
                    Kernel.Refresh();
                    F.Format(Convert.ToInt32(command[7..]));
                    result += "\nDone";
                    break;
                case "exit":
                    Kernel.canvas.Disable();
                    System.Console.ReadLine();
                    Kernel.SetRes(Kernel.screenX + "x" + Kernel.screenY);
                    break;
                case "beep":
                    result = "Playing default beep...";
                    Kernel.Refresh();
                    Console.Beep();
                    result = "Using a custom beep: beep 'frequency/duration(ms)'";
                    break;
                case { } when command.StartsWith("beep "):
                    string[] splitit = command.Replace("beep ", "").Split('/');
                    int numberone = Convert.ToInt32(splitit[0]);
                    int numbertwo = Convert.ToInt32(splitit[1]);
                    result = "Beeping - frequency " + numberone + " for " + numbertwo + " ms...";
                    Kernel.Refresh();
                    Console.Beep(numberone, numbertwo);
                    result = "Done";
                    break;
                case "ver" or "version":
                    result = (Kernel.CurrentVersion);
                    break;
                case "time" or "date":
                    try { result = Date.CurrentTime(true) + " | " + Date.CurrentDate(true, true); }
                    catch (Exception e) { result = ("Service 'Date' crashed\n" + e); }
                    break;
                case "fs.init":
                    F.Start(true);
                    return;
                case "set":
                    result = "Usage: set 'option'\n\nOptions:\nres - change screen resolution\nwallpaper\ncursorwhite\ncursordark\n\nTheme Colors:\ndefault\nred\ngreen\nyellow\ndarkblue\ngray\nblack";
                    break;
                case { } when command.StartsWith("set "):
                    result = Settings.Change(command[4..]);
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
                default:
                    result = ("Unknown command '" + command + "', type help for list of commands.");
                    break;
            }
        }
    }
}