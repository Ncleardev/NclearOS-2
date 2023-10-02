using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using NclearOS2.GUI;
using System;
using System.Drawing;
using System.Collections.Generic;
using F = NclearOS2.FileManager;
using Sys = Cosmos.System;
using System.Collections;

namespace NclearOS2.GUI
{
    internal class FilesLegacy : Window
    {
        public FilesLegacy(int x, int y) : base("Files", x, y, new Bitmap(Resources.Files), Priority.Realtime) { OnKeyPressed = Key; }
        public string cd = "Computer";
        public string CD
        {   get { return cd; }
            set
            {
                cd = value;
                if(value == "Computer")
                {
                    DrawString(disks, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, StartX + 15, StartY + 15);
                }
                else { DrawString(listresult, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, StartX + 15, StartY + 15); }
                DrawString(cd, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, StartX + 5, StartY + 5);
            }
        }
        public static string disks = "";
        public static string undoDir = "Computer";

        private static Bitmap diskIcon = new Bitmap(Resources.DiskIcon);
        private static Bitmap fileIcon = new Bitmap(Resources.FileIcon);

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
        internal override int Start()
        {
            if (!Kernel.useDisks) { Msg.Main("Files", "File system not initialized; Type 'init' in Console to access files.", Icons.warn); return -1; }
            if (F.diskList.Count == 0)
            {
                //disks = string.Join("\n", F.ListDisks().ToArray());
                if (F.diskList.Count == 0) { Msg.Main("Files", "No FAT32 partitions found", Icons.warn); return -1; }
            }
            Background(GUI.DarkGrayPen.ValueARGB);
            DrawFilledRectangle(GUI.SystemPen.ValueARGB, StartX, StartY, x, 10);
            DrawString(CD, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, StartX + 5, StartY + 5);
            DrawString(disks, Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, StartX + 15, StartY + 15);
            return 0;
        }
        internal override void Update()
        {
            if (!minimized)
            {
                //DrawHorizontalLine(Color.White, StartX + CD.Length * 8 + 10, StartY + 31, StartX + x, StartY + 31);
                if (selectionY != 0) { DrawImageAlpha(Icons.lockicon, 10, selectionY); }
                if (MouseManager.Y > StartY + 45 && Sys.MouseManager.Y < StartY + y && Sys.MouseManager.X > StartX + 10 && Sys.MouseManager.X < StartX + x)
                {
                    clickedOn = ((int)Sys.MouseManager.Y - StartY + 55) / 16 - 5;
                    GUI.canvas.DrawString("Position: " + clickedOn, GUI.font, GUI.WhitePen, StartX + x - 100, StartY + y - 10);
                    if (GUI.Pressed)
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
                                        GUI.Loading = true;
                                        GUI.Refresh();
                                        if (Type[clickedOn - 1].Equals("BMP"))
                                        {
                                            //GUI.WallpaperOn = true;
                                            //Resources.wallpaper = new Bitmap(F.OpenInBytes(CD + "\\" + RAWlistresult[clickedOn - 1]));
                                        }
                                        else
                                        {
                                            //ProcessManager.Add(new Notepad((int)(GUI.screenX - 250), (int)(GUI.screenY - 170), CD + "\\" + RAWlistresult[clickedOn - 1]));
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
                    GUI.canvas.DrawImageAlpha(diskIcon, StartX + 10, StartY + y - 10);
                    if (selectionY != 0)
                    {
                        GUI.canvas.DrawString("Format", GUI.font, GUI.WhitePen, StartX + 10, StartY + 1);
                        if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 5 && MouseManager.X < StartX + 50)
                        {
                            GUI.canvas.DrawString("Format", GUI.font, GUI.GrayPen, StartX + 10, StartY + 1);
                            if (GUI.Pressed)
                            {
                                F.Format(clickedOn - 1);
                            }
                        }
                    }
                }
                else
                {
                    GUI.canvas.DrawString("<  ^  New File  New Folder", GUI.font, GUI.WhitePen, StartX + 10, StartY + 1);
                    if (selectionType == "DIR" || selectionType.Contains("File"))
                    {
                        GUI.canvas.DrawString("Rename  Delete  Cut  Copy", GUI.font, GUI.WhitePen, StartX + 230, StartY + 1);

                    }
                    if (!string.IsNullOrWhiteSpace(tempPath))
                    {
                        GUI.canvas.DrawString("Paste", GUI.font, GUI.WhitePen, StartX + 445, StartY + 1);
                    }
                    if (MouseManager.Y < StartY + 20 && MouseManager.Y > StartY && MouseManager.X > StartX + 5 && MouseManager.X < StartX + x)
                    {
                        if (MouseManager.X < StartX + 25)
                        {
                            GUI.canvas.DrawString("<", GUI.font, GUI.GrayPen, StartX + 10, StartY + 1);
                            if (GUI.Pressed)
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
                            GUI.canvas.DrawString("   ^", GUI.font, GUI.GrayPen, StartX + 10, StartY + 1);
                            if (GUI.Pressed)
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
                            GUI.canvas.DrawString("      New File", GUI.font, GUI.GrayPen, StartX + 10, StartY + 1);
                            if (GUI.Pressed) { F.NewFile(CD + "\\New file"); RefreshList(false); }
                        }
                        else if (MouseManager.X < StartX + 220)
                        {
                            GUI.canvas.DrawString("                New Folder", GUI.font, GUI.GrayPen, StartX + 10, StartY + 1);
                            if (GUI.Pressed) { F.NewFolder(CD + "\\New folder"); RefreshList(false); }
                        }
                        else if (MouseManager.X < StartX + 490 && MouseManager.X > StartX + 440 && !string.IsNullOrWhiteSpace(tempPath))
                        {
                            GUI.canvas.DrawString("Paste", GUI.font, GUI.GrayPen, StartX + 445, StartY + 1);
                            if (GUI.Pressed)
                            {
                                //F.Paste(tempPath + "\\" + tempName, CD, tempName, true);
                            }
                        }

                        if (selectionType == "DIR" || selectionType.Contains("File") && MouseManager.X > StartX + 225)
                        {
                            if (MouseManager.X < StartX + 290)
                            {
                                GUI.canvas.DrawString("Rename", GUI.font, GUI.GrayPen, StartX + 230, StartY + 1);
                                if (GUI.Pressed)
                                {
                                    F.Rename(CD + "\\" + selected, "Renamed");
                                }
                            }
                            else if (MouseManager.X < StartX + 350)
                            {
                                GUI.canvas.DrawString("        Delete", GUI.font, GUI.GrayPen, StartX + 230, StartY + 1);
                                if (GUI.Pressed)
                                {
                                    if (selectionType == "DIR") { F.Delete(CD + "\\" + selected, true); }
                                    else { F.Delete(CD + "\\" + selected); }
                                }
                            }
                            else if (MouseManager.X < StartX + 390)
                            {
                                GUI.canvas.DrawString("                Cut", GUI.font, GUI.GrayPen, StartX + 230, StartY + 1);
                                if (GUI.Pressed)
                                {
                                    Cut(CD, selected);
                                }
                            }
                            else if (MouseManager.X < StartX + 435)
                            {
                                GUI.canvas.DrawString("                     Copy", GUI.font, GUI.GrayPen, StartX + 230, StartY + 1);
                                if (GUI.Pressed)
                                {
                                    Copy(CD, selected);
                                }
                            }
                        }


                    }
                    if (selectionType == null || selectionType == "DIR") { GUI.canvas.DrawImageAlpha(icon, StartX + 10, StartY + y - 10); }
                    else { GUI.canvas.DrawImageAlpha(icon, StartX + 10, StartY + y - 10); GUI.canvas.DrawString(selectionType, GUI.font, GUI.WhitePen, StartX + 40, StartY + y - 7); }
                }
            }


            /*if (SaveContent != null)
            {
                GUI.canvas.DrawFilledRectangle(GUI.SystemPen, StartX, StartY + y + 20, x, 20);
                Window.name = "Files - Save file";
                Graphic.TextView("File name: " + Input.input, StartX + 10, StartY + y + 22, Color.White, true);
                if (Input.Listener())
                {
                    GUI.Refresh();
                    Save(CD + Input.input, SaveContent);
                    SaveContent = null;
                    return;
                }
            }*/
        }
        internal override int Stop() { return 0; }
        public void RefreshList(bool ClearSelection)
        {
            GUI.Loading = true;
            GUI.Refresh();
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
                    if (ClearSelection) { GUI.Refresh(); }
                }
                if (dircount == 1) { listresult += ("\nTotal: " + dircount + " Dir | "); }
                else { listresult += ("\nTotal: " + dircount + " Dirs | "); }
                if (filecount == 1) { listresult += (filecount + " File - "); }
                else { listresult += (filecount + " Files - "); }
                listresult += (Convert.ToInt32(filesize / 1024f / 1024f) + " MB - " + filesize + " B\n");
            }
            catch
            {
                Msg.Main("Error", CD + "' is not available.", Icons.error);
                GUI.Refresh();
                CD = undoDir;
                if (CD != "Computer") { RefreshList(true); }
            }
            GUI.Loading = false;
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
        private void Key(KeyEvent key)
        {
            if (CD != "Computer")
            {
                switch (key.Key)
                {
                    case ConsoleKeyEx.F5:
                        RefreshList(true);
                        break;
                }
                if (selectionType == "DIR" || selectionType.Contains("File"))
                {
                    switch (key.Key)
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
                        switch (key.Key)
                        {
                            case ConsoleKeyEx.C:
                                Copy(CD, selected);
                                break;
                            case ConsoleKeyEx.V:
                                //F.Paste(tempPath + "\\" + tempName, CD, tempName, true);
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
                switch (key.Key)
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