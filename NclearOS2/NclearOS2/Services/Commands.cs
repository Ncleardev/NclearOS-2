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

        internal abstract int Execute(string[] args, CommandShell shell = null); // returns 0 - OK; returns 1 - command not found in CommandTree, returns other number - error executing command
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
            return tree.Execute(args);
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
        public string print
        {
            get { return result; }
            set { result = value; update.Invoke(); }
        }
        public int Execute(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                print = null;
                return 0;
            }
            string[] args = command.ToLower().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            CommandsTree tree = CommandManager.GetTree(args[0]);
            if (tree == null) { print = "Unknown command '" + args[0] + "', type help for list of commands."; return 1; }

            try { return tree.Execute(args, this); }
            catch (Exception e)
            {
                print = "[command '" + args[0] + "' crashed: " + e + "]";
            }
            return -1;
        }
        /*public void CrashShell()
        {
            throw new Exception("Manual crash");
        }*/
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
        internal override int Execute(string[] args, CommandShell shell)
        {
            switch (args[0])
            {
                case "command":
                case "alternativecommand":
                    shell.print = "Executing command..."; //print text to console
                    //do some stuff
                    shell.print = "Done!";
                    return 0; //if command executed successfully
                case "command2":
                case "alternativecommand2":
                    bool force = false;
                    shell.print = "Deleting 0:\\...";
                    foreach (string arg in args.Skip(1)) //if user also specified parameters then
                    {
                        if (arg == "/f") { force = true; }
                    }
                    if (force) { FileManager.Delete("0:\\", true); } else { shell.print = "You cannot delete disk!"; }
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
            new Command(new string[] { "echo"}, "Displays a message."),
            new Command(new string[] { "beep"}, "Plays the sound of a beep through the PC Speaker.", new string[] { "[frequency/duration(ms)] - play a custom beep"}),
            new Command(new string[] { "prompt"}, "Allows to change displayed command prompt.", new string[] { "[text] - specifies the text of the command prompt; if empty sets prompt back to system default"}),
            new Command(new string[] { "setres"}, "Allows to change display resolution.", new string[] { "[resolution] - WIDTHxHEIGHT or WIDTHxHEIGHT@COLORDEPTH or leave empty to see the list of available resolutions"}),
            new Command(new string[] { "exit"}, "Switch beetween GUI and Text mode.")
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell)
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
                                if (cmd2.commands.Length > 1)
                                {
                                    int i = 0;
                                    foreach (string cmdvariant in cmd2.commands)
                                    {
                                        result += cmdvariant + " ";
                                        i += cmdvariant.Length + 1;
                                    }
                                    result += new string(' ', 19 - i) + cmd2.help + "\n";
                                    i = 0;
                                }
                                else
                                {
                                    result += cmd2.commands[0] + new string(' ', 20 - cmd2.commands[0].Length) + cmd2.help + "\n";
                                }
                            }
                        }
                    }
                    else
                    {
                        var item = CommandManager.GetCommand(args[1]);
                        if (item == null) { shell.print = "Unknown command '" + args[1] + "', type help for list of commands."; return 0; }
                        result = item.help + "\n";
                        foreach (string str in item.possibleParams)
                        {
                            result += str + "\n";
                        }
                    }
                    shell.print = result;
                    return 0;
                case "shortcuts":
                    shell.print = Kernel.GUIenabled ?
                    "GUI Mode shortcuts:\nF11 - open Console App\nF12 - open Process Manager\nAlt + F4 - close window\nWinKey / Ctrl + Esc - open/close Menu\nCtrl + Alt + Delete - restart PC\nAlt + Tab - switch between windows\nConsole App:\nArrows Up/Down - browse command history\nESC - cancel input\nFiles App:\nF5 - refresh list\nDel - delete selected directory/file\nAlt + LeftArrow - go Back\nAlt + UpArrow - go to parent directory"
                    : "Text Mode shortcuts:\nF1 - display help\nCtrl + Alt + Delete - restart PC\nESC - cancel input\nArrows Up/Down - browse command history";
                    return 0;
                case "list":
                    string modules = "";
                    int allmodules = 0;
                    int allcommands = 0;
                    int allvariants = 0;
                    foreach (CommandsTree module in CommandManager.commandTrees)
                    {
                        modules += module.name + new string(' ', 19 - module.name.Length) + module.description + "\n";
                        foreach (Command cmd2 in module.commands)
                        {
                            foreach (string cmdvariant in cmd2.commands)
                            {
                                modules += cmdvariant + " | ";
                                allvariants++;
                            }
                            allcommands++;
                        }
                        modules += "\n\n";
                        allmodules++;
                    }
                    shell.print = modules + "Installed modules: " + allmodules + " Installed commands: " + allcommands + ", including all variants: " + allvariants;
                    return 0;
                case "echo":
                    foreach (string arg in args)
                    {
                        shell.print = arg;
                    }
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
                        shell.print = "Beeping - frequency " + numberone + " for " + numbertwo + " ms...";
                        System.Console.Beep(numberone, numbertwo);
                        shell.print = "Done";
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
                    shell.print = null;
                    shell.update.Invoke();
                    return 0;
                case "setres":
                    if (args.Length == 1)
                    {
                        string txt = "Available resolutions: ";
                        foreach (var i in GUI.GUI.canvas.AvailableModes)
                        {
                            txt += "\n" + i;
                        }
                        shell.print = txt;
                    }
                    else
                    {
                        if (Kernel.GUIenabled) { shell.print = GUI.GUI.SetRes(GUI.GUI.ResParse(args[1])); }
                        else { shell.print = GUI.GUI.Init(args[1]); }
                    }
                    return 0;
                case "exit":
                    if (Kernel.GUIenabled)
                    {
                        GUI.GUI.ShutdownGUI();
                        GUI.GUI.canvas.Disable();
                    }
                    else
                    {
                        shell.print = GUI.GUI.Init();
                    }
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
        internal override int Execute(string[] args, CommandShell shell)
        {
            switch (args[0])
            {
                case "sd":
                case "shutdown":
                    shell.print = "Shutting down...";
                    if (args.Length > 1)
                    {
                        if (args[1] == "/f")
                        {
                            Kernel.Shutdown(false, true);
                            return 0;
                        }
                        else if (args[1] == "/ff")
                        {
                            Cosmos.System.Power.Shutdown();
                            return 0;
                        }
                        return 2;
                    }
                    Kernel.Shutdown(false);
                    return 0;
                case "rb":
                case "reboot":
                case "restart":
                    shell.print = "Restarting...";
                    if(args.Length > 1)
                    {
                        if (args[1] == "/f")
                        {
                            Kernel.Shutdown(true, true);
                            return 0;
                        }
                        else if (args[1] == "/ff")
                        {
                            Cosmos.System.Power.Reboot();
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
            new Command(new string[] { "debug"}, "Switches beetween debug states."),
            new Command(new string[] { "err"}, "Crash specific level of system.", new string[] { "/c - Crash Command execution", "/s - Crash Command Shell client", "/k - Check Kernel error handling", "/xk - Exploit Kernel error handling"})
            })
        {
        }
        internal override int Execute(string[] args, CommandShell shell)
        {
            switch (args[0])
            {
                case "debug":
                    if (Kernel.GUIenabled)
                    {
                        GUI.GUI.debug = !GUI.GUI.debug;
                        shell.print = "Debug: " + GUI.GUI.debug;
                    }
                    else
                    {
                        shell.print = "Debug Mode is supported in GUI mode only.";
                    }
                    return 0;
                case "err":
                    if (GUI.GUI.debug)
                    {
                        if (args.Length == 1) { shell.print = "Error: missing parameter, type 'help err' for list of possible parameters."; return 0; }
                        switch (args[1])
                        {
                            case "/c":
                                throw new Exception("Manual crash");
                            /*case "/s":
                                shell.CrashShell();
                                return 0;*/
                            case "/s":
                                shell.crashClient.Invoke();
                                shell.print = "If you are seeing this, Console Shell client does not include debugging functions.";
                                return 0;
                            case "/k":
                                GUI.GUI.ExecuteError = 1;
                                return 0;
                            case "/xk":
                                GUI.GUI.ExecuteError = 2;
                                return 0;
                            default:
                                shell.print = "Error: wrong parameter, type 'help err' for list of possible parameters.";
                                return 0;
                        }
                    }
                    else
                    {
                        shell.print = "Debug Mode is required to use this function\nTo turn on Debug Mode type 'debug'";
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
        internal override int Execute(string[] args, CommandShell shell)
        {
            if (args[0] == "about" || args[0] == "info")
            {
                shell.print = "_____   __     ______                  _______________\n" +
                    "___  | / /________  /__________ _________  __ \\_  ___/\n" +
                    "__   |/ /_  ___/_  /_  _ \\  __ `/_  ___/  / / /____ \\\n" +
                    "_  /|  / / /__ _  / /  __/ /_/ /_  /   / /_/ /____/ /\n" +
                    "/_/ |_/  \\___/ /_/  \\___/\\__,_/ /_/    \\____/ /____/   " + Kernel.OSVERSION +
                    "\n\nBased on CosmosOS" + "\nCreated by Nclear\nGithub: https://github.com/Ncleardev/NclearOS-2 \nWebsite: https://ncleardev.github.io/nclearos";
                return 0;
            }
            else if (args[0] == "ver" || args[0] == "version")
            {
                shell.print = Kernel.OSVERSION;
                return 0;
            }
            return 1;
        }
    }
}