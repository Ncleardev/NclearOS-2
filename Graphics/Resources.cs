using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;
using System.Threading;

namespace NclearOS2
{
    public class Resources
    {
        public static Bitmap check;
        public static Bitmap uncheck;
        public static Bitmap warn;
        public static Bitmap info;
        public static Bitmap error;
        public static Bitmap close;
        public static Bitmap closered;
        public static Bitmap minimize;
        public static Bitmap minimize2;
        public static Bitmap console;
        public static Bitmap cursor;
        public static Bitmap disk;
        public static Bitmap cursorload;
        public static Bitmap wallpaper;
        public static Bitmap wallpapernew;
        public static Bitmap wallpaperold;
        public static Bitmap wallpaperlock;
        public static Bitmap logo;
        public static Bitmap lockicon;
        public static Bitmap program;
        public static Bitmap reboot;
        public static Bitmap settings;
        public static Bitmap shutdown;
        public static Bitmap start;
        public static Bitmap start2;
        public static Bitmap start3;
        public static Bitmap notepad;
        public static Bitmap filesicon;
        public static Bitmap fileicon;
        public static Bitmap sysinfo;

        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.check.bmp")]
        public static byte[] Check;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.uncheck.bmp")]
        public static byte[] Uncheck;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.warn.bmp")]
        public static byte[] Warn;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.info.bmp")]
        public static byte[] Info;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.error.bmp")]
        public static byte[] Error;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.close.bmp")]
        public static byte[] CloseButton;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.closered.bmp")]
        public static byte[] CloseRed;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.minimize.bmp")]
        public static byte[] Minimize;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.minimize2.bmp")]
        public static byte[] Minimize2;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.disk.bmp")]
        public static byte[] Disk;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.files.bmp")]
        public static byte[] FilesIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.file.bmp")]
        public static byte[] FileIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.notepad.bmp")]
        public static byte[] NotepadIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.console.bmp")]
        public static byte[] ConsoleIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.Cursor.bmp")]
        public static byte[] CursorIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.CursorLoad.bmp")]
        public static byte[] CursorLoad;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.CursorWhite.bmp")]
        public static byte[] CursorWhiteIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.CursorWhiteLoad.bmp")]
        public static byte[] CursorWhiteLoad;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.logo.bmp")]
        public static byte[] Logo;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.program.bmp")]
        public static byte[] Program;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.reboot.bmp")]
        public static byte[] Reboot;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.settings.bmp")]
        public static byte[] Settings;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.shutdown.bmp")]
        public static byte[] Shutdown;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.lock.bmp")]
        public static byte[] LockIcon;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.start.bmp")]
        public static byte[] StartButton;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.start2.bmp")]
        public static byte[] Start2;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.start3.bmp")]
        public static byte[] Start3;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.sysinfo.bmp")]
        public static byte[] SysInfo;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.WallpaperOld.bmp")]
        public static byte[] WallpaperFile2;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.WallpaperNew.bmp")]
        public static byte[] WallpaperFile;
        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.WallpaperLock.bmp")]
        public static byte[] WallpaperLock;

        [ManifestResourceStream(ResourceName = "NclearOS2.Resources.settingsUI.bmp")]
        public static byte[] SettingsUI;

        public static void InitResources()
        {
            cursor = new Bitmap(CursorIcon);
            check = new Bitmap(Check);
            uncheck = new Bitmap(Uncheck);
            warn = new Bitmap(Warn);
            info = new Bitmap(Info);
            error = new Bitmap(Error);
            cursorload = new Bitmap(CursorLoad);
            notepad = new Bitmap(NotepadIcon);
            logo = new Bitmap(Logo);
            program = new Bitmap(Program);
            settings = new Bitmap(Settings);
            shutdown = new Bitmap(Shutdown);
            wallpapernew = new Bitmap(WallpaperFile);
            wallpaper = wallpapernew;
            wallpaperold = new Bitmap(WallpaperFile2);
            wallpaperlock = new Bitmap(WallpaperLock);
            start = new Bitmap(StartButton);
            start2 = new Bitmap(Start2);
            start3 = new Bitmap(Start3);
            close = new Bitmap(CloseButton);
            minimize = new Bitmap(Minimize);
            minimize2 = new Bitmap(Minimize2);
            closered = new Bitmap(CloseRed);
            console = new Bitmap(ConsoleIcon);
            reboot = new Bitmap(Reboot);
            lockicon = new Bitmap(LockIcon);
            filesicon = new Bitmap(FilesIcon);
            disk = new Bitmap(Disk);
            fileicon = new Bitmap(FileIcon);
            sysinfo = new Bitmap(SysInfo);
        }
    }
}