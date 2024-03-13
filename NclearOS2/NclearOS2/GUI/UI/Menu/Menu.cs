using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Drawing;

namespace NclearOS2.GUI
{
    internal class Menu : Process
    {
        public Menu() : base("Menu", ProcessManager.Priority.None) { Start(); }
        private static Bitmap bar;
        public static Bitmap menuBitmap;

        public static bool Opened
        {
            get { return opened; }
            set
            {
                if (value) { openingOrOpened = true; Animation.Running.Add(new(menuBitmap, Animation.Property.TranslationY, (short)(GUI.ScreenY-30-menuBitmap.Height), (short)GUI.ScreenY, 50, 0, new(() => { opened = value; GUI.canvas.DrawImage(menuBitmap, 0, (int)GUI.ScreenY - 340); }))); }
                else { opened = openingOrOpened = powerMenu = false; GUI.canvas.DrawImage(menuBitmap, 0, (int)GUI.ScreenY - 340); Animation.Running.Add(new(menuBitmap, Animation.Property.TranslationY, (short)GUI.ScreenY, (short)(GUI.ScreenY - 30 - menuBitmap.Height), 50)); }
            }
        }
        private static bool opened;
        public static bool openingOrOpened;
        public static bool powerMenu;

        public static Bitmap start;
        public static Bitmap start2;
        public static Bitmap start3;

        public List<(string name, Bitmap icon, Action action)> appList = new();

        internal override int Start()
        {
            appList.Clear();
            appList.Add(("Settings", new Bitmap(Resources.Settings), () => ProcessManager.Run(new Settings())));
            appList.Add(("Notepad", new Bitmap(Resources.Notepad), () => ProcessManager.Run(new Notepad((int)(GUI.ScreenX - 200), (int)(GUI.ScreenY - 170)))));
            appList.Add(("Console", new Bitmap(Resources.ConsoleIcon), () => ProcessManager.Run(new ConsoleApp((int)(GUI.ScreenX - 200), (int)(GUI.ScreenY - 100)))));
            appList.Add(("Files", new Bitmap(Resources.Files), () => ProcessManager.Run(new Files(640, (int)(GUI.ScreenY - 170)))));
            appList.Add(("System Info", new Bitmap(Resources.InfoSystemIcon), () => ProcessManager.Run(new InfoSystem())));
            appList.Add(("Process Manager", new Bitmap(Resources.TaskmngIcon), () => ProcessManager.Run(new TaskManager())));
            
            bar = PostProcess.CropBitmap(Images.wallpaperBlur, 0, (int)GUI.ScreenY - 30, (int)GUI.ScreenX, 30);
            menuBitmap = PostProcess.CropBitmap(Images.wallpaperBlur, 0, (int)GUI.ScreenY - 340, 249, 310);
            
            int textY = 15;
            foreach (var item in appList)
            {
                Font.DrawString(item.name, -1, 40, textY, menuBitmap.rawData, 249);
                Font.DrawImageAlpha(item.icon, 10, textY - 5, menuBitmap.rawData, 249);
                textY += 30;
            }
            Font.DrawImageAlpha(Icons.lockicon, 40, 280, menuBitmap.rawData, 249);
            //Font.DrawString("Lock", -1, 40, 225, menuBitmap.rawData, 249);

            //Font.DrawImageAlpha(Icons.reboot, 10, 250, menuBitmap.rawData, 249);
            //Font.DrawString("Restart", -1, 40, 255, menuBitmap.rawData, 249);

            Font.DrawImageAlpha(Icons.shutdown, 10, 280, menuBitmap.rawData, 249);
            //Font.DrawString("Shutdown", -1, 40, 285, menuBitmap.rawData, 249);
            return 0;
        }
        internal override void Update()
        {
            GUI.canvas.DrawImage(bar, 0, (int)GUI.ScreenY - 30);
            GUI.canvas.DrawString(NclearOS2.Date.CurrentTime(true), GUI.font, GUI.WhitePen, (int)GUI.ScreenX - 70, (int)GUI.ScreenY - 20);
            if (Kernel.useNetwork) { GUI.canvas.DrawImageAlpha(Icons.connected, (int)GUI.ScreenX - 100, (int)GUI.ScreenY - 26); }
            byte j = 0;
            for (byte i = 0; i < ProcessManager.running.Count; i++)
            {
                if (ProcessManager.running[i] is Window w)
                {
                    GUI.canvas.DrawImageAlpha(w.icon, j * 40 + 50, (int)GUI.ScreenY - 27); j++;
                }
            }
            if (MouseManager.Y > GUI.ScreenY - 30)
            {
                if (MouseManager.X < 34)
                {
                    if (GUI.Pressed)
                    {
                        Opened = !Opened;
                        if (!Opened) { GUI.canvas.DrawImageAlpha(start2, 5, (int)GUI.ScreenY - 27); }
                    }
                    else if (!Opened)
                    { GUI.canvas.DrawImageAlpha(start2, 5, (int)GUI.ScreenY - 27); }
                }
                else if (MouseManager.X > (int)GUI.ScreenX - 75)
                {
                    if (!Opened) { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.ScreenY - 27); }
                    if (GUI.Pressed) { ProcessManager.Run(new Date()); }
                    else { GUI.canvas.DrawString(NclearOS2.Date.CurrentDate(true, false), GUI.font, GUI.WhitePen, (int)GUI.ScreenX - 200, (int)GUI.ScreenY - 45); }
                }
                else if (Kernel.useNetwork && MouseManager.X > (int)GUI.ScreenX - 100)
                {
                    if (!Opened) { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.ScreenY - 27); }
                    if (GUI.Pressed) { }
                    else { GUI.canvas.DrawString(NclearOS2.Net.GetInfo(), GUI.font, GUI.WhitePen, (int)GUI.ScreenX - 250, (int)GUI.ScreenY - 45); }
                }
                else
                {
                    if (!Opened) { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.ScreenY - 27); }
                    int clicked = ((int)MouseManager.X - 40) / 40;
                    if (clicked < ProcessManager.running.Count)
                    {
                        if (ProcessManager.running[clicked] is Window w && clicked < ProcessManager.running.Count(p => p is Window))
                        {
                            Font.DrawString(w.name, System.Drawing.Color.White, clicked * 40 + 40, (int)GUI.ScreenY - 45);
                            //GUI.canvas.DrawImage(PostProcess.ResizeBitmap(w.appCanvas, 100, 50), clicked * 40 + 40, (int)GUI.ScreenY - 145);
                            if (GUI.Pressed)
                            {
                                if (clicked == 0 && !w.minimized)
                                {
                                    w.Minimize();
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
            else if (openingOrOpened)
            { GUI.canvas.DrawImageAlpha(start3, 5, (int)GUI.ScreenY - 27); }
            else if (!Opened)
            { GUI.canvas.DrawImageAlpha(start, 5, (int)GUI.ScreenY - 27); }
            if (Opened)
            {
                GUI.canvas.DrawImage(menuBitmap, 0, (int)GUI.ScreenY - 340);
                GUI.canvas.DrawImageAlpha(start3, 5, (int)GUI.ScreenY - 27);
                //GUI.canvas.DrawFilledRectangle(GUI.SystemPen, 0, (int)GUI.ScreenY - 340, 250, 310);
                if (powerMenu)
                {
                    GUI.canvas.DrawImageAlpha(Icons.reboot, 10, GUI.ScreenY - 120);
                    Font.DrawString("Restart", Color.White, 40, GUI.ScreenY - 115);

                    GUI.canvas.DrawImageAlpha(Icons.shutdown, 10, GUI.ScreenY - 90);
                    Font.DrawString("Shutdown", Color.White, 40, GUI.ScreenY - 85);
                }
                if (GUI.Pressed)
                {
                    if (MouseManager.Y < (int)GUI.ScreenY - 340 | MouseManager.X > 250) { Opened = false; }
                    else
                    {
                        int clicked = ((int)MouseManager.Y - (int)GUI.ScreenY + 330) / 30;
                        if (clicked < appList.Count)
                        {
                            Opened = false;
                            appList[clicked].action.Invoke();
                        }
                        if (MouseManager.Y < (int)GUI.ScreenY - 30 && MouseManager.Y > (int)GUI.ScreenY - 60)
                        {
                            if (MouseManager.X < 37) { powerMenu = !powerMenu; }
                            else if (MouseManager.X < 67) { Opened = false; GUI.Lock = true; }
                        }
                        if(powerMenu)
                        {
                            if (MouseManager.Y < (int)GUI.ScreenY - 90 && MouseManager.Y > (int)GUI.ScreenY - 120)
                            {
                                Kernel.Shutdown(true);
                            }
                            else if (MouseManager.Y < (int)GUI.ScreenY - 60 && MouseManager.Y > (int)GUI.ScreenY - 90)
                            {
                                Kernel.Shutdown(false);
                            }
                        }
                    }
                }

            }
        }
    }
    public static class Desktop
    {
        static int[] debug = new int[3];
        public static void Click(int x, int y)
        {
            //if (Kernel.Debug) { debug[0] = GUI.fps/5; debug[1] = x; debug[2] = y; }
        }
        public static void Update()
        {
            GUI.canvas.DrawImage(Images.wallpaper, 0, 0);
            //if (debug[0] > 0) { debug[0]--; GUI.canvas.DrawImageAlpha(Icons.error, debug[1], debug[2]); }
        }
    }
}