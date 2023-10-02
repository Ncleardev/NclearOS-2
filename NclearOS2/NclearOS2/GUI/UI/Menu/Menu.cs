using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NclearOS2.GUI
{
    internal class Menu : Process
    {
        public Menu() : base("Menu", Priority.Realtime, true)
        {
            if (appList.Count == 0)
            {
                appList.Add(("Settings", new Bitmap(Resources.Settings), () => ProcessManager.Run(new Settings())));
                appList.Add(("Notepad", new Bitmap(Resources.Notepad), () => ProcessManager.Run(new Notepad((int)(GUI.screenX - 200), (int)(GUI.screenY - 170)))));
                appList.Add(("Console", new Bitmap(Resources.ConsoleIcon), () => ProcessManager.Run(new ConsoleApp((int)(GUI.screenX - 200), (int)(GUI.screenY - 100)))));
                appList.Add(("Files", new Bitmap(Resources.Files), () => ProcessManager.Run(new Files(640, (int)(GUI.screenY - 170)))));
                appList.Add(("System Info", new Bitmap(Resources.InfoSystemIcon), () => ProcessManager.Run(new InfoSystem())));
                appList.Add(("Process Manager", new Bitmap(Resources.TaskmngIcon), () => ProcessManager.Run(new TaskManager())));
            }
            if (Images.wallpaperBlur == null)
            {
                MemoryOperations.Fill(bar.rawData, GUI.SystemPen.ValueARGB);
                //MemoryOperations.Fill(bgmn.rawData, GUI.SystemPen.ValueARGB);
            }
            else
            {
                bar = PostProcess.CropBitmap(Images.wallpaperBlur, 0, (int)GUI.screenY - 30, (int)GUI.screenX, 30);
                //bgmn = PostProcess.CropBitmap(Images.wallpaperBlur, 0, (int)GUI.screenY - 340, 250, 310);
            }
        }
        private static Bitmap bar;
        //private static Bitmap bgmn;

        public static bool Opened;
        public int dClick;

        public static Bitmap start;
        public static Bitmap start2;
        public static Bitmap start3;

        public List<(string name, Bitmap icon, Action action)> appList = new();

        internal override int Start()
        {
            return 0;
        }
        internal override void Update()
        {
            //GUI.canvas.DrawFilledRectangle(GUI.SystemPen, 0, (int)GUI.screenY - 30, (int)GUI.screenX, 30);
            GUI.canvas.DrawImage(bar, 0, (int)GUI.screenY - 30);
            GUI.canvas.DrawString(NclearOS2.Date.CurrentTime(true), GUI.font, GUI.WhitePen, (int)GUI.screenX - 70, (int)GUI.screenY - 20);
            if (Kernel.useNetwork) { GUI.canvas.DrawImageAlpha(Icons.connected, (int)GUI.screenX - 100, (int)GUI.screenY - 26); }
            for (int i = 0; i < ProcessManager.running.Count; i++)
            {
                if (ProcessManager.running[i] is Window w)
                { GUI.canvas.DrawImageAlpha(w.icon, i * 40 + 50, (int)GUI.screenY - 27); }
            }
            if (MouseManager.Y > GUI.screenY - 30)
            {
                if (MouseManager.X < 34)
                {
                    if (GUI.Pressed)
                    {
                        Opened = !Opened;
                        dClick = 0;
                        if (!Opened) { GUI.canvas.DrawImageAlpha(start2, 5, (int)GUI.screenY - 27); }
                    }
                    else if (!Opened)
                    { GUI.canvas.DrawImageAlpha(start2, 5, (int)GUI.screenY - 27); }
                }
                else if (MouseManager.X > (int)GUI.screenX - 75)
                {
                    if (!Opened) { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.screenY - 27); }
                    if (GUI.Pressed) { ProcessManager.Run(new Date()); }
                    else { GUI.canvas.DrawString(NclearOS2.Date.CurrentDate(true, false), GUI.font, GUI.WhitePen, (int)GUI.screenX - 200, (int)GUI.screenY - 45); }
                }
                else if (Kernel.useNetwork && MouseManager.X > (int)GUI.screenX - 100)
                {
                    if (!Opened) { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.screenY - 27); }
                    if (GUI.Pressed) { }
                    else { GUI.canvas.DrawString(NclearOS2.Net.GetInfo(), GUI.font, GUI.WhitePen, (int)GUI.screenX - 200, (int)GUI.screenY - 45); }
                }
                else
                {
                    if (!Opened) { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.screenY - 27); }
                    int clicked = ((int)MouseManager.X - 40) / 40;
                    if (GUI.Pressed && clicked < ProcessManager.running.Count)
                    {
                        if (ProcessManager.running[clicked] is Window w && clicked < ProcessManager.running.Count(p => p is Window))
                        {
                            if (clicked == 0 && !w.minimized)
                            {
                                w.minimized = true;
                            }
                            else
                            {
                                WindowManager.FocusAtWindow(clicked);
                            }
                        }
                    }
                }
            }
            else if (!Opened)
            { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.screenY - 27); }
            if (Opened)
            {
                GUI.canvas.DrawImageAlpha(start3, 5, (int)GUI.screenY - 27);
                GUI.canvas.DrawFilledRectangle(GUI.SystemPen, 0, (int)GUI.screenY - 340, 250, 310);
                //GUI.canvas.DrawImage(bgmn, 0, (int)GUI.screenY - 340);
                int iconY = (int)GUI.screenY - 330;
                int textY = (int)GUI.screenY - 325;
                foreach (var item in appList)
                {
                    GUI.canvas.DrawString(item.name, GUI.font, GUI.WhitePen, 40, textY);
                    GUI.canvas.DrawImageAlpha(item.icon, 10, iconY);
                    iconY += 30; textY += 30;
                }
                GUI.canvas.DrawImageAlpha(Icons.lockicon, 10, (int)GUI.screenY - 120);
                GUI.canvas.DrawString("Lock", GUI.font, GUI.WhitePen, 40, (int)GUI.screenY - 115);

                GUI.canvas.DrawImageAlpha(Icons.reboot, 10, (int)GUI.screenY - 90);
                GUI.canvas.DrawString("Restart", GUI.font, GUI.WhitePen, 40, (int)GUI.screenY - 85);

                GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, (int)GUI.screenY - 60);
                GUI.canvas.DrawString("Shutdown", GUI.font, GUI.WhitePen, 40, (int)GUI.screenY - 55);
                if (GUI.Pressed)
                {
                    if (MouseManager.Y < (int)GUI.screenY - 340 | MouseManager.X > 250)
                    {
                        Opened = false;
                    }
                    else
                    {
                        int clicked = ((int)MouseManager.Y - (int)GUI.screenY + 330) / 30;
                        if (clicked < appList.Count)
                        {
                            Opened = false;
                            appList[clicked].action.Invoke();
                        }

                        if (MouseManager.Y < (int)GUI.screenY - 90 && MouseManager.Y > (int)GUI.screenY - 120)
                        {
                            Opened = false;
                            GUI.Lock = true;
                        }
                        else if (MouseManager.Y < (int)GUI.screenY - 60 && MouseManager.Y > (int)GUI.screenY - 90)
                        {
                            if (dClick == 1) { Kernel.Shutdown(true); }
                            else { Toast.msg = "Press again to restart"; dClick = 1; }
                        }
                        else if (MouseManager.Y < (int)GUI.screenY - 30 && MouseManager.Y > (int)GUI.screenY - 60)
                        {
                            if (dClick == 2) { Kernel.Shutdown(false); }
                            else { Toast.msg = "Press again to shutdown"; dClick = 2; }
                        }
                    }
                }
            }
        }
        internal override int Stop()
        {
            return 0;
        }
    }
}