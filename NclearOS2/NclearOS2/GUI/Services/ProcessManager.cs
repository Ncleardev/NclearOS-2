using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace NclearOS2.GUI
{
    public static class ProcessManager
    {
        internal static List<Process> running = new();
        static short second = -1;
        static char second10;
        private static Window hoveredOn;
        internal enum Priority //how often Update() method of Process is called
        {
            None,
            Low, //every 10s
            High, //every second
            Realtime //every Kernel refresh
        }
        public static void Refresh()
        {
            hoveredOn = null;
            bool lowpriority = false;
            bool highpriority = false;
            if (RTC.Second != second)
            {
                highpriority = true;
                second = RTC.Second;
            }
            if (NclearOS2.Date.CurrentSecond()[0] != second10)
            {
                lowpriority = true;
                second10 = NclearOS2.Date.CurrentSecond()[0];
            }
            for (int i = running.Count - 1; i >= 0; i--)
            {
                uint before = GCImplementation.GetUsedRAM();

                switch (running[i].priority)
                {
                    case Priority.Realtime:
                        Update(i);
                        break;
                    case Priority.High when highpriority:
                        Update(i);
                        break;
                    case Priority.Low when lowpriority:
                        Update(i);
                        break;
                }

                if (running[i] is Window w && !w.minimized)
                {
                    WindowManager.Draw(w);
                    if (MouseManager.X >= w.StartX && MouseManager.X <= w.StartX + w.x && MouseManager.Y >= w.StartY && MouseManager.Y <= w.StartY + w.y + 30)
                    {
                        if (w.windowlock) { hoveredOn = null; } else { hoveredOn = w; }
                    }
                }

                running[i].usageRAM += GCImplementation.GetUsedRAM() - before;
            }
        }
        public static void Update(int id)
        {
            try { running[id].Update(); }
            catch (Exception e) { string name = running[id].name; RemoveAt(id); Msg.Main("Error", "Process '" + name + "' crashed: " + e, Icons.error); }
        }
        internal static Process Run(Process process)
        {
            if (!Kernel.GUIenabled) { throw new Exception("GUI mode not enabled!"); }
            GUI.Loading = true;
            GUI.Refresh();
            uint j = GCImplementation.GetUsedRAM();

            int id = process is Window ? 0 : running.Count;

            int code = process.Start();
            if (code == -1) { GUI.Loading = false; return null; }
            else if (code != 0) { Msg.Main("Process Manager", "Process '" + process.name + " launched and then stopped with an unexpected error code: " + code, Icons.warn); GUI.Loading = false; return null; }

            process.usageRAM = (GCImplementation.GetUsedRAM() - j) / 1024;
            GUI.Refresh();
            j = GCImplementation.GetUsedRAM();

            running.Insert(id, process);
            WindowManager.FocusAtWindow(0);

            GUI.Loading = false;
            process.usageRAM += (GCImplementation.GetUsedRAM() - j) / 1024;
            return process;
        }
        internal static int RemoveAt(int i, bool force = false)
        {
            GUI.Loading = true;
            if (force) { running.RemoveAt(i); GUI.Refresh(); GUI.Loading = false; return 0; }
            int exitCode = running[i].Stop(force);
            if (exitCode == 0) { if (running[i] is Window w) { Animation.Running.Add(new((short)w.StartX, (short)(w.StartY), w.borderCanvas, 50)); } running.RemoveAt(i); } //Animation.Running.Add(new(w.appCanvas, PostProcess.CropBitmap(Images.wallpaperBlur, w.StartX, w.StartY + 30, w.x, w.y), (short)w.StartX, (short)(w.StartY+30), 50));
            else if (exitCode != 1)
            {
                //if (exitCode == -1) { if (Run(running[i]) != null) { NotificationSystem.Notify("Process Manager", "System Process '" + running[i + 1] + "' crashed and successfully recovered."); running.RemoveAt(i + 1); } else { NotificationSystem.Notify("Process Manager", "System Process '" + running[i + 1] + "' crashed and unsuccessfully recovered. Reboot recommended."); } running.RemoveAt(i + 1); }
                //else
                //{
                string name = running[i].name;
                running.RemoveAt(i);
                Msg.Main("Process Manager", "Process '" + name + " stopped with an unexpected error code: " + exitCode, Icons.warn);
                //}
            }
            if (running[0] is Window w2) { Font.DrawString(w2.name, Color.White.ToArgb(), 36, 10, w2.borderCanvas.rawData, w2.x); }
            GUI.Loading = false;
            return exitCode;
        }
        public static string StopAll(bool force = false)
        {
            if (!Kernel.GUIenabled || running == null || running.Count == 0) { return null; }
            string apps = null;
            bool animations = GUI.animationEffects;
            GUI.animationEffects = false;
            GUI.Loading = true;
            Process[] processList = running.ToArray();
            foreach (Process process in processList)
            {
                GUI.Refresh();
                string name = process.name;
                try { if (process.Exit(force) == 1) apps += "\n" + name; } catch { }
                GUI.Refresh();
            }
            GUI.Loading = false;
            GUI.animationEffects = animations;
            return apps;
        }
        public static bool Click(int x, int y)
        {
            if (hoveredOn != null) { hoveredOn.Click(x, y); return true; }
            return false;
        }

        public static void LongPress(int x, int y)
        {
            hoveredOn?.LongPress(x, y);
        }
        public static void Hover(int x, int y)
        {
            hoveredOn?.Hover(x, y);
        }
        public static void Key(KeyEvent keyEvent)
        {
            if (running[0] is Window w)
            {
                w.OnKey(keyEvent);
            }
        }
        public static string GetPriority(int id)
        {
            return running[id].priority switch
            {
                Priority.High => "High",
                Priority.Low => "Low",
                Priority.Realtime => "Realtime",
                Priority.None => "None",
                _ => "Unknown",
            };
        }
    }
    public static class WindowManager
    {
        internal enum Resizable
        {
            None,
            Scale,
            Full
        }
        internal static void Draw(Window w)
        {
            if (w.windowlock)
            {
                if (GUI.LongPress)
                {
                    w.StartX = w.StartXOld + (int)MouseManager.X;
                    w.StartY = w.StartYOld + (int)MouseManager.Y;
                    if (w.StartY < 0) { w.StartY = 0; }
                    else if (w.StartY > GUI.ScreenY - 60) { w.StartY = (int)GUI.ScreenY - 60; }
                    if (w.StartX < 0) { w.StartX = 0; }
                    else if (w.StartX + w.x > GUI.ScreenX) { w.StartX = (int)GUI.ScreenX - w.x; }
                }
                else { w.windowlock = false; w.RefreshBorder(); w.OnMoved?.Invoke(); }
            }
            GUI.canvas.DrawImage(w.borderCanvas, w.StartX, w.StartY);

            if (w.resizable == Resizable.Scale)
            {
                int[] canvas = w.appCanvas.rawData;
                Bitmap res = new((uint)w.x, (uint)w.y, GUI.DisplayMode.ColorDepth)
                { rawData = PostProcess.ResizeBitmap(canvas, w.appCanvas.Width, w.appCanvas.Height, (uint)w.x, (uint)w.y) };
                //if (res == null || res.Width != w.x || res.Height != w.y) { }
                GUI.canvas.DrawImage(res, w.StartX, w.StartY + 30);
            }
            else { GUI.canvas.DrawImage(w.appCanvas, w.StartX, w.StartY + 30); }
        }
        public static void FocusAtWindow(int id)
        {
            if (ProcessManager.running[id] is Window w2)
            {
                for (int i = ProcessManager.running.Count - 1; i > 0; i--)
                {
                    if (i <= id) { ProcessManager.running[i] = ProcessManager.running[i - 1]; }

                    if (ProcessManager.running[i] is Window w) Font.DrawString(w.name, Color.Gray.ToArgb(), 36, 10, w.borderCanvas.rawData, w.x);
                }
                ProcessManager.running[0] = w2;
                Font.DrawString(w2.name, Color.White.ToArgb(), 36, 10, w2.borderCanvas.rawData, w2.x);
                w2.Unminimize();
            }
        }
    }
    internal class TaskManager : Window
    {
        internal TaskManager() : base("Process Manager", 400, 400, new Bitmap(Resources.TaskmngIcon), ProcessManager.Priority.High) { OnClicked = Clicked; OnKeyPressed = Key; }
        private int selY = -1;
        private int list = -1;
        internal override int Start()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            return 0;
        }
        internal override void Update()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            DrawFilledRectangle(GUI.SystemPen.ValueARGB, x - 100, y - 50, 90, 20);
            DrawStringAlpha("End task", Color.Gray.ToArgb(), x - 90, y - 45);
            DrawHorizontalLine(Color.White.ToArgb(), 5, 10, x - 10);
            DrawString(" Task ", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, 0, 5); DrawString(" RAM ", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, x - 160, 5); DrawString(" Priority ", Color.White.ToArgb(), GUI.DarkGrayPen.ValueARGB, x - 80, 5);
            byte i = 0;
            if (selY != -1)
            {
                if (ProcessManager.running.Count < list) { selY--; list--; }
                else if (ProcessManager.running.Count > list) { selY++; list++; }
            }
            foreach (var task in ProcessManager.running)
            {
                i++;

                if (selY == i - 1)
                {
                    DrawFilledRectangle(Color.Gray.ToArgb(), 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                    DrawStringAlpha("End task", Color.White.ToArgb(), x - 90, y - 45);
                }
                if (i < 17) { Print(task, i); }
            }
            int ram = (int)(NclearOS2.Sysinfo.UsedRAM / NclearOS2.Sysinfo.InstalledRAM * 100);
            DrawFilledRectangle(Color.DarkGray.ToArgb(), 0, y - 20, x, 20);
            if (ram > 100) { ram = 100; }
            DrawFilledRectangle(GUI.SystemPen.ValueARGB, 0, y - 20, ram * (x / 100), 20);
            DrawStringAlpha("RAM: " + ram + "%", Color.White.ToArgb(), 10, y - 15);
        }

        private void Print(Process task, int i)
        {
            DrawStringAlpha(task.name, Color.White.ToArgb(), 10, i * 20 + 10); DrawStringAlpha(FormatBytes(task.usageRAM), Color.White.ToArgb(), x - 150, i * 20 + 10); DrawStringAlpha(ProcessManager.GetPriority(task.ID), Color.White.ToArgb(), x - 70, i * 20 + 10);
        }
        public static string FormatBytes(uint bytes)
        {
            if (bytes == 0) return "0 B";

            string[] units = { "B", "KB", "MB", "GB" };
            int i = 0;
            float value = bytes;

            while (value >= 1024 && i < units.Length - 1)
            {
                value /= 1024;
                i++;
            }

            string formatString = value < 10 ? "0.0" : "0";
            return $"{value.ToString(formatString)} {units[i]}";
        }

        private void Clicked(int x, int y)
        {
            if (y > 30 && y < ProcessManager.running.Count * 20 + 29 && y < this.y - 60)
            {
                if (selY == (y - 30) / 20)
                {
                    if (ProcessManager.running[selY] is Window) { WindowManager.FocusAtWindow(selY); }
                }
                else if (selY != -1)
                {
                    DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                    Print(ProcessManager.running[selY], selY + 1);
                }
                selY = (y - 30) / 20;
                list = ProcessManager.running.Count;
                DrawFilledRectangle(Color.Gray.ToArgb(), 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                Print(ProcessManager.running[selY], selY + 1);
                DrawStringAlpha("End task", Color.White.ToArgb(), this.x - 90, this.y - 45);
            }
            else if (selY != -1)
            {
                if (x > this.x - 100 && x < this.x - 10 && y > this.y - 50 && y < this.y - 30) { ProcessManager.RemoveAt(selY, true); selY = -1; return; }
                DrawFilledRectangle(GUI.DarkGrayPen.ValueARGB, 10, selY * 20 + 29, this.x - 20, Font.fontY + 2);
                Print(ProcessManager.running[selY], selY + 1); selY = -1;
            }
        }
        private void Key(KeyEvent key)
        {
            if (selY != -1 && key.Key == ConsoleKeyEx.Delete) { ProcessManager.RemoveAt(selY); selY = -1; }
        }
    }
}