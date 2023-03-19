using Cosmos.System;
using Cosmos.System.Graphics;

namespace NclearOS2
{
    public class Menu
    {
        public static bool Opened;
        public static bool dClick;
        public static void Update()
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, 0, (int)Kernel.screenY - 30, (int)Kernel.screenX, 30);
            Kernel.canvas.DrawString(Date.CurrentTime(true), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX-70, (int)Kernel.screenY - 20);
            for (int i = ProcessManager.running.Count; i > 0; i--)
            {
                Kernel.canvas.DrawImageAlpha(ProcessManager.running[i - 1].icon, (i-1)*40+50, (int)Kernel.screenY - 27);
            }
            if (MouseManager.Y > Kernel.screenY - 30)
            {
                if (MouseManager.X < 34)
                {
                    if (Kernel.Pressed)
                    {
                        Opened = !Opened;
                        dClick = false;
                        if (!Opened) { Kernel.canvas.DrawImageAlpha(Resources.start2, 5, (int)Kernel.screenY - 27); }
                    }
                    else if (!Opened)
                    { Kernel.canvas.DrawImageAlpha(Resources.start2, 5, (int)Kernel.screenY - 27); }
                }
                else if(MouseManager.X > (int)Kernel.screenX - 80)
                {
                    if (!Opened) { Kernel.canvas.DrawImageAlpha(Resources.start, 5, (int)Kernel.screenY - 27); }
                    if (Kernel.Pressed) { ProcessManager.Add(new Date()); }
                    else { Kernel.canvas.DrawString(Date.CurrentDate(true, false), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX - 200, (int)Kernel.screenY - 45); }
                }
                else if (Kernel.Pressed)
                {
                    if (!Opened) { Kernel.canvas.DrawImageAlpha(Resources.start, 5, (int)Kernel.screenY - 27); }
                    int clicked = ((int)MouseManager.X - 40) / 40;
                    if(clicked < ProcessManager.running.Count)
                    {
                        if (clicked == 0 && !ProcessManager.running[0].minimized)
                        {
                            Animation.hideWindow = (ProcessManager.running[0].name, (float)ProcessManager.running[0].StartWindowX, (float)ProcessManager.running[0].StartWindowY, (float)ProcessManager.running[0].x, ProcessManager.running[0].icon); ProcessManager.running[0].minimized = true;
                        }
                        else { ProcessManager.FocusAtWindow(clicked); }
                    }
                }
                else
                {
                    if (!Opened) { Kernel.canvas.DrawImageAlpha(Resources.start, 5, (int)Kernel.screenY - 27); }
                }
            }
            else if(!Opened)
            { Kernel.canvas.DrawImageAlpha(Resources.start, 5, (int)Kernel.screenY - 27); }
            if (Opened)
            {
                Kernel.canvas.DrawImageAlpha(Resources.start3, 5, (int)Kernel.screenY - 27);
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, 0, (int)Kernel.screenY - 340, 250, 310);
                Kernel.canvas.DrawImageAlpha(Resources.settings, 10, (int)Kernel.screenY - 330);
                Kernel.canvas.DrawString("Settings", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 325);

                Kernel.canvas.DrawImage(Resources.notepad, 10, (int)Kernel.screenY - 300);
                Kernel.canvas.DrawString("Notepad", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 295);

                Kernel.canvas.DrawImage(Resources.console, 10, (int)Kernel.screenY - 270);
                Kernel.canvas.DrawString("Console", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 265);

                Kernel.canvas.DrawImageAlpha(Resources.filesicon, 10, (int)Kernel.screenY - 240);
                Kernel.canvas.DrawString("Files", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 235);

                Kernel.canvas.DrawImageAlpha(Resources.sysinfo, 10, (int)Kernel.screenY - 210);
                Kernel.canvas.DrawString("System Info", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 205);

                Kernel.canvas.DrawImageAlpha(Resources.sysinfo, 10, (int)Kernel.screenY - 180);
                Kernel.canvas.DrawString("Task Manager", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 175);

                Kernel.canvas.DrawImageAlpha(Resources.lockicon, 10, (int)Kernel.screenY - 120);
                Kernel.canvas.DrawString("Lock", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 115);

                Kernel.canvas.DrawImageAlpha(Resources.reboot, 10, (int)Kernel.screenY - 90);
                Kernel.canvas.DrawString("Restart", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 85);

                Kernel.canvas.DrawImageAlpha(Resources.shutdown, 10, (int)Kernel.screenY - 60);
                Kernel.canvas.DrawString("Shutdown", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 55);
                if (Kernel.Pressed)
                {
                    if (MouseManager.Y < (int)Kernel.screenY - 340 | MouseManager.X > 250)
                    {
                        Opened = false;
                    }
                    else
                    {

                        if (MouseManager.Y < (int)Kernel.screenY - 300)
                        {
                            ProcessManager.Add(new Settings());
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 270)
                        {
                            ProcessManager.Add(new Notepad((int)(Kernel.screenX - 250), (int)(Kernel.screenY - 170)));
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 240)
                        {
                            ProcessManager.Add(new ConsoleApp((int)(Kernel.screenX - 250), (int)(Kernel.screenY - 170)));
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 210)
                        {
                            ProcessManager.Add(new Files((int)(Kernel.screenX - 250), (int)(Kernel.screenY - 170)));
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 180)
                        {
                            ProcessManager.Add(new Sysinfo());
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 150)
                        {
                            ProcessManager.Add(new TaskManager());
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 90 && MouseManager.Y > (int)Kernel.screenY - 120)
                        {
                            Opened = false;
                            Kernel.Lock = true;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 60 && MouseManager.Y > (int)Kernel.screenY - 90)
                        {
                            if (dClick) { Kernel.ShutdownPC(true); }
                            else { Toast.msg = "Press again to restart"; dClick = true; }
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 30 && MouseManager.Y > (int)Kernel.screenY - 60)
                        {
                            if (dClick) { Kernel.ShutdownPC(false); }
                            else { Toast.msg = "Press again to shutdown"; dClick = true; }
                        }
                    }
                }
            }
        }
    }
}