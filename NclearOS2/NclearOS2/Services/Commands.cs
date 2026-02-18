using Cosmos.System;
using Cosmos.System.Graphics;
using NclearOS2.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NclearOS2.Commands
{
    internal abstract class CommandsTree
    {
        internal CommandsTree(string name, string description, Command[] commands)
        {
            this.name = name;
            this.description = description;
            this.commands = commands;
        }
        internal string name;
        internal string description;
        internal Command[] commands;

        internal abstract int Execute(string[] args, string rawInput, CommandShell shell = null); // returns 0 - OK; returns 1 - command not found in CommandTree, returns 2 - wrong parameters, returns other number - error executing command
    }
    internal class Command
    {
        internal Command(string[] commands, string help = "", string[] possibleParams = null)
        {
            this.commands = commands;
            this.help = help;
            this.possibleParams = possibleParams;
        }
        internal string[] commands;
        internal string help;
        internal string[] possibleParams;
    }
    internal static class CommandManager
    {
        internal static List<CommandsTree> commandTrees = new();
        public static void Register()
        {
            commandTrees.Add(new Basic());
            commandTrees.Add(new Power());
            commandTrees.Add(new Debug());
            commandTrees.Add(new SystemInfo());
            commandTrees.Add(new Files());
            commandTrees.Add(new Date());
            commandTrees.Add(new About());
            commandTrees.Add(new Network());
        }
        public static int RawExecute(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) { return 0; }
            string[] args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            CommandsTree tree = GetTree(args[0]);
            if (tree == null) { return 1; }
            return tree.Execute(args, command, null);
        }
        internal static CommandsTree GetTree(string text)
        {
            foreach (CommandsTree tree in commandTrees)
            {
                foreach (Command cmd in tree.commands)
                {
                    foreach (string cmdvariant in cmd.commands)
                    {
                        if (cmdvariant == text)
                        {
                            return tree;
                        }
                    }
                }
            }
            return null;
        }
        internal static Command GetCommand(string text)
        {
            foreach (CommandsTree tree in commandTrees)
            {
                foreach (Command cmd in tree.commands)
                {
                    foreach (string cmdvariant in cmd.commands)
                    {
                        if (cmdvariant == text)
                        {
                            return cmd;
                        }
                    }
                }
            }
            return null;
        }
    }
    public class CommandShell
    {
        public static readonly string defaultPrompt = ">";
        public string prompt = defaultPrompt;
        private string result;
        internal Action update;
        internal Action crashClient;
        internal Action clearScreen;
        internal Action exit;
        public string Print
        {
            get { return result; }
            set { result = value; update.Invoke(); }
        }
        public int Execute(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                Print = null;
                return 0;
            }
            string[] args = command.ToLower().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            CommandsTree tree = CommandManager.GetTree(args[0]);
            if (tree == null) { Print = "Unknown command '" + args[0] + "', type help for list of commands."; return 1; }

            try { return tree.Execute(args, command, this); }
            catch (Exception e)
            {
                if (e.Message == "system/Crash_Console_Shell") { throw new Exception("Manual crash"); }
                Print = "[command '" + args[0] + "' crashed: " + e + "]";
            }
            
            return -1;
        }
    }
    internal class Example : CommandsTree
    {
        internal Example() : base
            ("Name of Command Tree", "Description",
            new Command[] {
            new Command(new string[] { "command", "alternativecommand" }, "Description of a command", new string[] {"available parameters"}),
            new Command(new string[] { "command2", "alternativecommand2" }, "Description of a command", new string[] {"/f - force"})
            })
        {
        }
        internal override int Execute(string[] args, string rawInput, CommandShell shell)
        {
            switch (args[0])
            {
                case "command":
                case "alternativecommand":
                    shell.Print = "Executing command..."; //Print text to console
                    //do some stuff
                    shell.Print = "Done!";
                    return 0; //if command executed successfully
                case "command2":
                case "alternativecommand2":
                    bool force = false;
                    shell.Print = "Deleting 0:\\...";
                    foreach (string arg in args.Skip(1)) //if user also specified parameters then
                    {
                        if (arg == "/f") { force = true; }
                    }
                    if (force) { FileManager.Delete("0:\\", true); } else { shell.Print = "You cannot delete disk!"; }
                    return 0; //if command executed successfully
            }
            return 1; //command not found in this CommandTree!
        }
    }

    internal class Basic : CommandsTree
    {
        internal Basic() : base
            ("Basic", "",
            new Command[] {
            new Command(new string[] { "help", "?" }, "Provides help information for commands.", new string[] {"help [command] - displays help information for that command."}),
            new Command(new string[] { "shortcuts" }, "Provides list of shortucts"),
            new Command(new string[] { "list" }, "Provides list of installed command modules"),
            new Command(new string[] { "cls", "clear"}, "Clears the output."),
            new Command(new string[] { "echo"}, "Displays a message."),
            new Command(new string[] { "beep"}, "Plays the sound of a beep through the PC Speaker.", new string[] { "[frequency/duration(ms)] - play a custom beep"}),
            new Command(new string[] { "prompt"}, "Allows to change displayed command prompt.", new string[] { "[text] - specifies the text of the command prompt; if empty sets prompt back to system default"}),
            new Command(new string[] { "setres"}, "Allows to change display resolution.", new string[] { "[resolution] - WIDTHxHEIGHT or WIDTHxHEIGHT@COLORDEPTH or leave empty to see the list of available resolutions"}),//, "/c [canvas type] - SVGAII / VBE / VGA"}),
            //new Command(new string[] { "fps"}, "Allows to change FPS lock.", new string[] { "[fps] - fps value or leave empty to uncap"}),
            new Command(new string[] { "switch"}, "Switch beetween GUI and Text mode."),
            new Command(new string[] { "exit"}, "Closes the Console.")
            })
        {
        }
        internal override int Execute(string[] args, string rawInput, CommandShell shell)
        {
            switch (args[0])
            {
                case "?":
                case "help":
                    string result;
                    if (args.Length == 1)
                    {
                        result = "For more information on a specific command, type help [command]\n";
                        foreach (CommandsTree tree in CommandManager.commandTrees)
                        {
                            foreach (Command cmd2 in tree.commands)
                            {
                                string str = String.Join(' ', cmd2.commands);
                                result += str + new string(' ', 22 - str.Length) + cmd2.help + "\n";
                            }
                        }
                    }
                    else
                    {
                        var item = CommandManager.GetCommand(args[1]);
                        if (item == null) { shell.Print = "Unknown command '" + args[1] + "', type help for list of commands."; return 0; }
                        result = item.help + "\n";
                        foreach (string str in item.possibleParams)
                        {
                            result += str + "\n";
                        }
                    }
                    shell.Print = result;
                    return 0;
                case "shortcuts":
                    shell.Print = Kernel.GUIenabled ?
                    "GUI Mode shortcuts:\nF10 - open Console App\nF11 - toggle fullscreen\nF12 - open Process Manager\nAlt + F4 - close window\nWinKey / Ctrl + Esc - open/close Menu\nCtrl + Alt + Delete - restart PC\nAlt + Tab - switch between windows\nConsole App:\nArrows Up/Down - browse command history\nESC - cancel input\nFiles App:\nF5 - refresh list\nDel - delete selected directory/file\nAlt + LeftArrow - go Back\nAlt + UpArrow - go to parent directory"
                    : "Text Mode shortcuts:\nF1 - display help\nCtrl + Alt + Delete - restart PC\nESC - cancel input\nArrows Up/Down - browse command history";
                    return 0;
                case "list":
                    string modules = "";
                    int allmodules = 0;
                    int allcommands = 0;
                    int allvariants = 0;
                    foreach (CommandsTree module in CommandManager.commandTrees)
                    {
                        modules += module.name + new string(' ', 19 - module.name.Length) + module.description + "\n| ";
                        foreach (Command cmd2 in module.commands)
                        {
                            foreach (string cmdvariant in cmd2.commands)
                            {
                                modules += cmdvariant + " | ";
                                allvariants++;
                            }
                            allcommands++;
                        }
                        modules += "\n";
                        allmodules++;
                    }
                    shell.Print = modules + "Installed modules: " + allmodules + " Installed commands: " + allcommands + ", including all variants: " + allvariants;
                    return 0;
                case "cls":
                case "clear":
                    shell.Print = "";
                    shell.clearScreen?.Invoke();
                    return 0;
                case "echo":
                    shell.Print = rawInput.Remove(0, "echo ".Length);
                    return 0;
                case "beep":
                    if (args.Length == 1)
                    {
                        System.Console.Beep();
                    }
                    else
                    {
                        string[] splitit = args[1].Split('/');
                        int numberone = Convert.ToInt32(splitit[0]);
                        int numbertwo = Convert.ToInt32(splitit[1]);
                        shell.Print = "Beeping - frequency " + numberone + " for " + numbertwo + " ms...";
                        System.Console.Beep(numberone, numbertwo);
                        shell.Print = "Done";
                    }
                    return 0;
                case "prompt":
                    if (args.Length == 1)
                    {
                        shell.prompt = CommandShell.defaultPrompt;
                    }
                    else
                    {
                        shell.prompt = args[1];
                    }
                    shell.Print = null;
                    shell.update.Invoke();
                    return 0;
                case "setres":
                    if (args.Length == 1)
                    {
                        if (GUI.GUI.canvas == null)
                        {
                            shell.Print = "No canvas detected!"; return 0;// Use /c parameter to create canvas"; return 0;
                        }
                        string txt = "Available resolutions: ";
                        foreach (var i in GUI.GUI.canvas.AvailableModes)
                        {
                            txt += "\n" + i;
                        }
                        shell.Print = txt;
                    }
                    else
                    {
                        /*string res = null;
                        string canvas = null;
                        for (int i = 0; i < args.Length; i++)
                        {
                            if (args[i] == "/r") { res = args[i + 1]; }
                            if (args[i] == "/c") { try { canvas = args[i + 1]; } catch { canvas = ""; }  }//canvas = args?[i + 1] ?? "";
                        }
						
						if (res != null)
                        {
                        }
                        else if (canvas != null)
                        {
                            try { GUI.GUI.SetCanvas(canvas); }
                            catch (Exception e) { throw new Exception(canvas + " canvas is not available; " + e); }
                        }*/
						
						string res = args[1];
						
						Mode mode = GUI.GUI.ResParse(res);
                        try {
                            if (Kernel.GUIenabled) { shell.Print = GUI.GUI.SetRes(mode, false, false); }
                            else { shell.Print = GUI.GUI.Init(res); }
                        }
                        catch (Exception e) {
                            //if (!string.IsNullOrEmpty(canvas)) { canvas += " Canvas: "; }
                            throw new Exception("Resolution " + mode.Columns + "x" + mode.Rows + " is not available; " + e);
                        }
                    }
                    return 0;
                /*case "fps":
                    if (args.Length == 1) { GUI.GUI.targetFPS = -1; shell.Print = "FPS uncapped."; }
                    else { GUI.GUI.targetFPS = int.Parse(args[1]); shell.Print = "FPS capped to " + args[1] + "."; }
                    return 0;*/
                case "switch":
                    if (Kernel.GUIenabled)
                    {
                        shell.Print = "Shutting down GUI...";
                        Kernel.GUIenabled = false;
                    }
                    else
                    {
                        shell.Print = GUI.GUI.Init();
                    }
                    return 0;
                case "exit":
                    shell.exit?.Invoke();
                    shell.Print = "If you are seeing this, Console Shell client does not include exit function.";
                    return 0;
            }
            return 1;
        }
    }
    internal class Power : CommandsTree
    {
        internal Power() : base
            ("Power Managment", "Manages the computer power state.",
            new Command[] {
            new Command(new string[] { "sd", "shutdown" }, "Shuts down computer.", new string[] {"/f - Forces running applications to close without prompts.", "/ff - Forces the system to shut down immediately." }),
            new Command(new string[] { "rb", "reboot", "restart" }, "Restarts computer.", new string[] {"/f - Forces running applications to close without prompts.", "/ff - Forces the system to shut down immediately." })
            })
        {
        }
        internal override int Execute(string[] args, string rawInput, CommandShell shell)
        {
            switch (args[0])
            {
                case "sd":
                case "shutdown":
                    shell.Print = "Shutting down...";
                    if (args.Length > 1)
                    {
                        if (args[1] == "/f")
                        {
                            Kernel.Shutdown(false, 1);
                            return 0;
                        }
                        else if (args[1] == "/ff")
                        {
                            Kernel.Shutdown(false, 2);
                            return 0;
                        }
                        return 2;
                    }
                    Kernel.Shutdown(false);
                    return 0;
                case "rb":
                case "reboot":
                case "restart":
                    shell.Print = "Restarting...";
                    if(args.Length > 1)
                    {
                        if (args[1] == "/f")
                        {
                            Kernel.Shutdown(true, 1);
                            return 0;
                        }
                        else if (args[1] == "/ff")
                        {
                            Kernel.Shutdown(true, 2);
                            return 0;
                        }
                        return 2;
                    }
                    Kernel.Shutdown(true);
                    return 0;
            }
            return 1;
        }
    }
    internal class Debug : CommandsTree
    {
        internal Debug() : base
            ("Debug", "Provides debugging options",
            new Command[] {
            new Command(new string[] { "debug"}, "Switches between debug states.", new string[] { "true", "false" }),
            new Command(new string[] { "err"}, "Crash specific level of system.", new string[] { "/c - Crash Command execution", "/s - Crash Command Shell", "/sc - Crash Command Shell client", "/k - Check Kernel error handling", "/xk - Exploit Kernel error handling"})
            })
        {
        }
        internal override int Execute(string[] args, string rawInput, CommandShell shell)
        {
            switch (args[0])
            {
                case "debug":
                    if(args.Length == 1) { Kernel.Debug = !Kernel.Debug; }
                    else { Kernel.Debug = args[1] == "true"; }
                    shell.Print = "Debug: " + Kernel.Debug;
                    return 0;
                case "err":
                    if (Kernel.Debug)
                    {
                        if (args.Length == 1) { shell.Print = "Error: missing parameter, type 'help err' for list of possible parameters."; return 0; }
                        switch (args[1])
                        {
                            case "/c":
                                throw new Exception("Manual crash");
                            case "/s":
                                if(shell == null) { return 0; }
                                throw new Exception("system/Crash_Console_Shell");
                            case "/sc":
                                shell.crashClient?.Invoke();
                                shell.Print = "If you are seeing this, Console Shell client does not include debugging functions.";
                                return 0;
                            case "/k":
                                Kernel.ExecuteError = 1;
                                return 0;
                            case "/xk":
                                Kernel.ExecuteError = 2;
                                return 0;
                            default:
                                shell.Print = "Error: wrong parameter, type 'help err' for list of possible parameters.";
                                return 0;
                        }
                    }
                    else
                    {
                        shell.Print = "Debug Mode is required to use this function\nTo turn on Debug Mode type 'debug'";
                        return 0;
                    }
            }
            return 1;
        }
    }
    internal class About : CommandsTree
    {
        internal About() : base
            ("About", "Provides info about NclearOS",
            new Command[] {
            new Command(new string[] { "about", "info"}, "Display info about NclearOS"),
            new Command(new string[] { "ver", "version"}, "Display NclearOS version")
            })
        {
        }
        internal override int Execute(string[] args, string rawInput, CommandShell shell)
        {
            if (args[0] == "about" || args[0] == "info")
            {
                shell.Print = "_____   __     ______                  _______________\n" +
                    "___  | / /________  /__________ _________  __ \\_  ___/\n" +
                    "__   |/ /_  ___/_  /_  _ \\  __ `/_  ___/  / / /____ \\\n" +
                    "_  /|  / / /__ _  / /  __/ /_/ /_  /   / /_/ /____/ /\n" +
                    "/_/ |_/  \\___/ /_/  \\___/\\__,_/ /_/    \\____/ /____/\n" +
                    Kernel.OSVERSION + "\n\nBased on CosmosOS" + "\nCreated by Ncleardev\nGithub: https://github.com/Ncleardev/NclearOS-2 \nWebsite: https://ncleardev.github.io/nclearos";
                return 0;
            }
            else if (args[0] == "ver" || args[0] == "version")
            {
                shell.Print = Kernel.OSVERSION;
                return 0;
            }
            return 1;
        }
    }
}