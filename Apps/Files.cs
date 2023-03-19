using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using F = NclearOS2.FileManager;
using Sys = Cosmos.System;

namespace NclearOS2
{
    public class Files : Window
    {
        public Files(int x, int y) : base("Files", x, y, Resources.filesicon) { }
        public string CD = "Computer";
        public static string disks = "";
        public static string undoDir = "Computer";

        public static string tempPath;
        public static string tempName;
        public static string toDelete;

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
        internal override bool Start()
        {
            if (!Kernel.UseDisks) { Msg.Main("Files", "File system not initialized; Type fs.init in Console to access files", Resources.warn); return false; }
            if (F.diskList.Count == 0)
            {
                disks = F.ListDisks();
                if(F.diskList.Count == 0) { Msg.Main("Files", "No FAT32 partitions found", Resources.warn); return false; }
            }
            return true;
        }
        internal override bool Update(int StartX, int StartY, int x, int y)
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
                                    Kernel.Refresh();
                                    if (Type[clickedOn - 1].Equals("BMP"))
                                    {
                                        Kernel.WallpaperOn = true;
                                        Resources.wallpaper = new Bitmap(F.OpenInBytes(CD + "\\" + RAWlistresult[clickedOn - 1]));
                                    }
                                    else
                                    {
                                        ProcessManager.Add(new Notepad((int)(Kernel.screenX - 250), (int)(Kernel.screenY - 170), CD + "\\" + RAWlistresult[clickedOn - 1]));
                                    }
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
                Kernel.canvas.DrawImageAlpha(Resources.disk, StartX + 10, StartY + y - 10);
                Graphic.TextView(disks, StartX + 10, StartY + 45, Kernel.WhitePen);
                if (selectionY != 0)
                {
                    Kernel.canvas.DrawString("Format", Kernel.font, Kernel.WhitePen, StartX + 10, StartY + 1);
                    if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 5 && MouseManager.X < StartX + 50)
                    {
                        Kernel.canvas.DrawString("Format", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed)
                        {
                            F.Format(clickedOn - 1);
                        }
                    }
                }
            }
            else
            {
                Kernel.canvas.DrawString("<  ^  New File  New Folder", Kernel.font, Kernel.WhitePen, StartX + 10, StartY + 1);
                if (selectionType == "DIR" || selectionType.Contains("File"))
                {
                    Kernel.canvas.DrawString("Rename  Delete  Cut  Copy", Kernel.font, Kernel.WhitePen, StartX + 230, StartY + 1);

                }
                if (!string.IsNullOrWhiteSpace(tempPath))
                {
                    Kernel.canvas.DrawString("Paste", Kernel.font, Kernel.WhitePen, StartX + 445, StartY + 1);
                }
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
                        if (Kernel.Pressed) { F.NewFile(CD + "\\New file"); RefreshList(false); }
                    }
                    else if (MouseManager.X < StartX + 220)
                    {
                        Kernel.canvas.DrawString("                New Folder", Kernel.font, Kernel.GrayPen, StartX + 10, StartY + 1);
                        if (Kernel.Pressed) { F.NewFolder(CD + "\\New folder"); RefreshList(false); }
                    }
                    else if (MouseManager.X < StartX + 490 && MouseManager.X > StartX + 440 && !string.IsNullOrWhiteSpace(tempPath))
                    {
                        Kernel.canvas.DrawString("Paste", Kernel.font, Kernel.GrayPen, StartX + 445, StartY + 1);
                        if (Kernel.Pressed)
                        {
                            F.Paste(tempPath + "\\" + tempName, CD, tempName, true);
                        }
                    }

                    if (selectionType == "DIR" || selectionType.Contains("File") && MouseManager.X > StartX + 225)
                    {
                        if (MouseManager.X < StartX + 290)
                        {
                            Kernel.canvas.DrawString("Rename", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                F.Rename(CD + "\\" + selected, "Renamed");
                            }
                        }
                        else if (MouseManager.X < StartX + 350)
                        {
                            Kernel.canvas.DrawString("        Delete", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                if(selectionType == "DIR") { F.Delete(CD + "\\" + selected, true); }
                                else{ F.Delete(CD + "\\" + selected); }
                            }
                        }
                        else if (MouseManager.X < StartX + 390)
                        {
                            Kernel.canvas.DrawString("                Cut", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                Cut(CD, selected);
                            }
                        }
                        else if (MouseManager.X < StartX + 435)
                        {
                            Kernel.canvas.DrawString("                     Copy", Kernel.font, Kernel.GrayPen, StartX + 230, StartY + 1);
                            if (Kernel.Pressed)
                            {
                                Copy(CD, selected);
                            }
                        }
                    }


                }
                Graphic.TextView(listresult, StartX + 10, StartY + 45, Kernel.WhitePen);
                if (selectionType == null || selectionType == "DIR") { Kernel.canvas.DrawImageAlpha(Resources.filesicon, StartX + 10, StartY + y - 10); }
                else { Kernel.canvas.DrawImageAlpha(Resources.fileicon, StartX + 10, StartY + y - 10); Kernel.canvas.DrawString(selectionType, Kernel.font, Kernel.WhitePen, StartX + 40, StartY + y - 7); }
            }
            if (SaveContent != null)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, StartX, StartY + y + 20, x, 20);
                /*Window.name = "Files - Save file";
                Graphic.TextView("File name: " + Input.input, StartX + 10, StartY + y + 22, Kernel.WhitePen, true);
                if (Input.Listener())
                {
                    Kernel.Refresh();
                    Save(CD + Input.input, SaveContent);
                    SaveContent = null;
                    return;
                }*/
            }
            return true;
        }
        internal override int Stop() { return 0; }
        public void RefreshList(bool ClearSelection)
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
                var directory_list = F.fs.GetDirectoryListing(CD);
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
                Msg.Main("Error", CD + "' is not available.", Resources.error);
                Kernel.Refresh();
                CD = undoDir;
                if (CD != "Computer") { RefreshList(true); }
            }
            Kernel.Loading = false;
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
        internal override void Key(ConsoleKeyEx key)
        {
            if (CD != "Computer")
            {
                switch (key)
                {
                    case ConsoleKeyEx.F5:
                        RefreshList(true);
                        break;
                }
                if (selectionType == "DIR" || selectionType.Contains("File"))
                {
                    switch (key)
                    {
                        case ConsoleKeyEx.Delete:
                            if (selectionType == "DIR") { F.Delete(CD + "\\" + selected, true); }
                            else { F.Delete(CD + "\\" + selected); }
                            break;
                        case ConsoleKeyEx.F2:
                            F.Rename(CD + "\\" + selected, "Renamed");
                            break;
                    }
                    if (KeyboardManager.ControlPressed)
                    {
                        switch (key)
                        {
                            case ConsoleKeyEx.C:
                                Copy(CD, selected);
                                break;
                            case ConsoleKeyEx.V:
                                F.Paste(tempPath + "\\" + tempName, CD, tempName, true);
                                break;
                            case ConsoleKeyEx.X:
                                Cut(CD, selected);
                                break;
                            case ConsoleKeyEx.N:
                                if (KeyboardManager.ShiftPressed) { F.NewFolder(CD); }
                                break;
                        }
                    }
                }
            }
            if (KeyboardManager.AltPressed)
            {
                switch (key)
                {
                    case ConsoleKeyEx.LeftArrow:
                        if (undoDir == CD)
                        {
                            CD = "Computer";
                        }
                        else
                        {
                            CD = undoDir;
                            if (CD != "Computer") { RefreshList(true); }
                        }
                        break;
                    case ConsoleKeyEx.UpArrow:
                        if (CD.Replace("\\", "") == currentDisk + ":")
                        {
                            CD = "Computer";
                        }
                        else
                        {
                            CD = Convert.ToString(System.IO.Directory.GetParent(CD));
                            RefreshList(true);
                        }
                        break;
                }
            }
        }
    }
}