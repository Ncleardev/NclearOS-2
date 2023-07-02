using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.Threading;

namespace NclearOS2.GUI
{
    public static class Resources
    {
        //Curosrs
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Cursors.Cursor.bmp")]
        public static byte[] Cursor;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Cursors.CursorLoad.bmp")]
        public static byte[] CursorLoad;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Cursors.CursorWhite.bmp")]
        public static byte[] CursorWhite;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Cursors.CursorWhiteLoad.bmp")]
        public static byte[] CursorWhiteLoad;
        //UI
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.loading.bmp")]
        public static byte[] Load;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.check.bmp")]
        public static byte[] Check;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.check2.bmp")]
        public static byte[] Check2;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.circle.bmp")]
        public static byte[] Circle;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.circle2.bmp")]
        public static byte[] Circle2;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.warn.bmp")]
        public static byte[] Warn;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.info.bmp")]
        public static byte[] Info;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.error.bmp")]
        public static byte[] Error;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.close.bmp")]
        public static byte[] CloseButton;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.close2.bmp")]
        public static byte[] Close2;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.minimize.bmp")]
        public static byte[] Minimize;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.minimize2.bmp")]
        public static byte[] Minimize2;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.taskmng.bmp")]
        public static byte[] TaskmngIcon;
        //Walpapers
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Wallpapers.Wallpaper.bmp")]
        public static byte[] Wallpaper;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Wallpapers.WallpaperLock.bmp")]
        public static byte[] WallpaperLock;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Wallpapers.WallpaperOld.bmp")]
        public static byte[] WallpaperOld;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Wallpapers.2005s.bmp")]
        public static byte[] Wallpaper2005s;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Wallpapers.Origami.bmp")]
        public static byte[] WallpaperOrigami;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.Wallpapers.Cosmos.bmp")]
        public static byte[] WallpaperCosmos;

        //Icons
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Resources.UI.program.bmp")]
        public static byte[] Program;

        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.Console.console.bmp")]
        public static byte[] ConsoleIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.SysInfo.sysinfo.bmp")]
        public static byte[] SysInfo;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.Settings.settings.bmp")]
        public static byte[] Settings;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.Files.files.bmp")]
        public static byte[] Files;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.Notepad.notepad.bmp")]
        public static byte[] Notepad;

        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.Files.disk.bmp")]
        public static byte[] DiskIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.Apps.Files.file.bmp")]
        public static byte[] FileIcon;

        //Menu
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.reboot.bmp")]
        public static byte[] Reboot;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.shutdown.bmp")]
        public static byte[] Shutdown;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.lock.bmp")]
        public static byte[] LockIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.start.bmp")]
        public static byte[] StartButton;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.start2.bmp")]
        public static byte[] Start2;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.start3.bmp")]
        public static byte[] Start3;
        [ManifestResourceStream(ResourceName = "NclearOS2.GUI.UI.Menu.connected.bmp")]
        public static byte[] Connected;

        public static void InitResources()
        {
            //Cursors
            Icons.cursor = new Bitmap(Cursor);
            Icons.cursorload = new Bitmap(CursorLoad);
            //UI
            Icons.warn = new Bitmap(Warn);
            Icons.info = new Bitmap(Info);
            Icons.error = new Bitmap(Error);
            Icons.load = new Bitmap(Load);

            Icons.close = new Bitmap(CloseButton);
            Icons.close2 = new Bitmap(Close2);
            Icons.minimize = new Bitmap(Minimize);
            Icons.minimize2 = new Bitmap(Minimize2);

            Icons.program = new Bitmap(Program);

            Icons.lockicon = new Bitmap(LockIcon);
            Icons.reboot = new Bitmap(Reboot);
            Icons.shutdown = new Bitmap(Shutdown);
            Icons.connected = new Bitmap(Connected);

            Menu.start = new Bitmap(StartButton);
            Menu.start2 = new Bitmap(Start2);
            Menu.start3 = new Bitmap(Start3);
        }
    }
}