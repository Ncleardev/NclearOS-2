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
            Kernel.canvas.DrawString(Date.CurrentTime(true), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX-70, (int)Kernel.screenY - 20);
            if (MouseManager.Y > Kernel.screenY - 30)
            {
                if(MouseManager.X < 34)
                {
                    if (Kernel.Pressed)
                    { Opened = !Opened; }
                    
                }
                else if(MouseManager.X > (int)Kernel.screenX - 80)
                {
                    if (Kernel.Pressed)
                    { Process.Run(Process.Apps.date);
                    }
                    else { Kernel.canvas.DrawString(Date.CurrentDate(true, false), Kernel.font, Kernel.WhitePen, (int)Kernel.screenX - 200, (int)Kernel.screenY - 40); }
                }
            }
            if (Opened)
            {
                Kernel.canvas.DrawImageAlpha(Kernel.start2, 5, (int)Kernel.screenY - 27);
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

                //Kernel.canvas.DrawImageAlpha(Kernel.program, 10, (int)Kernel.screenY - 180);
                //Kernel.canvas.DrawString("Date", Kernel.font, Kernel.WhitePen, 40, (int)Kernel.screenY - 175);

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
                            Process.Run(Process.Apps.settings);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 270)
                        {
                            Process.Run(Process.Apps.notepad);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 240)
                        {
                            Process.Run(Process.Apps.console);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 210)
                        {
                            Process.Run(Process.Apps.files);
                            Opened = false;
                        }
                        else if (MouseManager.Y < (int)Kernel.screenY - 180)
                        {
                            Process.Run(Process.Apps.sysinfo);
                            Opened = false;
                        }
                        //else if (MouseManager.Y < (int)Kernel.screenY - 150){ Process.Run("Date");}
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
            else { Kernel.canvas.DrawImageAlpha(Kernel.start, 5, (int)Kernel.screenY - 27); }
            if (Window.display) { Kernel.canvas.DrawImageAlpha(Window.icon, Window.StartWindowX + 5, Window.StartWindowY + 3); } //to make sure icon is visible over taskbar (minimized state)
        }
    }
}