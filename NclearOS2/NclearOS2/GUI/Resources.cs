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
        private const string defaultPath = "NclearOS2.GUI";
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.logo.bmp")]
        public static byte[] Logo;
        //Cursors
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Cursors.Cursor.bmp")]
        public static byte[] Cursor;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Cursors.CursorLoad.bmp")]
        public static byte[] CursorLoad;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Cursors.CursorWhite.bmp")]
        public static byte[] CursorWhite;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Cursors.CursorWhiteLoad.bmp")]
        public static byte[] CursorWhiteLoad;
        //UI
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.loading.bmp")]
        public static byte[] Load;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.check.bmp")]
        public static byte[] Check;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.check2.bmp")]
        public static byte[] Check2;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.circle.bmp")]
        public static byte[] Circle;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.circle2.bmp")]
        public static byte[] Circle2;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.warn.bmp")]
        public static byte[] Warn;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.info.bmp")]
        public static byte[] Info;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.error.bmp")]
        public static byte[] Error;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.close.bmp")]
        public static byte[] CloseButton;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.close2.bmp")]
        public static byte[] Close2;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.minimize.bmp")]
        public static byte[] Minimize;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.minimize2.bmp")]
        public static byte[] Minimize2;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.min.bmp")]
        public static byte[] Min;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.min2.bmp")]
        public static byte[] Min2;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.max.bmp")]
        public static byte[] Max;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.max2.bmp")]
        public static byte[] Max2;

        //Wallpapers
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Wallpapers.Wallpaper.bmp")]
        public static byte[] Wallpaper;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Wallpapers.WallpaperLock.bmp")]
        public static byte[] WallpaperLock;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Wallpapers.WallpaperOld.bmp")]
        public static byte[] WallpaperOld;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Wallpapers.2005s.bmp")]
        public static byte[] Wallpaper2005s;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Wallpapers.Origami.bmp")]
        public static byte[] WallpaperOrigami;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.Wallpapers.Cosmos.bmp")]
        public static byte[] WallpaperCosmos;

        //Icons
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.program.bmp")]
        public static byte[] Program;
        [ManifestResourceStream(ResourceName = defaultPath + ".Resources.UI.taskmng.bmp")]
        public static byte[] TaskmngIcon;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Console.console.bmp")]
        public static byte[] ConsoleIcon;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.SysInfo.sysinfo.bmp")]
        public static byte[] InfoSystemIcon;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Settings.settings.bmp")]
        public static byte[] Settings;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Files.files.bmp")]
        public static byte[] Files;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Notepad.notepad.bmp")]
        public static byte[] Notepad;

        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Files.disk.bmp")]
        public static byte[] DiskIcon;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Files.file.bmp")]
        public static byte[] FileIcon;
        [ManifestResourceStream(ResourceName = defaultPath + ".Apps.Files.refresh.bmp")]
        public static byte[] RefreshIcon;

        //Menu
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.reboot.bmp")]
        public static byte[] Reboot;
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.shutdown.bmp")]
        public static byte[] Shutdown;
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.lock.bmp")]
        public static byte[] LockIcon;
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.start.bmp")]
        public static byte[] StartButton;
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.start2.bmp")]
        public static byte[] Start2;
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.start3.bmp")]
        public static byte[] Start3;
        [ManifestResourceStream(ResourceName = defaultPath + ".UI.Menu.connected.bmp")]
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
            Icons.min = new Bitmap(Min);
            Icons.min2 = new Bitmap(Min2);
            Icons.max = new Bitmap(Max);
            Icons.max2 = new Bitmap(Max2);

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