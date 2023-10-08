using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using NclearOS2.GUI;
using System;
using System.Drawing;
using System.Collections.Generic;
using F = NclearOS2.FileManager;
using Sys = Cosmos.System;
using System.Linq;
using Cosmos.System.FileSystem.VFS;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Cosmos.System.FileSystem;
using System.ComponentModel.Design;

namespace NclearOS2.GUI
{
    internal class Files : Window
    {
        public Files(int x, int y) : base("Files", x, y, new Bitmap(Resources.Files), Priority.None) { OnKeyPressed = Key; OnClicked = OnPressed; }
        public string cd = "Computer";
        private int selY = -1;
        public string CD
        {   get { return cd; }
            set
            {
                GUI.Loading = true;
                cd = value;
                selY = -1;
                Background(GUI.DarkGrayPen.ValueARGB);
                DrawString(CD, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 5, 25);
                DrawHorizontalLine(Color.White.ToArgb(), CD.Length * Font.fontX + 10, 32, x - 10);
                if(CD != "Computer") { DrawString("<  ^  New File  New Folder", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 5); }
                DrawImageAlpha(refreshIcon, x - 20, 5);
                DrawString(RefreshList(), Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, this.y - 30);
                int i = -1;
                foreach (var el in listresult)
                {
                    Print(++i, GUI.DarkGrayPen.ValueARGB);
                    GUI.Refresh();
                }
                GUI.Loading = false;
            }
        }
        public static string undoDir = "Computer";

        private static Bitmap diskIcon = new Bitmap(Resources.DiskIcon);
        private static Bitmap fileIcon = new Bitmap(Resources.FileIcon);
        private static Bitmap refreshIcon = new Bitmap(Resources.RefreshIcon);

        public static string tempPath;
        public static string tempName;
        public static string toDelete;

        public static List<string[]> listresult = new();
        internal override int Start()
        {
            if (!Kernel.useDisks) { Msg.Main("Files", "File system not initialized; Type 'init' in Console to access files.", Icons.warn); return -1; }
            if (F.fs.GetDisks().Count == 0) { Msg.Main("Files", "No FAT32 partitions found", Icons.warn); return -1; }
            Background(GUI.DarkGrayPen.ValueARGB);
            CD = "Computer";
            return 0;
        }
        internal override void Update() { }
        internal override int Stop() { return 0; }
        private void Key(KeyEvent key)
        {
            switch (key.Key)
            {
                case ConsoleKeyEx.F5:
                    CD = CD;
                    break;
            }
            if (CD != "Computer")
            {
                if (selY != -1)
                {
                    switch (key.Key)
                    {
                        case ConsoleKeyEx.Delete:
                            if (listresult[selY][0] == "DIR")
                            {
                                F.Delete(CD + "\\" + listresult[selY][1], true);
                            }
                            else
                            {
                                F.Delete(CD + "\\" + listresult[selY][1], false);
                            }
                            CD = CD;
                            break;
                    }
                }
                if (KeyboardManager.AltPressed)
                {
                    switch (key.Key)
                    {
                        case ConsoleKeyEx.LeftArrow:
                            GoBack();
                            break;
                        case ConsoleKeyEx.UpArrow:
                            GoUp();
                            break;
                    }
                }
            }
        }
        private void Print(int index, int bgcolor)
        {
            if(index + 6 > this.y / 20) { DrawString("...", Color.White.ToArgb(), bgcolor, 15, this.y - 60); return; }
            if (listresult[index][1].Length > 48) { DrawString(listresult[index][1] + " " + listresult[index][0], Color.White.ToArgb(), bgcolor, 15, index * 20 + 50); }
            else
            {
                switch (listresult[index].Length)
                {
                    case 5:
                        if(listresult[index][1] == listresult[index][2])
                        {
                            DrawString(listresult[index][1] + new string(' ', 40 - listresult[index][1].Length) + listresult[index][0] + " | " + listresult[index][3] + " MB / " + listresult[index][4] + " MB", Color.White.ToArgb(), bgcolor, 15, index * 20 + 50);
                        }
                        else
                        {
                            DrawString(listresult[index][1] + " - " + listresult[index][2] + new string(' ', 40 - listresult[index][2].Length + listresult[index][1].Length + 3) + listresult[index][0] + " | " + listresult[index][3] + " MB / " + listresult[index][4] + " MB", Color.White.ToArgb(), bgcolor, 15, index * 20 + 50);
                        }
                        break;
                    case 3:
                        DrawString(listresult[index][1] + new string(' ', 50 - listresult[index][1].Length) + listresult[index][0] + " File | " + listresult[index][2] + " bytes\n", Color.White.ToArgb(), bgcolor, 15, index * 20 + 50);
                        break;
                    default:
                        DrawString(listresult[index][1] + new string(' ', 50 - listresult[index][1].Length) + "Folder", Color.White.ToArgb(), bgcolor, 15, index * 20 + 50);
                        break;
                }
            }
        }
        private void OnPressed(int x, int y)
        {
            if(y > 50 && y < listresult.Count * 20 + 50 && y < this.y - 60)
            {
                if(selY == (y - 50) / 20)
                {
                    if (listresult[selY][0] == "DIR")
                    {
                        undoDir = CD;
                        if (CD.EndsWith('\\')) { CD += listresult[selY][1]; }
                        else { CD += '\\' + listresult[selY][1]; }
                        return;
                    }
                    else
                    {
                        if (CD == "Computer")
                        {
                            CD = listresult[selY][1]; return;
                        }
                        else
                        {
                            if (CD.EndsWith('\\')) { ProcessManager.Run(new Notepad((int)(GUI.screenX - 250), (int)(GUI.screenY - 170), CD + listresult[selY][1])); }
                            else { ProcessManager.Run(new Notepad((int)(GUI.screenX - 250), (int)(GUI.screenY - 170), CD + "\\" + listresult[selY][1])); }
                            
                            DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 49, this.x - 20, Font.fontY + 2);
                            Print(selY, GUI.DarkGrayPen.ValueARGB);
                            selY = -1;
                            return;
                        }
                    }
                }
                else if(selY != -1)
                {
                    DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 49, this.x - 20, Font.fontY + 2);
                    Print(selY, GUI.DarkGrayPen.ValueARGB);
                }
                selY = (y - 50) / 20;
                DrawFilledRectangle(Color.Gray.ToArgb(), 10, selY * 20 + 49, this.x - 20, Font.fontY + 2);
                Print(selY, Color.Gray.ToArgb());
                if(CD == "Computer")
                {
                    DrawString("Format", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 10, 5);
                }
                else
                {
                    DrawString("Delete  Copy", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 230, 5);
                }
            }
            else if(y < 20)
            {
                GUI.Loading = true;
                GUI.Refresh();
                if(CD == "Computer" && selY != -1 && x < 70)
                {
                    F.Format(Convert.ToInt32(listresult[selY][1].Split(':')[0]));
                    
                }
                else if(x < 25)
                {
                    GoBack();
                    return;
                }
                else if(x < 55)
                {
                    GoUp();
                    return;
                }
                else if (x < 130)
                {
                    F.NewFile(CD + "\\New file");
                }
                else if (x < 220)
                {
                    F.NewFolder(CD + "\\New folder");
                }
                else if (x < 290 && selY != -1)
                {
                    if(listresult[selY][0] == "DIR")
                    {
                        F.Delete(CD + "\\" + listresult[selY][1], true);
                    }
                    else
                    {
                        F.Delete(CD + "\\" + listresult[selY][1], false);
                    }
                }
                else if (x < 350 && selY != -1)
                {
                    if (listresult[selY][0] == "DIR")
                    {
                        F.CopyDirectory(CD + "\\" + listresult[selY][1], F.NewFolder(CD + "\\" + listresult[selY][1]), false);
                    }
                    else
                    {
                        F.CopyFile(CD + "\\" + listresult[selY][1]);
                    }
                }
                else if (x > this.x - 25) { }
                else { GUI.Loading = false; return; }
                CD = CD;
            }
            else if (selY != -1)
            {
                DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 49, this.x - 20, Font.fontY + 2);
                Print(selY, GUI.DarkGrayPen.ValueARGB);
            }
        }
        private string RefreshList()
        {
            listresult.Clear();
            string status = "";
            if (cd == "Computer")
            {
                //int diskcount = 0;
                int partcount = 0;
                foreach (Disk disk in F.fs.GetDisks())
                {
                    foreach (ManagedPartition partition in disk.Partitions)
                    {
                        long used_space = partition.MountedFS.Size - partition.MountedFS.TotalFreeSpace / 1024 / 1024;
                        listresult.Add(new string[] { partition.MountedFS.Type, partition.RootPath, partition.MountedFS.Label, used_space.ToString(), partition.MountedFS.Size.ToString() });
                        partcount++;
                    }
                    //diskcount++;
                }
                //if (diskcount == 1) { status += ("Total: " + diskcount + " Disk | "); }
                //else { status += ("Total: " + diskcount + " Disks | "); }
                if (partcount == 1) { status += (partcount + " Partition"); }
                else { status += (partcount + " Partitions"); }
                return status;
            }
            int dircount = 0;
            int filecount = 0;
            int filesize = 0;
            var directory_list = VFSManager.GetDirectoryListing(cd);
            foreach (var directoryEntry in directory_list)
            {
                try
                {
                    if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                    {
                        listresult.Add(new string[] { directoryEntry.mName.Substring(directoryEntry.mName.IndexOf('.') + 1).ToUpper(), directoryEntry.mName, directoryEntry.mSize.ToString() });
                        filecount++;
                        filesize += Convert.ToInt32(directoryEntry.mSize);
                    }
                    else if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.Directory)
                    {
                        listresult.Add(new string[] { "DIR", directoryEntry.mName });
                        dircount++;
                    }
                    else
                    {
                        listresult.Add(new string[] { "UNKNOWN", directoryEntry.mName });
                    }
                }
                catch (Exception e)
                {
                    listresult.Add(new string[] { "UNKNOWN", e.Message });
                }
            }
            if (dircount == 1) { status += ("Total: " + dircount + " Folder | "); }
            else { status += ("Total: " + dircount + " Folders | "); }
            if (filecount == 1) { status += (filecount + " File - "); }
            else { status += (filecount + " Files - "); }
            status += (Convert.ToInt32(filesize / 1024f / 1024f) + " MB - " + filesize + " B");
            return status;
        }
        private void GoBack()
        {
            if (undoDir == CD)
            {
                CD = "Computer";
            }
            else
            {
                CD = undoDir;
            }
        }
        private void GoUp()
        {
            string path = Convert.ToString(Directory.GetParent(CD));
            CD = (path == "") ? "Computer" : path;
        }
        public void Cut(string path, string name)
        {
            tempPath = path;
            tempName = name;
            toDelete = path + "\\" + name;
        }
        public void Copy(string path, string name)
        {
            tempPath = path;
            tempName = name;
            toDelete = null;
        }
    }
}