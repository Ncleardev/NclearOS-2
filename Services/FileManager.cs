using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NclearOS2
{
    public static class FileManager
    {
        public static CosmosVFS fs = new();
        public static List<string> diskList = new();
        public static void Start(bool showMsg = false)
        {
            if (Kernel.UseDisks)
            {
                if (showMsg) { Msg.Main("File System", "File System already initialized", Resources.check); }
            }
            else
            {
                try
                {
                    VFSManager.RegisterVFS(fs, false);
                    if (!VFSManager.DirectoryExists("0:\\NclearOS"))
                    {
                        System.Console.WriteLine("Creating System Folder 0:\\NclearOS...");
                        fs.CreateDirectory("0:\\NclearOS");
                        System.Console.WriteLine("System Folder created");
                    }
                    Kernel.UseDisks = true;
                    if (showMsg) { Msg.Main("File System", "Successfully initialized File System", Resources.check); }
                }
                catch (Exception e)
                {
                    Kernel.UseDisks = false;
                    if (showMsg) { Msg.Main("File System Error", Convert.ToString(e), Resources.error); }
                }
            }
        }
        public static string ListDisks()
        {
            diskList.Clear();
            try
            {
                for (int diskslistnumber = 0; true; diskslistnumber++)
                {
                    string label = fs.GetFileSystemLabel(diskslistnumber + ":\\");
                    string fs_type = fs.GetFileSystemType(diskslistnumber + ":\\");
                    double space = fs.GetTotalSize(diskslistnumber + ":\\");
                    double available_space = fs.GetAvailableFreeSpace(diskslistnumber + ":\\");
                    space = space / 1024f / 1024f;
                    double used_space = space - (available_space / 1024f / 1024f);
                    if (label == diskslistnumber + ":\\")
                    {
                        diskList.Add(diskslistnumber + ":\\" + new string(' ', 48 - label.Length) + fs_type + " | " + Convert.ToInt32(used_space) + " MB / " + space + " MB");
                    }
                    else
                    {
                        diskList.Add(diskslistnumber + ":\\ - " + label + new string(' ', 48 - label.Length) + fs_type + " | " + Convert.ToInt32(used_space) + " MB / " + space + " MB");
                    }
                }
            }
            catch { Files.disknumbermax = diskList.Count; }
            return string.Join('\n', diskList.ToArray());
        }
        public static string Open(string path)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            Kernel.Loading = false;
            var hello_file = VFSManager.GetFile(path);
            var hello_file_stream = hello_file.GetFileStream();

            if (hello_file_stream.CanRead)
            {
                byte[] text_to_read = new byte[hello_file_stream.Length];
                hello_file_stream.Read(text_to_read, 0, (int)hello_file_stream.Length);
                return Encoding.Default.GetString(text_to_read);

            }
            return "File access denied";
        }
        public static byte[] OpenInBytes(string path)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            Kernel.Loading = false;
            var hello_file = VFSManager.GetFile(path);
            var hello_file_stream = hello_file.GetFileStream();

            if (hello_file_stream.CanRead)
            {
                byte[] text_to_read = new byte[hello_file_stream.Length];
                hello_file_stream.Read(text_to_read, 0, (int)hello_file_stream.Length);
                return text_to_read;
            }
            return null;
        }
        public static void Save(string path, string toSave)
        {
            Kernel.Loading = true;
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
                    Toast.msg = "Saved " + path;
                }
                else
                {
                    Msg.Main("Error", "Cannot write to " + path, Resources.error);
                }
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed saving " + path + "; " + e, Resources.error);
            }
            Kernel.Loading = false;
        }
        public static void Format(int diskIndex, string format = "FAT32", bool quicky = true)
        {
            Kernel.Loading = true;
            Toast.msg = "Formatting " + diskIndex + ":\\ - file system " + format + "...";
            Kernel.Refresh();
            try
            {
                fs.Disks[diskIndex].FormatPartition(diskIndex, format, quicky);
                Toast.msg = "Successfully formatted " + diskIndex + ":\\ - file system " + format + " - Restart PC to see changes";
                Kernel.UseDisks = false;
            }
            catch (Exception e)
            {
                Msg.Main("Error", "Failed formatting " + diskIndex + ":\\ - file system " + format + "; " + e, Resources.error);
            }
            Kernel.Loading = false;
        }
        public static string NewFolder(string path)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            if (VFSManager.DirectoryExists(path))
            {
                int i = 1;
                while (VFSManager.DirectoryExists(path + " (" + i + ")"))
                {
                    i++;
                    Kernel.Refresh();
                }
                VFSManager.CreateDirectory(path + " (" + i + ")");
                Kernel.Loading = false;
                return (path + " (" + i + ")");
            }
            else
            {
                VFSManager.CreateDirectory(path);
                Kernel.Loading = false;
                return (path);
            }
        }
        public static string NewFile(string path, string Type = ".txt")
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            if (String.IsNullOrWhiteSpace(Type))
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
                    Kernel.Refresh();
                }
                VFSManager.CreateFile(path + " (" + i + ")" + Type);
                Kernel.Loading = false;
                return (path + " (" + i + ")" + Type);
            }
            else
            {
                VFSManager.CreateFile(path + Type);
                Kernel.Loading = false;
                return (path + Type);
            }
        }
        public static void Delete(string path, bool folder = false)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            try
            {
                if (folder) { Directory.Delete(path, true); }
                else { File.Delete(path); }
            }
            catch (Exception e) { Msg.Main("Error", "Failed deleting " + path + "; " + e, Resources.error); }
            Kernel.Loading = false;
        }
        public static void Rename(string path, string newName)
        {
            Toast.msg = "Not implemented yet";
        }

        /*public static void Paste(string from, string fromName, string target)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            if (string.IsNullOrWhiteSpace(target)){ target = from; }
            string sourcePath = from + fromName;
            string targetPath = target;
            try
            {
                if (toDelete != null)
                {
                    Delete(toDelete);
                }
                tempPath = null;
                tempName = null;
                toDelete = null;
                RefreshList(false);
                Toast.msg = "Copied " + from + "\\" + fromName + " to " + target);
            }
            catch (Exception e) { Toast.msg = "Failed copying " + from + "\\" + fromName + " to " + target + "; " + e, true); }
            RefreshList(true);
            Kernel.Loading = false;
        }*/
        //static public void Paste(string sourceFolder, string add, string destFolder)
        //{
        //    sourceFolder += add;
        //    if (!Directory.Exists(destFolder))
        //        Directory.CreateDirectory(destFolder);
        //    string[] files = Directory.GetFiles(sourceFolder);
        //    foreach (string file in files)
        //    {
        //        string name = Path.GetFileName(file);
        //        string dest = Path.Combine(destFolder, name);
        //        Save(NewFile(dest, ""), Open(file));
        //    }
        //    string[] folders = Directory.GetDirectories(sourceFolder);
        //    foreach (string folder in folders)
        //    {
        //        string name = Path.GetFileName(folder);
        //        string dest = Path.Combine(destFolder, name);
        //        Paste(folder, "", dest);
        //    }
        //}

        public static void Paste(string sourceDir, string destinationDir, string folderName, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir + "\\" + folderName);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir + "\\" + folderName, file.Name);
                file.CopyTo(targetFilePath);
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir + "\\" + folderName, subDir.Name);
                    Paste(subDir.FullName, newDestinationDir, folderName, true);
                }
            }
        }
    }
}