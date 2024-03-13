using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.HAL.BlockDevice;
using Cosmos.System;
using Cosmos.System.Audio.IO;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace NclearOS2.GUI
{
    public class GUI
    {
        public static Mode DisplayMode { get; private set; } = new(1280, 720, ColorDepth.ColorDepth32);
        public static int ScreenX { get { return DisplayMode.Columns; } }
        public static int ScreenY { get { return DisplayMode.Rows; } }

        public static short fps;
        private static short fps2;
        private static short frames;
        private static bool displayFPS = true;

        public static bool Lock = true;
        public static bool screenSaver;
        
        //private static bool OneClick;
        public static bool Pressed;
        public static bool LongPress;
        //private static bool StartOneClick;
        //public static bool StartClick;

        public static bool Loading;
        public static bool DisplayCursor;
        public static bool keyCursor = true;
        public static bool wasClicked;

        public static Canvas canvas;

        public static readonly Pen WhitePen = new(Color.White);
        public static readonly Pen GrayPen = new(Color.Gray);
        public static readonly Pen DarkPen = new(Color.Black);
        public static readonly Pen DarkGrayPen = new(Color.FromArgb(40, 40, 40));
        public static readonly Pen RedPen = new(Color.DarkRed);
        public static readonly Pen Red2Pen = new(Color.Red);
        public static readonly Pen GreenPen = new(Color.Green);
        public static readonly Pen YellowPen = new(Color.Goldenrod);
        public static readonly Pen BluePen = new(Color.SteelBlue);
        public static readonly Pen DarkBluePen = new(Color.MidnightBlue);
        public static Pen SystemPen = BluePen;

        public static PCScreenFont font;
        private static Menu menu;
        private static GlobalInput globalInput;
        internal static Process screenSaverProcess;

        public static bool blurEffects = true;
        public static bool animationEffects = true;

        public static string Init(string overrideRes = null, string customCanvas = "")
        {
            try
            {
                Lock = true;
                DisplayCursor = false;
                if (overrideRes != null) { DisplayMode = ResParse(overrideRes); }
                else if (Kernel.safeMode) { DisplayMode = new(800, 600, ColorDepth.ColorDepth32); }
                else if (Kernel.useDisks) { DisplayMode = Profiles.LoadSystem(); }
                SetRes(DisplayMode, true, true, customCanvas);
                //if (!Kernel.safeMode) { canvas.DrawImage(new Bitmap(Resources.Wallpaper), 0, 0); canvas.DrawFilledRectangle(DarkPen, ((int)(GUI.ScreenX - "Starting NclearOS".Length * 8) / 2) - 10, (int)(GUI.ScreenY - 12) / 2, "Starting NclearOS".Length * 8 + 20, 24); }
                //canvas.DrawString("NclearOS", font, new Pen(Color.LimeGreen), (int)(GUI.ScreenX - "NclearOS".Length * font.Width) / 2, (int)(GUI.ScreenY - font.Height * 2) / 2);
                canvas.DrawImageAlpha(new Bitmap(Resources.CursorLoad), ScreenX / 2, ScreenY / 2);
                canvas.Display();

                if (font == null)
                {
                    font = PCScreenFont.Default;
                    Resources.InitResources();
                    Font.Main();
                }

                if (!Kernel.safeMode)
                {
                    //Toast.Display("Loading user settings...");
                    //canvas.Display();
                    Profiles.LoadUser();
                    Settings.LoadWallpaper();
                    if(Sysinfo.InstalledRAM < 120) { blurEffects = false; }
                }
                else
                {
                    blurEffects = false;
                    animationEffects = false;
                    Bitmap wallpaper = new(1, 1, DisplayMode.ColorDepth);
                    MemoryOperations.Fill(wallpaper.rawData, Color.CadetBlue.ToArgb());
                    Images.RequestSystemWallpaperChange(wallpaper);
                }
                ApplyRes();
                globalInput = new();
                if (!Kernel.safeMode)
                {
                    screenSaverProcess = ProcessManager.Run(new ScreenSaverService());
                    ProcessManager.Run(new PerformanceWatchdog());
                    NclearOS2.Net.Start();
                }
                if (Kernel.Debug) { ProcessManager.Run(new InfoDisplay()); }
                DisplayCursor = true;
                return "OK";
            }
            catch (Exception e)
            {
                Kernel.GUIenabled = false;
                try { canvas.Disable(); } catch { }
                return e.Message;
            }
        }
        public static void Refresh()
        {
            if (fps2 != RTC.Second)
            {
                fps = frames;
                frames = 0;
                fps2 = RTC.Second;
            }
            frames++;

            if ((MouseManager.MouseState == MouseState.Left || MouseManager.MouseState == MouseState.Right || GlobalInput.clickAlternative))
            {
                LongPress = true;
                if (!screenSaver && !Lock && MouseManager.Y < ScreenY - 30) { ProcessManager.LongPress((int)MouseManager.X, (int)MouseManager.Y); }
                wasClicked = true;
            }
            else
            {
                Pressed = false;
                if (LongPress)
                {
                    Pressed = true;
                    LongPress = false;
                    if (!screenSaver && !Lock && MouseManager.Y < ScreenY-30)
                    {
                        if(!ProcessManager.Click((int)MouseManager.X, (int)MouseManager.Y)) { Desktop.Click((int)MouseManager.X, (int)MouseManager.Y); }
                    }
                }
            }

            if (screenSaver) { ScreenSaver.Update(); }
            else if (Lock) { LockScreen.Update(); }
            else
            {
                Desktop.Update();
                globalInput.Update();
                ProcessManager.Refresh();
                if (!screenSaver && !Lock && MouseManager.Y < ScreenY - 30) { ProcessManager.Hover((int)MouseManager.X, (int)MouseManager.Y); }
                Animation.Update();
                menu.Update();
            }

            Toast.Update();
            
            if (Kernel.Debug) { GUI.canvas.DrawImage(InfoDisplay.display, 0, 0); }
            
            if (DisplayCursor) { if (Loading) { canvas.DrawImageAlpha(Icons.cursorload, (int)MouseManager.X, (int)MouseManager.Y); } else { canvas.DrawImageAlpha(Icons.cursor, (int)MouseManager.X, (int)MouseManager.Y); } }

            if (displayFPS)
            {
                GUI.canvas.DrawFilledRectangle(DarkPen, (int)GUI.ScreenX - 14, 0, 14, 7);
                Font.DrawNumbers(Convert.ToString(fps), Color.Yellow, (int)GUI.ScreenX - 13, 1);
            }
            Heap.Collect();
            canvas.Display();
        }
        public static void Wait()
        {
            canvas.DrawImageAlpha(Icons.cursorload, (int)MouseManager.X, (int)MouseManager.Y);
            Toast.Update();
            canvas.Display();
        }
        public static bool ShutdownGUI(bool restart = false, bool disableCanvas = true, bool force = false) //to shutdown PC use Kernel.Shutdown() instead
        {
            Kernel.GUIenabled = true; //display Toast messages on GUI for now
            Toast.Force("Stopping processes...");
            string apps = ProcessManager.StopAll(force);
            if (apps != null)
            {
                Kernel.GUIenabled = true;
                Msg.Main("System", "Apps preventing shutdown:" + apps, Icons.warn, new Option[] { new("Cancel"), new("Force Shutdown",
                    new Action(() =>
                    {
                        if (disableCanvas) { ProcessManager.StopAll(true); Kernel.GUIenabled = false; }
                        else { Kernel.Shutdown(restart, 1); }
                    } ))
                });
                
                Toast.Msg = null;
                return false;
            }
            canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            if (Kernel.useDisks && !Kernel.safeMode)
            {
                Toast.Force("Saving user settings...");
                Profiles.Save();
                Thread.Sleep(300);
                canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            }
            if (disableCanvas) { canvas.Disable(); canvas = null; }
            else { Toast.Display(restart ? "Restarting..." : "Shutting down..."); GUI.canvas.Display(); }
            Toast.Msg = null;
            globalInput = null;
            menu = null;
            Kernel.GUIenabled = !disableCanvas;
            return true;
        }
        public static void SetCanvas(Mode mode, string canvasType = "")
        {
            canvas = canvasType switch
            {
                "VBE" => new VBECanvas(mode),
                "SVGAII" => new SVGAIICanvas(mode),
                "VGA" => new VGACanvas(mode),
                _ => FullScreenCanvas.GetFullScreenCanvas(mode),
            };
        }
        public static void SetCanvas(string canvasType = "")
        {
            canvas = canvasType switch
            {
                "VBE" => new VBECanvas(),
                "SVGAII" => new SVGAIICanvas(),
                "VGA" => new VGACanvas(),
                _ => FullScreenCanvas.GetFullScreenCanvas(),
            };
        }
        public static string SetRes(Mode mode, bool fast = false, bool boot = false, string canvasType = "")
        {
            try { SetCanvas(mode, canvasType); }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(canvasType)) { canvasType += " Canvas: "; }
                if (!boot) { SetRes(DisplayMode, true); } //rollback to previous res
                throw new Exception(canvasType + "Resolution " + mode.Columns + "x" + mode.Rows + " is not available; " + e);
            }
            DisplayMode = mode;
            MouseManager.ScreenWidth = (uint)mode.Columns - 5;
            MouseManager.ScreenHeight = (uint)mode.Rows;
            MouseManager.X = MouseManager.ScreenWidth / 2;
            MouseManager.Y = MouseManager.ScreenHeight / 2;
            if (!fast)
            {
                ApplyRes();
            }
            Kernel.GUIenabled = true;
            return "Successfully changed resolution to " + mode.Columns + "x" + mode.Rows; // mode.ColorDepth.ToString() not implemented
        }
        public static void ApplyRes()
        {
            if(GUI.ScreenX * GUI.ScreenY > Sysinfo.AvailableRAM * 31000) { throw new Exception("Not enough system memory to complete desktop environment initialization!");  }
            
            Images.wallpaper = new((uint)GUI.ScreenX, (uint)GUI.ScreenY, DisplayMode.ColorDepth)
            { rawData = PostProcess.ResizeBitmap(Images.systemWallpaper, (uint)GUI.ScreenX, (uint)GUI.ScreenY) };

            Images.wallpaperBlur = new((uint)GUI.ScreenX, (uint)GUI.ScreenY, DisplayMode.ColorDepth);
            if (blurEffects)
            { Images.wallpaperBlur.rawData = PostProcess.ResizeBitmap(PostProcess.DarkenBitmap(PostProcess.ApplyBlur(PostProcess.ResizeBitmap(Images.systemWallpaper, 640, 360), 20), 0.5f), 640, 360, (uint)GUI.ScreenX, (uint)GUI.ScreenY); }
            else
            { Images.wallpaperBlur.rawData = PostProcess.DarkenBitmap(PostProcess.ResizeBitmap(Images.systemWallpaper, (uint)GUI.ScreenX, (uint)GUI.ScreenY), 0.5f); }

            menu = new();
            for (int i = 0; i < ProcessManager.running.Count; i++)
            {
                if (ProcessManager.running[i] is Window w)
                { w.RefreshBorder(); }
            }
            NotificationSystem.RefreshBG();
        }
        public static Mode ResParse(string res)
        {
            if (res.Contains('x'))
            {
                ColorDepth colorss;
                uint x;
                uint y;
                if (res.Contains('@'))
                {
                    string[] split = res.Split('@');
                    colorss = GetColorDepth(split[1]);
                    x = Convert.ToUInt32(split[0].Split('x')[0]);
                    y = Convert.ToUInt32(split[0].Split('x')[1]);
                }
                else
                {
                    x = Convert.ToUInt32(res.Split('x')[0]);
                    y = Convert.ToUInt32(res.Split('x')[1]);
                    colorss = DisplayMode.ColorDepth;
                }
                return new Mode((int)x, (int)y, colorss);
            }
            else
            {
                throw new Exception("'x' character expected");
            }
        }
        public static ColorDepth GetColorDepth(string str)
        {
            return str switch
            {
                "32" => ColorDepth.ColorDepth32,
                "24" => ColorDepth.ColorDepth24,
                "16" => ColorDepth.ColorDepth16,
                "8" => ColorDepth.ColorDepth8,
                "4" => ColorDepth.ColorDepth4,
                _ => ColorDepth.ColorDepth32,
            };
        }
    }
    public class PostProcess
    {
        public static int[] DarkenBitmap(int[] data, float factor)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (255 << 24) | ((byte)(((data[i] >> 16) & 0xFF) * factor) << 16) | ((byte)(((data[i] >> 8) & 0xFF) * factor) << 8) | (byte)((data[i] & 0xFF) * factor);
            }
            return data;
        }

        public static int[] ResizeBitmap(Bitmap bmp, uint nX, uint nY, bool check = false)
        {
            if(check && bmp.Width == nX && bmp.Height == nY) { return bmp.rawData; }
            int[] result = new int[nX * nY];
            if (bmp.Width == nX && bmp.Height == nY) { result = bmp.rawData; return result; }

            for (int i = 0; i < nX; i++)
            {
                for (int j = 0; j < nY; j++)
                {
                    result[i + j * nX] = bmp.rawData[(i * bmp.Width / nX) + (j * bmp.Height / nY) * bmp.Width];
                }
            }
            return result;
        }
        public static int[] ResizeBitmap(int[] data, uint x, uint y, uint nX, uint nY)
        {
            int[] result = new int[nX * nY];
            if (x == nX && y == nY) { result = data; return result; }

            for (int i = 0; i < nX; i++)
            {
                for (int j = 0; j < nY; j++)
                {
                    result[i + j * nX] = data[(i * x / nX) + (j * y / nY) * x];
                }
            }
            return result;
        }
        public static Bitmap CropBitmap(Bitmap bitmap, int x, int y, int width, int height)
        {
            Bitmap croppedBitmap = new((uint)width, (uint)height, GUI.DisplayMode.ColorDepth);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    croppedBitmap.rawData[row * width + col] = bitmap.rawData[(y + row) * bitmap.Width + (x + col)];
                }
            }
            return croppedBitmap;
        }

        public static int[] ApplyBlur(int[] originalData, int blurRadius, uint width = 640, uint height = 360)
        {
            int[] blurredData = new int[width * height];
            double[] kernel = new double[blurRadius * 2 + 1];
            double sum = 0;
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] = Math.Exp(-(Math.Pow(i - blurRadius, 2) / (2 * Math.Pow(blurRadius / 3.0, 2))));
                sum += kernel[i];
            }
            for (int i = 0; i < kernel.Length; i++) { kernel[i] /= sum; }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double r = 0, g = 0, b = 0;
                    for (int i = -blurRadius; i <= blurRadius; i++)
                    {
                        int idx = x + i;
                        if (idx < 0 || idx >= width)
                        {
                            idx = x;
                        }
                        int pixel = originalData[y * width + idx];
                        double weight = kernel[i + blurRadius];
                        r += ((pixel >> 16) & 0xFF) * weight;
                        g += ((pixel >> 8) & 0xFF) * weight;
                        b += (pixel & 0xFF) * weight;
                    }
                    blurredData[y * width + x] = ((int)r << 16) | ((int)g << 8) | (int)b;
                }
            }

            return blurredData;
        }
        public static int[] ApplyAlpha(int[] data, float alphaFactor)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (Math.Max(0, Math.Min(255, (int)(((data[i] >> 24) & 0xFF) * alphaFactor))) << 24) | (((data[i] >> 16) & 0xFF) << 16) | (((data[i] >> 8) & 0xFF) << 8) | (data[i] & 0xFF);
            }
            return data;
        }
    }
    public class Font
    {
        public static int fontX = GUI.font.Width;
        public static int fontY = GUI.font.Height;
        public static Dictionary<char, bool[]> charCache = new();
        public static Dictionary<char, bool[]> numCache = new();
        public static void Main()
        {
            foreach (char c in "?ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[{]}\\|;:'\",<.>/`~")
            {
                charCache[c] = CreateCharCache(c);
            }

            numCache.Add('0', new bool[] {
    true, true, true,
    true, false, true,
    true, false, true,
    true, false, true,
    true, true, true
});

            numCache.Add('1', new bool[] {
    false, true, false,
    true, true, false,
    false, true, false,
    false, true, false,
    true, true, true
});

            numCache.Add('2', new bool[] {
    true, true, true,
    false, false, true,
    false, true, false,
    true, false, false,
    true, true, true
});

            numCache.Add('3', new bool[] {
    true, true, true,
    false, false, true,
    false, true, false,
    false, false, true,
    true, true, true
});

            numCache.Add('4', new bool[] {
    true, false, true,
    true, false, true,
    true, true, true,
    false, false, true,
    false, false, true
});

            numCache.Add('5', new bool[] {
    true, true, true,
    true, false, false,
    true, true, true,
    false, false, true,
    true, true, true
});

            numCache.Add('6', new bool[] {
    true, true, true,
    true, false, false,
    true, true, true,
    true, false, true,
    true, true, true
});

            numCache.Add('7', new bool[] {
    true, true, true,
    false, false, true,
    false, false, true,
    false, true, false,
    false, true, false
});

            numCache.Add('8', new bool[] {
    true, true, true,
    true, false, true,
    true, true, true,
    true, false, true,
    true, true, true
});

            numCache.Add('9', new bool[] {
    true, true, true,
    true, false, true,
    true, true, true,
    false, false, true,
    true, true, true
});

        }
        public static void DrawChar(char c, Color color, int x, int y)
        {
            if (c == ' ') { return; }
            bool[] cache = charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    if (cache[py * fontX + px])
                    {
                        GUI.canvas.DrawPoint(new Pen(color), x + px, y + py);
                    }
                }
            }
        }

        public static void DrawNum(char c, Color color, int x, int y)
        {
            if (!char.IsDigit(c)) { return; }
            bool[] cache = numCache[c];
            for (int py = 0; py < 5; py++)
            {
                for (int px = 0; px < 3; px++)
                {
                    if (cache[py * 3 + px])
                    {
                        GUI.canvas.DrawPoint(new Pen(color), x + px, y + py);
                    }
                }
            }
        }
        public static bool[] CreateCharCache(char c)
        {
            bool[] cache = new bool[fontX * fontY];
            GUI.canvas.Clear();
            GUI.canvas.DrawChar(c, GUI.font, GUI.WhitePen, 0, 0);
            for (int y = 0; y < fontY; y++)
            {
                for (int x = 0; x < fontX; x++)
                {
                    cache[y * fontX + x] = GUI.canvas.GetPointColor(x, y).R != 0;
                }
            }
            return cache;
        }
        public static void DrawString(string str, Color color, int x, int y)
        {
            foreach (char c in str)
            {
                DrawChar(c, color, x, y);
                x += fontX;
            }
        }

        public static void DrawNumbers(string str, Color color, int x, int y)
        {
            foreach (char c in str)
            {
                DrawNum(c, color, x, y);
                x += 4;
            }
        }

        public static bool DrawChar(char c, int color, int[] canvas, int canvasWidth, int x2, int y2)
        {
            int fontY = Font.fontY;
            int fontX = Font.fontX;
            if (c == ' ')
            {
                return x2 > canvasWidth - fontX * 2;
            }
            bool[] cache = charCache[c];
            for (int py = 0; py < fontY; py++)
            {
                for (int px = 0; px < fontX; px++)
                {
                    if (cache[py * fontX + px])
                    {
                        canvas[(y2 + py) * canvasWidth + (x2 + px)] = color;
                    }
                }
            }
            return x2 > canvasWidth - fontX * 2;
        }
        public static void DrawString(string str, int color, int x2, int y2, int[] bitmap, int canvasWidth)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                //if (y2 + Font.fontY > y) { return; }
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                if(DrawChar(c, color, bitmap, canvasWidth, x2, y2)) { y2 += 14; x2 = ogX; continue; }
                x2 += Font.fontX;
            }
        }
        public static void DrawImageAlpha(Bitmap image, int x2, int y2, int[] canvas, int canvasWidth)
        {
            for (int py = 0; py < image.Height; py++)
            {
                for (int px = 0; px < image.Width; px++)
                {
                    int temp = image.rawData[py * image.Width + px];
                    if (temp == 0) { continue; }
                    canvas[(int)((y2 + py) * canvasWidth + (x2 + px))] = temp;
                }
            }
        }
    }
}