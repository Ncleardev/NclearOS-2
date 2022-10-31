using Cosmos.System.Graphics;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.FileSystem;
using System.IO;
using Sys = Cosmos.System;

namespace NclearOS2
{
    public static class Files
    {
        public static string CD = "Computer - to change directory use ' cd 'directory' ' in Command App";
        public static string disks = "";
        public static string seldisk = "0";
        public static string filename = "New file";
        public static string undoDir = "Computer";
        public static string textfromfile;
        public static string listresult;
        public static int disknumbermax = 0;
        public static bool acknowledged;
        public static int listlenght;
        public static int clickedOn;
        public static void Command(string option)
        {
            Kernel.Loading = true;
            Kernel.Refresh();
            switch (option)
            {
                case "wallpaper":
                    Msg.Main("Successfully changed background", false);
                    break;
                case "cursorwhite":
                    Kernel.cursor = new Bitmap(Kernel.CursorWhiteIcon);
                    Kernel.cursorload = new Bitmap(Kernel.CursorWhiteLoad);
                    Msg.Main("Successfully changed cursor theme", false);
                    break;
                case "cursordark":
                    Kernel.cursor = new Bitmap(Kernel.CursorIcon);
                    Kernel.cursorload = new Bitmap(Kernel.CursorLoad);
                    Msg.Main("Successfully changed cursor theme", false);
                    break;
                case "default":
                    Kernel.SystemPen.Color = Color.SteelBlue;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "red":
                    Kernel.SystemPen.Color = Color.DarkRed;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "green":
                    Kernel.SystemPen.Color = Color.Green;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "yellow":
                    Kernel.SystemPen.Color = Color.Goldenrod;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "darkblue":
                    Kernel.SystemPen.Color = Color.MidnightBlue;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "gray":
                    Kernel.SystemPen.Color = Color.FromArgb(40, 40, 40);
                    Msg.Main("Successfully changed theme color", false);
                    break;
                case "black":
                    Kernel.SystemPen.Color = Color.Black;
                    Msg.Main("Successfully changed theme color", false);
                    break;
                default:
                    Msg.Main("Error, type settings for help", true);
                    break;
            }
            Kernel.Loading = false;
        }
        public static void Update(int StartX, int StartY, int x, int y)
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.DarkGrayPen, StartX, StartY, x, y);
            Kernel.canvas.DrawString(CD, Kernel.font, Kernel.WhitePen, StartX+5, StartY+5);
            Kernel.canvas.DrawLine(Kernel.WhitePen, StartX + CD.Length*8 + 10, StartY + 11, StartX + x, StartY + 11);
            if(CD == "Computer" || CD == "Computer - to change directory use ' cd 'directory' ' in Command App")
            {
                Kernel.canvas.DrawImageAlpha(Kernel.disk, StartX + 10, StartY + y - 30);
                Window.DisplayText(disks, StartX + 10, StartY + 25, Kernel.WhitePen);
            }
            else
            {
                if (Kernel.Pressed) { RefreshList(); }
                Kernel.canvas.DrawImageAlpha(Kernel.fileicon, StartX + 10, StartY + y - 30);
                Window.DisplayText(listresult, StartX + 10, StartY + 25, Kernel.WhitePen);
            }
            if (Sys.MouseManager.Y > StartY + 25 && Sys.MouseManager.Y < StartY + y && Sys.MouseManager.X > StartX + 10 && Sys.MouseManager.X < StartX + x)
            {
                Kernel.canvas.DrawString(Convert.ToString(((int)Sys.MouseManager.Y - StartY + 25) / 16), Kernel.font, Kernel.WhitePen, 10, 10);
                if (Kernel.Pressed)
                {
                    clickedOn = ((int)Sys.MouseManager.Y - StartY + 25) / 16;
                }
            }
        }
        public static void Init()
        {
            try
            {
                CosmosVFS fs = new CosmosVFS();
                VFSManager.RegisterVFS(fs);
            }
            catch { return; }
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
                    disks+= "Volume " + diskslistnumber + " - " + label + " (" + fs_type + ") | " + Convert.ToInt32(used_space) + " MB / " + space + " MB\n";
                }
                catch
                {
                    disknumbermax = diskslistnumber;
                    diskslistdone = true; // =)
                }
            }
        }
        public static void RefreshList()
        {
            listresult = "";
            int dircount = 0;
            int filecount = 0;
            int filesize = 0;
            var directory_list = VFSManager.GetDirectoryListing(CD);
            foreach (var directoryEntry in directory_list)
            {
                if (directoryEntry.mEntryType == Sys.FileSystem.Listing.DirectoryEntryTypeEnum.File)
                {
                    if(directoryEntry.mName.Length < 64) { listresult += directoryEntry.mName + new string(' ', 64 - directoryEntry.mName.Length) + directoryEntry.mSize + " bytes\n"; }
                    else { listresult += directoryEntry.mName + "\n"; }
                    filecount++;
                    filesize += Convert.ToInt32(directoryEntry.mSize);
                }
                else
                {
                    if (directoryEntry.mName.Length < 64) { listresult += directoryEntry.mName + new string(' ', 64 - directoryEntry.mName.Length) + "Folder\n"; }
                    else { listresult += directoryEntry.mName + "\n"; }
                    dircount++;
                }
            }
            if (dircount == 1) { listresult+= ("\nTotal: " + dircount + " Dir | "); }
            else { listresult+= ("\nTotal: " + dircount + " Dirs | "); }
            if (filecount == 1) { listresult+= (filecount + " File - "); }
            else { listresult+= (filecount + " Files - "); }
            listresult+= (Convert.ToInt32(filesize / 1024f / 1024f) + " MB - " + filesize + " B\n");
        }
    }
}