using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace NclearOS2
{
    public class Menu
    {
        public static bool Opened;
        public static void Update()
        {
            Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, 0, (int)Kernel.screenY - 30, (int)Kernel.screenX, 30);
            Kernel.canvas.DrawString(Date.CurrentTime(true), Kernel.font, Kernel.WhitePen, 1200, 700);
            if (MouseManager.Y > Kernel.screenY - 30 && MouseManager.X < 34)
            {
                if (Kernel.Pressed)
                {
                    Kernel.canvas.DrawImageAlpha(Kernel.start2, 10, (int)Kernel.screenY - 28);
                    Opened = !Opened;
                }
                else
                {
                    Kernel.canvas.DrawImageAlpha(Kernel.start2, 10, (int)Kernel.screenY - 25);
                }
            }
            else
            {
                Kernel.canvas.DrawImageAlpha(Kernel.start, 10, (int)Kernel.screenY - 28);
            }
            if (Opened)
            {
                Kernel.canvas.DrawFilledRectangle(Kernel.SystemPen, 0, (int)Kernel.screenY - 340, 250, 310);
                Kernel.canvas.DrawImageAlpha(Kernel.settings, 10, (int)Kernel.screenY - 330);
                Kernel.canvas.DrawString("Settings", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 325);

                Kernel.canvas.DrawImage(Kernel.notepad, 10, (int)Kernel.screenY - 300);
                Kernel.canvas.DrawString("Notepad", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 295);

                Kernel.canvas.DrawImage(Kernel.console, 10, (int)Kernel.screenY - 270);
                Kernel.canvas.DrawString("Console", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 265);

                Kernel.canvas.DrawImageAlpha(Kernel.filesicon, 10, (int)Kernel.screenY - 240);
                Kernel.canvas.DrawString("Files", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 235);

                Kernel.canvas.DrawImageAlpha(Kernel.sysinfo, 10, (int)Kernel.screenY - 210);
                Kernel.canvas.DrawString("System Info", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 205);

                Kernel.canvas.DrawImageAlpha(Kernel.lockicon, 10, (int)Kernel.screenY - 120);
                Kernel.canvas.DrawString("Lock", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 115);

                Kernel.canvas.DrawImageAlpha(Kernel.reboot, 10, (int)Kernel.screenY - 90);
                Kernel.canvas.DrawString("Restart", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 85);

                Kernel.canvas.DrawImageAlpha(Kernel.shutdown, 10, (int)Kernel.screenY - 60);
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
                            Process.Reset();
                            Process.settings = true;
                            Window.Init("Settings", 300, 300, Kernel.settings, true);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 270)
                        {
                            Process.Reset();
                            Process.notepad = true;
                            Window.Init("Notepad", (int)(Kernel.screenX - 150), (int)(Kernel.screenY - 150), Kernel.notepad, true);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 240)
                        {
                            Process.Reset();
                            Process.console = true;
                            Window.Init("Console", (int)(Kernel.screenX - 250), (int)(Kernel.screenY - 150), Kernel.console, true);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 210)
                        {
                            Process.Reset();
                            Process.files = true;
                            Window.Init("Files", (int)(Kernel.screenX - 150), (int)(Kernel.screenY - 150), Kernel.filesicon, false);
                            Files.ListDisks();
                            Opened = false;
                            Kernel.Loading = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 180)
                        {
                            Process.Reset();
                            Process.sysinfo = true;
                            Window.Init("System Info", 500, 200, Kernel.sysinfo, true);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 90 && MouseManager.Y > (int)Kernel.screenY - 120)
                        {
                            Opened = false;
                            Kernel.Lock = true;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 60 && MouseManager.Y > (int)Kernel.screenY - 90)
                        {
                            Opened = false;
                            Kernel.ShutdownPC(true);
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 30 && MouseManager.Y > (int)Kernel.screenY - 60)
                        {
                            Opened = false;
                            Kernel.ShutdownPC(false);
                        }
                        else { }
                    }
                }
            }
        }
    }
}