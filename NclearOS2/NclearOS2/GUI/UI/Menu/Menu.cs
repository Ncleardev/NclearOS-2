using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;

namespace NclearOS2.GUI
{
    internal class Menu : Process
    {
        public Menu() : base("Menu", Priority.None) { Start(); }
        private static Bitmap bar;
        private static Bitmap menuBitmap;

        public static bool Opened;
        public byte dClick;

        public static Bitmap start;
        public static Bitmap start2;
        public static Bitmap start3;

        public List<(string name, Bitmap icon, Action action)> appList = new();

        internal override int Start()
        {
            appList.Clear();
            appList.Add(("Settings", new Bitmap(Resources.Settings), () => ProcessManager.Run(new Settings())));
            appList.Add(("Notepad", new Bitmap(Resources.Notepad), () => ProcessManager.Run(new Notepad((int)(GUI.screenX - 200), (int)(GUI.screenY - 170)))));
            appList.Add(("Console", new Bitmap(Resources.ConsoleIcon), () => ProcessManager.Run(new ConsoleApp((int)(GUI.screenX - 200), (int)(GUI.screenY - 100)))));
            appList.Add(("Files", new Bitmap(Resources.Files), () => ProcessManager.Run(new Files(640, (int)(GUI.screenY - 170)))));
            appList.Add(("System Info", new Bitmap(Resources.InfoSystemIcon), () => ProcessManager.Run(new InfoSystem())));
            appList.Add(("Process Manager", new Bitmap(Resources.TaskmngIcon), () => ProcessManager.Run(new TaskManager())));
            if (Images.wallpaperBlur == null)
            {
                MemoryOperations.Fill(bar.rawData, GUI.SystemPen.ValueARGB);
                MemoryOperations.Fill(menuBitmap.rawData, GUI.SystemPen.ValueARGB);
            }
            else
            {
                bar = PostProcess.CropBitmap(Images.wallpaperBlur, 0, (int)GUI.screenY - 30, (int)GUI.screenX, 30);
                menuBitmap = PostProcess.CropBitmap(Images.wallpaperBlur, 0, (int)GUI.screenY - 340, 249, 310);
            }
            int textY = 15;
            foreach (var item in appList)
            {
                Font.DrawString(item.name, -1, 40, textY, menuBitmap.rawData, 249);
                Font.DrawImageAlpha(item.icon, 10, textY - 5, menuBitmap.rawData, 249);
                textY += 30;
            }
            Font.DrawImageAlpha(Icons.lockicon, 10, 220, menuBitmap.rawData, 249);
            Font.DrawString("Lock", -1, 40, 225, menuBitmap.rawData, 249);

            Font.DrawImageAlpha(Icons.reboot, 10, 250, menuBitmap.rawData, 249);
            Font.DrawString("Restart", -1, 40, 255, menuBitmap.rawData, 249);

            Font.DrawImageAlpha(Icons.shutdown, 10, 280, menuBitmap.rawData, 249);
            Font.DrawString("Shutdown", -1, 40, 285, menuBitmap.rawData, 249);
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
                {
                    GUI.canvas.DrawImageAlpha(w.icon, i * 40 + 50, (int)GUI.screenY - 27);
                }
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
                    if (clicked < ProcessManager.running.Count)
                    {
                        if (ProcessManager.running[clicked] is Window w && clicked < ProcessManager.running.Count(p => p is Window))
                        {
                            Font.DrawString(w.name, System.Drawing.Color.White, clicked * 40 + 40, (int)GUI.screenY - 45);
                            if (GUI.Pressed)
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
            }
            else if (!Opened)
            { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.screenY - 27); }
            if (Opened)
            {
                GUI.canvas.DrawImage(menuBitmap, 0, (int)GUI.screenY - 340);
                GUI.canvas.DrawImageAlpha(start3, 5, (int)GUI.screenY - 27);
                //GUI.canvas.DrawFilledRectangle(GUI.SystemPen, 0, (int)GUI.screenY - 340, 250, 310);

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