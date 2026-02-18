using Cosmos.HAL.BlockDevice;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Network.IPv4.TCP;
using NclearOS2.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Cosmos.HAL.BlockDevice.ATA_PIO;
using static System.Net.Mime.MediaTypeNames;

namespace NclearOS2
{
    public static class FileManager
    {
        public static CosmosVFS fs = new();
        private static void Info(string msg, bool silent) {
            if (!silent) {
                if (Kernel.GUIenabled)
                {
                    GUI.GUI.Loading = true;
                    Toast.Msg = msg;
                    GUI.GUI.Refresh();
                    GUI.GUI.Loading = false;
                }
                else
                {
                    System.Console.WriteLine(msg);
                }
            }
        }
        private static void Error(string msg) { Msg.Main("Error", msg, Icons.error); }
        public static string Start()
        {
            try
            {
                VFSManager.RegisterVFS(fs);
                Kernel.useDisks = true;
                return "Success";
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("already initialized"))
                {
                    Kernel.useDisks = false;
                    Msg.Main("File System Error", e.Message, Icons.error);     
                }
                return e.Message;
            }
        }
        public static string Open(string path) { return File.ReadAllText(path); }
        public static byte[] OpenInBytes(string path) { return File.ReadAllBytes(path); }
        public static string Save(string path, string toSave, bool silent = false)
        {
            Info("Saving " + path + "...", silent);
            try
            {
                if (!VFSManager.FileExists(path))
                {
                    VFSManager.CreateFile(path);
                }

                var helloFile = VFSManager.GetFile(path);
                var helloFileStream = helloFile.GetFileStream();

                if (helloFileStream.CanWrite)
                {
                    byte[] textToWrite = Encoding.ASCII.GetBytes(toSave);
                    helloFileStream.Write(textToWrite, 0, textToWrite.Length);
                    Info("Saved " + path, silent);
                }
                else
                {
                    Error("Cannot write to " + path);
                }
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed saving " + path + "; " + e, Icons.error);
            }
            GUI.GUI.Loading = false;
            return path;
        }
        public static void SaveInBytes(string path, byte[] toSave, bool silent = false)
        {
            if (!silent && Kernel.GUIenabled) { GUI.GUI.Wait(); }
            try
            {
                if (!VFSManager.FileExists(path))
                {
                    VFSManager.CreateFile(path);
                }

                var helloFile = VFSManager.GetFile(path);
                var helloFileStream = helloFile.GetFileStream();

                if (helloFileStream.CanWrite)
                {
                    helloFileStream.Write(toSave, 0, toSave.Length);
                }
                else
                {
                    Msg.Main("Error", "Cannot write to " + path, Icons.error);
                }
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed saving " + path + "; " + e, Icons.error);
            }
        }
        public static bool Format(int diskIndex, string format = "FAT32", bool quick = true)
        {
            Toast.Msg = "Formatting " + diskIndex + ":\\ - file system " + format + "...";
            if (Kernel.GUIenabled) GUI.GUI.Wait();
            try
            {
                fs.Disks[diskIndex].FormatPartition(diskIndex, format, quick);
                NotificationSystem.Notify("File Manager", "Successfully formatted " + diskIndex + ":\\ - file system " + format + " - Restart PC to see changes", Icons.info);
                Kernel.useDisks = false;
                return true;
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed formatting " + diskIndex + ":\\ - file system " + format + "; " + e, Icons.error);
                return false;
            }
        }
        public static string NewFolder(string path, bool silent = false)
        {
            if (!silent && Kernel.GUIenabled)
            {
                GUI.GUI.Loading = true;
                GUI.GUI.Refresh();
            }
            if (VFSManager.DirectoryExists(path))
            {
                int i = 1;
                while (VFSManager.DirectoryExists(path + " (" + i + ")"))
                {
                    i++;
                    GUI.GUI.Refresh();
                }
                VFSManager.CreateDirectory(path + " (" + i + ")");
                GUI.GUI.Loading = false;
                return (path + " (" + i + ")");
            }
            else
            {
                VFSManager.CreateDirectory(path);
                GUI.GUI.Loading = false;
                return (path);
            }
        }
        public static string NewFile(string path, string Type = ".txt", bool silent = false)
        {
            if (!silent && Kernel.GUIenabled)
            {
                GUI.GUI.Loading = true;
                GUI.GUI.Refresh();
            }
            if (String.IsNullOrWhiteSpace(Type) && path.Contains('.'))
            {
                string[] splitit = path.Split('.');
                path = splitit[0];
                Type = '.' + splitit[1];
            }
            if (VFSManager.FileExists(path + Type))
            {
                int i = 1;
                while (VFSManager.FileExists(path + " (" + i + ")" + Type))
                {
                    i++;
                    if (!silent && Kernel.GUIenabled) GUI.GUI.Refresh();
                }
                VFSManager.CreateFile(path + " (" + i + ")" + Type);
                GUI.GUI.Loading = false;
                return (path + " (" + i + ")" + Type);
            }
            else
            {
                VFSManager.CreateFile(path + Type);
                GUI.GUI.Loading = false;
                return (path + Type);
            }
        }
        public static void Delete(string path, bool folder = false, bool silent = false)
        {
            if (!silent && Kernel.GUIenabled)
            {
                GUI.GUI.Loading = true;
                GUI.GUI.Refresh();
            }
            try
            {
                if (folder) { Directory.Delete(path, true); }
                else { File.Delete(path); }
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed deleting " + path + "; " + e, Icons.error);
            }
            GUI.GUI.Loading = false;
        }
        public static void Rename(string path, string newName, bool silent = false)
        {
            if (IsFolder(path))
            {
                string dir = Directory.GetParent(path).ToString();
                string oldName = path.Replace(dir, null);
                if (oldName == newName) { return; }
                CopyDirectory(path, dir + "\\" + newName, true);
                Delete(path, true, silent);
            }
            else
            {
                string dir = Directory.GetParent(path).ToString();
                string oldName = path.Replace(dir, null);
                if (oldName == newName) { return; }
                SaveInBytes(NewFile(dir + "\\" + newName, "", silent), OpenInBytes(path), silent);
                Delete(path, false, silent);
            }
        }
        public static void CopyFile(string from, string target = "")
        {
            if (IsFolder(from)) throw new FileNotFoundException("Source file does not exist: " + from);
            if (Kernel.GUIenabled)
            {
                GUI.GUI.Loading = true;
                GUI.GUI.Refresh();
            }
            if (string.IsNullOrWhiteSpace(target)){ target = NewFile(from, null); }
            File.Copy(from, target, true);

            GUI.GUI.Loading = false;
        }
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            if (!IsFolder(sourceDir)) throw new DirectoryNotFoundException("Source directory does not exist: " + sourceDir);
            if (Kernel.GUIenabled)
            {
                GUI.GUI.Loading = true;
                GUI.GUI.Refresh();
            }
            var dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
            GUI.GUI.Loading = false;
        }
        public static bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }
    }
}
namespace NclearOS2.Commands
{
    internal class Files : CommandsTree
    {
        public static int selDisk = 0;
        public static string path = "Computer";
        public static string undoPath = "Computer";
        public static string ParentPath { get { return Convert.ToString(Directory.GetParent(path)); } }
        internal Files() : base
            ("Files", "Manages files on computer.",
            new Command[] {
            new(new string[] { "init" }, "Initializes file system, if not initialized already."),
            new(new string[] { "dir", "ls" }, "Lists files and folders"),
            new(new string[] { "cd", "cd\\", "cd..", "cd-" }, "Changes directory"),
            new(new string[] { "format" }, "Formats disk using FAT32", new string[] {"[disk number]"} ),
            new(new string[] { "diskinfo" }, "Shows disk info and usage", new string[] {"[disk number]"} ),
            new(new string[] { "mkdir" }, "Makes a directory", new string[] {"[folder name]"} ),
            new(new string[] { "touch" }, "Creates a file", new string[] {"[filename]"} ),
            new(new string[] { "cat" }, "Displays file content", new string[] {"[filename]"} ),
            new(new string[] { "rm", "rmdir" }, "Removes file / dir", new string[] { "[filename] / [folder name]" } ),
            })
        {
        }
        internal override int Execute(string[] args, string rawInput, CommandShell shell)
        {
            /*bool silent = false;
            foreach (string arg in args.Skip(1))
            {
                if (arg == "/s") { silent = true; }
            }*/
            if (args.Length > 1) { args[1] = rawInput.Remove(0, args[0].Length).Trim(); }
            switch (args[0])
            {
                case "init":
                    shell.Print = FileManager.Start();
                    return 0;
                case "format":
                    shell.Print = "Now formatting " + args[1] + ":";
                    FileManager.Format(Convert.ToInt32(args[1].Replace(":", "")));
                    shell.Print += "\nDone";
                    return 0;
                case "dir":
                case "ls":
                    string txt = "";
                    if (path.Equals("Computer"))
                    {
                        txt = "Computer:\n\n";
                        //int diskcount = 0;
                        int partcount = 0;
                        int sizeCount = 0;
                        foreach (Disk disk in FileManager.fs.GetDisks())
                        {
                            foreach (ManagedPartition partition in disk.Partitions)
                            {
                                long usedSpace = partition.MountedFS.Size - partition.MountedFS.TotalFreeSpace / 1024 / 1024;
                                txt += " " + partition.MountedFS.Type + " " + partition.RootPath + " - " + partition.MountedFS.Label + (Kernel.GUIenabled ? "       " : "\t") + usedSpace.ToString() + " MB / " + partition.MountedFS.Size.ToString() + " MB\n";
                                partcount++;
                                sizeCount += Convert.ToInt32(partition.MountedFS.Size);
                            }
                            //diskcount++;
                        }
                        //if (diskcount == 1) { txt += ("Total: " + diskcount + " Disk | "); }
                        //else { txt += ("Total: " + diskcount + " Disks | "); }
                        txt += "\nTotal: ";
                        if (partcount == 1) { txt += (partcount + " Partition - "); }
                        else { txt += (partcount + " Partitions - "); }
                        txt += sizeCount + " MB";
                        shell.Print = txt;
                        
                        return 0;
                    }
                    int dircount = 0;
                    int filecount = 0;
                    int filesize = 0;
                    txt = ("Directory of " + path + "\n");
                    var directory_list = VFSManager.GetDirectoryListing(path);
                    foreach (var directoryEntry in directory_list)
                    {
                        if (directoryEntry.mEntryType == Cosmos.System.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                        {
                            txt += "\n " + directoryEntry.mSize + " bytes" + new string(' ', 20 - (directoryEntry.mSize + " bytes").Length) + directoryEntry.mName;
                            filecount++;
                            filesize += Convert.ToInt32(directoryEntry.mSize);
                        }
                        else
                        {
                            txt += "\n DIR" + new string(' ', 17) + directoryEntry.mName;
                            dircount++;
                        }
                    }
                    if (dircount == 1) { txt += ("\n\nTotal: " + dircount + " Dir | "); }
                    else { txt += ("\n\nTotal: " + dircount + " Dirs | "); }
                    if (filecount == 1) { txt += (filecount + " File - "); }
                    else { txt += (filecount + " Files - "); }
                    txt += (Convert.ToInt32(filesize / 1024f / 1024f) + " MB - " + filesize + " B");
                    shell.Print = txt;
                    return 0;
                case "cd":
                    if (args.Length == 1)
                    {
                        shell.Print = "CD: " + path;
                    }
                    else
                    {
                        if (args[1].Equals("Computer", StringComparison.OrdinalIgnoreCase))
                        {
                            undoPath = path;
                            path = "Computer";
                            shell.prompt = CommandShell.defaultPrompt;
                            return 0;
                        }
                        else if (char.IsDigit(args[1][0]) && args[1].Contains(':'))
                        {
                            undoPath = path;
                            path = args[1];
                        }
                        else if (args[1].Equals("-"))
                        {
                            string i = Convert.ToString(Directory.GetParent(path));
                            path = (i == "") ? "Computer" : path;
                        }
                        else if (args[1].Equals(".."))
                        {
                            Execute(new string[] { "cd..", }, "cd..", shell);
                            return 0;
                        }
                        else
                        {
                            undoPath = path;
                            if (path.EndsWith("\\")) { path += args[1]; }
                            else { path += "\\" + args[1]; }
                        }
                        if (!VFSManager.DirectoryExists(path))
                        {
                            shell.Print = ("\nPath does not exist");
                            path = undoPath;
                        }
                        selDisk = int.Parse(path[0].ToString());
                        shell.prompt = path + " >";
                    }
                    return 0;
                case "cd\\":
                    path = selDisk.ToString() + ":\\";
                    shell.prompt = path + " >";
                    return 0;
                case "cd..":
                    string j = Convert.ToString(Directory.GetParent(path));
                    path = (j == "") ? "Computer" : j;
                    if (path.Equals("Computer"))
                    {
                        shell.prompt = CommandShell.defaultPrompt;
                    }
                    else
                    {
                        shell.prompt = path + " >";
                    }
                    return 0;
                case "cd-":
                    path = undoPath;
                    if (path.Equals("Computer"))
                    {
                        shell.prompt = CommandShell.defaultPrompt;
                    }
                    else
                    {
                        shell.prompt = path + " >";
                    }
                    return 0;
                case "diskinfo":
                    int sel = selDisk;
                    if (args.Length > 1)
                    {
                        sel = Convert.ToInt32(args[1].Replace(":", ""));
                    }
                    string label = VFSManager.GetFileSystemLabel(sel + ":\\");
                    string fs_type = VFSManager.GetFileSystemType(sel + ":\\");
                    double spaceB = VFSManager.GetTotalSize(sel + ":\\");
                    double available_spaceB = VFSManager.GetAvailableFreeSpace(sel + ":\\");
                    double used_spaceB = spaceB - available_spaceB;
                    double space = spaceB / 1024f / 1024f;
                    double available_space = available_spaceB / 1024f / 1024f;
                    double used_space = space - available_space;
                    shell.Print = (label + " (" + sel + ":\\)\n\nFile System: " + fs_type + " | Capacity: " + space + " MB - " + spaceB + " B\nUsed Space: " + Convert.ToInt32(used_space) + " MB - " + used_spaceB + " B | Free Space: " + Convert.ToInt32(available_space) + " MB - " + available_spaceB + " B");
                    return 0;
                case "mkdir":
                    if (char.IsDigit(args[1][0]) && args[1].Contains(':')) { FileManager.NewFolder(args[1]); }
                    else { FileManager.NewFolder(path + "\\" + args[1]); }
                    return 0;
                case "cat":
                    if (char.IsDigit(args[1][0]) && args[1].Contains(':')) { shell.Print = FileManager.Open(args[1]); }
                    else { shell.Print = FileManager.Open(path + "\\" + args[1]); }
                    return 0;
                case "touch":
                    if (char.IsDigit(args[1][0]) && args[1].Contains(':')) { FileManager.NewFile(args[1], ""); }
                    else { FileManager.NewFile(path + "\\" + args[1], ""); }
                    return 0;
                case "rm":
                    if (char.IsDigit(args[1][0]) && args[1].Contains(':')) { FileManager.Delete(args[1], false); }
                    else { FileManager.Delete(path + "\\" + args[1], false); }
                    return 0;
                case "rmdir":
                    if (char.IsDigit(args[1][0]) && args[1].Contains(':')) { FileManager.Delete(args[1], true); }
                    else { FileManager.Delete(path + "\\" + args[1], true); }
                    return 0;
            }
            return 1;
        }
    }
}