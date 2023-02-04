using Cosmos.HAL.BlockDevice.Registers;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.Listing;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using Sys = Cosmos.System;

namespace NclearOS2
{
    public static class Files
    {
        public static CosmosVFS fs = new();

        public static string CD = "Computer";
        public static string disks = "";
        public static string undoDir = "Computer";

        public static string SaveContent;

        public static string listresult;
        public static List<string> RAWlistresult = new();

        public static List<string> Type = new();

        public static string selectionType;
        public static int selectionY = 0;
        public static string selected;
        public static int clickedOn;

        private static int doubleClick = -1;
        private static int currentDisk;

        public static int disknumbermax = 0;
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY + 20, x, y);
            Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, StartX, StartY, x, 20);
            Kernel.canvas.DrawString(CD, Kernel.font, Kernel.WhitePen, StartX + 5, StartY + 25);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + CD.Length * 8 + 10, StartY + 31, StartX + x, StartY + 31);
            if (selectionY != 0) { Kernel.canvas.DrawFilledRectangle(Kernel.GrayPen, StartX + 5, selectionY + StartY, x - 10, 16); }
            if (MouseManager.Y > StartY + 45 && Sys.MouseManager.Y < StartY + y && Sys.MouseManager.X > StartX + 10 && Sys.MouseManager.X < StartX + x)
            {
                clickedOn = ((int)Sys.MouseManager.Y - StartY + 55) / 16 - 5;
                Kernel.canvas.DrawString("Position: " + clickedOn, Kernel.font, Kernel.WhitePen, StartX + x - 100, StartY + y - 10);
                if (Kernel.Pressed)
                {
                    if (CD == "Computer")
                    {
                        if (clickedOn - 1 < disknumbermax)
                        {
                            if (doubleClick == clickedOn - 1)
                            {
                                currentDisk = clickedOn - 1;
                                CD = currentDisk + ":\\";
                                doubleClick = -1;
                                RefreshList(true);
                            }
                            else
                            {
                                selectionY = clickedOn * 16 + 28;
                                doubleClick = clickedOn - 1;
                            }
                        }
                        else { selectionY = 0; doubleClick = -1; }
                    }
                    else
                    {
                        if (clickedOn <= RAWlistresult.Count)
                        {
                            if (selected == RAWlistresult[clickedOn - 1])
                            {
                                selected = null;
                                selectionY = 0;
                                if (Type[clickedOn - 1].Equals("DIR"))
                                {
                                    undoDir = CD;
                                    if (CD.EndsWith('\\')) { CD += RAWlistresult[clickedOn - 1]; }
                                    else { CD += '\\' + RAWlistresult[clickedOn - 1]; }
                                    RefreshList(true);
                                }
                                else
                                {
                                    Kernel.Loading = true;
                                    string keep = CD;
                                    CD += "\\" + RAWlistresult[clickedOn - 1];
                                    if (Type[clickedOn - 1].Equals("BMP"))
                                    {
                                        Kernel.WallpaperOn = true;
                                        Kernel.wallpaper = new Bitmap(Files.OpenInBytes(CD));
                                    }
                                    else
                                    {
                                        Process.Run(Process.Apps.notepad);
                                        Input.input = Files.Open(CD);
                                        Notepad.filePath = CD;
                                    }
                                    CD = keep;
                                    return;
                                }
                                selectionType = null;
                            }
                            else
                            {
                                selected = RAWlistresult[clickedOn - 1];
                                selectionY = clickedOn * 16 + 28;
                                if (Type[clickedOn - 1].Equals("DIR"))
                                {
                                    selectionType = "DIR";
                                }
                                else
                                {
                                    selectionType = Type[clickedOn - 1] + " File";
                                }
                            }
                        }
                        else
                        {
                            selectionY = 0; selectionType = null; selected = null;
                        }
                    }
                }
            }
            if (CD == "Computer")
            {
                Kernel.canvas.DrawImageAlpha(Kernel.disk, StartX + 10, StartY + y - 10);
                Graphic.DisplayText(disks, StartX + 10, StartY + 45, Kernel.WhitePen);
                if (selectionY != 0)
                {
                    Kernel.canvas.DrawString("Format", Kernel.font, Kernel.WhitePen, StartX + 10, StartY + 1);
                    if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 5 && MouseManager.X < StartX + 50)
                    {
                        Kernel.canvas.DrawString("Format", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed)
                        {
                            Format(clickedOn - 1);
                        }
                    }
                }
            }
            else
            {
                Kernel.canvas.DrawString("<  ^  New File  New Folder", Kernel.font, Kernel.WhitePen, StartX + 10, StartY + 1);
                if (selectionType == "DIR" || selectionType.Contains("File")) { Kernel.canvas.DrawString("Rename  Delete  Move  Copy", Kernel.font, Kernel.WhitePen, StartX + 230, StartY + 1); }
                if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 5 && MouseManager.X < StartX + x)
                {
                    if (MouseManager.X < StartX + 25)
                    {
                        Kernel.canvas.DrawString("<", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed)
                        {
                            if (undoDir == CD)
                            {
                                CD = "Computer";
                            }
                            else
                            {
                                CD = undoDir;
                                if (CD != "Computer") { RefreshList(true); }
                            }
                        }
                    }
                    else if (MouseManager.X < StartX + 55)
                    {
                        Kernel.canvas.DrawString("   ^", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed)
                        {
                            if (CD.Replace("\\", "") == currentDisk + ":")
                            {
                                CD = "Computer";
                            }
                            else
                            {
                                CD = Convert.ToString(System.IO.Directory.GetParent(CD));
                                RefreshList(true);
                            }
                        }
                    }
                    else if (MouseManager.X < StartX + 130)
                    {
                        Kernel.canvas.DrawString("      New File", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed) { NewFile(CD + "\\New file"); RefreshList(false); }
                    }
                    else if (MouseManager.X < StartX + 220)
                    {
                        Kernel.canvas.DrawString("                New Folder", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed) { NewFolder(CD + "\\New folder"); RefreshList(false); }
                    }

                    if (selectionType == "DIR" || selectionType.Contains("File") && MouseManager.X > StartX + 225)
                    {
                        if (MouseManager.X < StartX + 290)
                        {
                            Kernel.canvas.DrawString("Rename", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                Rename(CD + "\\" + selected, "Renamed");
                            }
                        }
                        else if (MouseManager.X < StartX + 350)
                        {
                            Kernel.canvas.DrawString("        Delete", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                Delete(CD + "\\" + selected);
                            }
                        }
                        else if (MouseManager.X < StartX + 400)
                        {
                            Kernel.canvas.DrawString("                Move", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                Move(CD + "\\" + selected, "0:\\");
                            }
                        }
                        else if (MouseManager.X < StartX + 420)
                        {
                            Kernel.canvas.DrawString("                      Copy", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                Copy(CD + "\\" + selected);
                            }
                        }
                    }


                }
                Graphic.DisplayText(listresult, StartX + 10, StartY + 45, Kernel.WhitePen);
                if (selectionType == null || selectionType == "DIR") { Kernel.canvas.DrawImageAlpha(Kernel.filesicon, StartX + 10, StartY + y - 10); }
                else { Kernel.canvas.DrawImageAlpha(Kernel.fileicon, StartX + 10, StartY + y - 10); Kernel.canvas.DrawString(selectionType, Kernel.font, Kernel.WhitePen, StartX + 40, StartY + y - 7); }
            }
            if (SaveContent != null)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, StartX, StartY + y + 20, x, 20);
                Window.name = "Files - Save file";
                Graphic.DisplayText("File name: " + Input.input, StartX + 10, StartY + y + 22, Kernel.WhitePen, true);
                if (Input.Listener())
                {
                    Kernel.Refresh();
                    Save(CD + Input.input, SaveContent);
                    SaveContent = null;
                    return;
                }
            }
        }
        public static string Init()
        {
            try
            {
                VFSManager.RegisterVFS(fs);
                if (!VFSManager.DirectoryExists("0:\\NclearOS"))
                {
                    System.Console.WriteLine("Creating System Folder 0:\\NclearOS...");
                    fs.CreateDirectory("0:\\NclearOS");
                    System.Console.WriteLine("System Folder created");
                }
                Kernel.UseDisks = true;
                return "Successfully initialized File System";
            }
            catch (Exception e)
            {
                Kernel.UseDisks = false;
                return e.ToString();
            }
        }
        public static void ListDisks()
        {
            disks = "";
            bool diskslistdone = false;
            for (int diskslistnumber = 0; diskslistdone == false; diskslistnumber++)
            {
                try
                {
                    string label = VFSManager.GetFileSystemLabel(diskslistnumber + ":\\");
                    string fs_type = VFSManager.GetFileSystemType(diskslistnumber + ":\\");
                    double space = VFSManager.GetTotalSize(diskslistnumber + ":\\");
                    double available_space = VFSManager.GetAvailableFreeSpace(diskslistnumber + ":\\");
                    space = space / 1024f / 1024f;
                    double used_space = space - (available_space / 1024f / 1024f);
                    disks += diskslistnumber + ":\\ - " + label + new string(' ', 48 - label.Length) + fs_type + " | " + Convert.ToInt32(used_space) + " MB / " + space + " MB\n";
                }
                catch
                {
                    disknumbermax = diskslistnumber;
                    diskslistdone = true; // =)
                }
            }
        }
        public static void RefreshList(bool ClearSelection)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            try
            {
                if (ClearSelection) { selectionY = 0; selectionType = null; selected = null; }
                listresult = "";
                RAWlistresult.Clear();
                Type.Clear();
                int dircount = 0;
                int filecount = 0;
                int filesize = 0;
                var directory_list = VFSManager.GetDirectoryListing(CD);
                foreach (var directoryEntry in directory_list)
                {
                    try
                    {
                        if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                        {
                            Type.Add(directoryEntry.mName.Substring(directoryEntry.mName.IndexOf('.') + 1).ToUpper());
                            if (directoryEntry.mName.Length < 48) { listresult += directoryEntry.mName + new string(' ', 48 - directoryEntry.mName.Length) + Type[Type.Count - 1] + " File | " + directoryEntry.mSize + " bytes\n"; }
                            else { listresult += directoryEntry.mName + "\n"; }
                            filecount++;
                            filesize += Convert.ToInt32(directoryEntry.mSize);
                        }
                        else if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory)
                        {
                            if (directoryEntry.mName.Length < 48) { listresult += directoryEntry.mName + new string(' ', 48 - directoryEntry.mName.Length) + "Folder\n"; }
                            else { listresult += directoryEntry.mName + "\n"; }
                            Type.Add("DIR");
                            dircount++;
                        }
                        else
                        {
                            if (directoryEntry.mName.Length < 48) { listresult += directoryEntry.mName + new string(' ', 48 - directoryEntry.mName.Length) + "UNKNOWN\n"; }
                            else { listresult += directoryEntry.mName + "\n"; }
                            Type.Add("UNKNOWN");
                        }
                        RAWlistresult.Add(directoryEntry.mName);
                    }
                    catch
                    {
                        listresult += "UNKNOWN";
                        Type.Add("UNKNOWN");
                        RAWlistresult.Add("UNKNOWN");
                    }
                    if (ClearSelection) { Kernel.Refresh(); }
                }
                if (dircount == 1) { listresult += ("\nTotal: " + dircount + " Dir | "); }
                else { listresult += ("\nTotal: " + dircount + " Dirs | "); }
                if (filecount == 1) { listresult += (filecount + " File - "); }
                else { listresult += (filecount + " Files - "); }
                listresult += (Convert.ToInt32(filesize / 1024f / 1024f) + " MB - " + filesize + " B\n");
            }
            catch
            {
                Msg.Main("Error: '" + CD + "' is not available.", true);
                Kernel.Refresh();
                CD = undoDir;
                if (CD != "Computer") { RefreshList(true); }
            }
            Kernel.Loading = false;
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
            else { return "File access denied"; }
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
            else { return null; }
        }
        public static void Save(string path, string toSave)
        {
            Kernel.Loading = true;
            Msg.Main("Saving " + path + "...", false);
            Kernel.Refresh();
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
                    Msg.Main("Saved " + path, false);
                }
                else
                {
                    Msg.Main("Cannot write to " + path, true);
                }
            }
            catch (Exception e)
            {
                Msg.Main("Failed saving " + path + "; " + e, true);
            }
            Kernel.Loading = false;
        }
        public static void Format(int diskIndex, string format = "FAT32", bool quicky = true)
        {
            Kernel.Loading = true;
            Msg.Main("Formatting " + diskIndex + ":\\ - file system " + format + "...", false);
            Kernel.Refresh();
            try
            {
                fs.Disks[diskIndex].FormatPartition(diskIndex, format, quicky);
                Msg.Main("Successfully formatted " + diskIndex + ":\\ - file system " + format + " - Restart PC to see changes", false);
            }
            catch (Exception e)
            {
                Msg.Main("Failed formatting " + diskIndex + ":\\ - file system " + format + "; " + e, true);
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
        public static void Delete(string path)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else { File.Delete(path); }
            }
            catch (Exception e) { Msg.Main("Failed deleting " + path + "; " + e, true); }
            RefreshList(true);
            Kernel.Loading = false;
        }
        public static void Rename(string path, string newName)
        {
            Msg.Main("Not implemented yet");
        }
        public static void Move(string oldPath, string newPath)
        {
            Msg.Main("Not implemented yet");
        }
        public static void Copy(string path)//, string newPath)
        {
            Kernel.Loading = true;
            Msg.Main("Copying " + path + " ...");
            Kernel.Refresh();
            try
            {
                if (File.Exists(path))
                {
                    Save(NewFile(path, ""), Open(path));
                }
                else if (Directory.Exists(path))
                {
                    NewFolder(path);

                    Msg.Main("Copied " + path);
                    RefreshList(false);
                    return;
                    var directory_list = VFSManager.GetDirectoryListing(path);
                    foreach (var directoryEntry in directory_list)
                    {
                        Copy(NewFolder(path));
                    }
                }
                RefreshList(false);
                Msg.Main("Copied " + path);
            }
            catch (Exception e) { Msg.Main("Failed copying " + path + "; " + e, true); }
            RefreshList(true);
            Kernel.Loading = false;
        }
    }
}