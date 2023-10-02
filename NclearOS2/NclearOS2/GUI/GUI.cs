using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace NclearOS2.GUI
{
    public class GUI
    {
        public static Mode displayMode { get; private set; } = new(1280, 720, ColorDepth.ColorDepth32);
        public static int screenX { get { return displayMode.Columns; } }
        public static int screenY { get { return displayMode.Rows; } }

        public static int fps;
        private static int fps2;
        private static int frames;

        public static bool wasClicked;

        public static bool Lock = true;
        public static bool screenSaver;
        public static bool debug;
        public static byte ExecuteError;

        public static bool Pressed;
        private static bool OneClick;
        private static bool StartOneClick;
        public static bool StartClick;
        public static bool LongPress;
        public static bool Loading;
        public static bool HideCursor;

        public static VBECanvas canvas;

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
        public static int wallpapernum;

        public static void Init()
        {
            NclearOS2.Sysinfo.CPUname = "Unknown";
            if (!Kernel.GUIenabled)
            {
                if (Kernel.safeMode) { displayMode = new(800, 600, ColorDepth.ColorDepth32); }
                //else if (Kernel.useDisks) { displayMode = Profiles.LoadSystem(); }
                else { NclearOS2.Sysinfo.CPUname = CPU.GetCPUBrandString(); }
                try { SetRes(displayMode, true); }
                catch (Exception e) { TextMode.BootMenu(e.Message); }
            }
            font = PCScreenFont.Default;
            //if (!Kernel.safeMode) { canvas.DrawImage(new Bitmap(Resources.Wallpaper), 0, 0); canvas.DrawFilledRectangle(DarkPen, ((int)(GUI.screenX - "Starting NclearOS".Length * 8) / 2) - 10, (int)(GUI.screenY - 12) / 2, "Starting NclearOS".Length * 8 + 20, 24); }
            canvas.DrawString("Starting NclearOS", font, GUI.WhitePen, (int)(GUI.screenX - "Starting NclearOS".Length * 8) / 2, (int)(GUI.screenY - 1) / 2);
            canvas.Display();

            Resources.InitResources();
            Font.Main();

            if (!Kernel.safeMode)
            {
                if (Kernel.useDisks)
                {
                    //canvas.DrawImage(new Bitmap(Resources.WallpaperLock), 0, 0);
                    //Toast.Display("Loading user settings...");
                    //canvas.Display();
                    switch (0)
                    {
                        case 1:
                            ApplyRes(new Bitmap(Resources.WallpaperOld)); break;
                        case 2:
                            ApplyRes(new Bitmap(Resources.WallpaperLock)); break;
                        case 3:
                            ApplyRes(new Bitmap(Resources.WallpaperOrigami)); break;
                        case 4:
                            ApplyRes(new Bitmap(Resources.Wallpaper2005s)); break;
                        case 5:
                            ApplyRes(new Bitmap(Resources.WallpaperCosmos)); break;
                        default:
                            ApplyRes(new Bitmap(Resources.Wallpaper)); break;
                    }
                }
                else
                {
                    //Msg.Main("Information", "No valid FAT32 partitions found; File system is now disabled.", Icons.info);
                    ApplyRes(new Bitmap(Resources.Wallpaper));
                }
                ProcessManager.Run(new ScreenSaverService());
                ProcessManager.Run(new Net());
            }
            else
            {
                Images.wallpaper = new Bitmap(800, 600, ColorDepth.ColorDepth32);
                Images.wallpaperBlur = new Bitmap(800, 600, ColorDepth.ColorDepth32);
                menu = new();
            }
            ProcessManager.Run(new GlobalInput());
            ProcessManager.Run(new PerformanceWatchdog());
            MouseManager.X = (uint)GUI.screenX;
            MouseManager.Y = (uint)GUI.screenY;
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

            switch (MouseManager.MouseState)
            {
                case MouseState.Left or MouseState.Right:
                    StartClick = false;
                    if (!StartOneClick) { StartOneClick = true; StartClick = true; }
                    OneClick = true;
                    LongPress = true;
                    wasClicked = true;
                    break;
                case MouseState.None:
                    Pressed = false;
                    if (OneClick)
                    {
                        Pressed = true;
                        OneClick = false;
                        if(!screenSaver && !Lock) { ProcessManager.Click((int)MouseManager.X, (int)MouseManager.Y); }
                    }
                    StartOneClick = false;
                    StartClick = false;
                    LongPress = false;
                    break;
            }

            if (screenSaver) { ScreenSaver.Update(); }
            else
            {
                if (Lock) { LockScreen.Update(); }
                else
                {
                    if (!Kernel.safeMode) { canvas.DrawImage(Images.wallpaper, 0, 0); } else { canvas.Clear(Color.CadetBlue); }
                    ProcessManager.Refresh();
                    menu.Update();
                }
            }
            if (debug)
            {
                if (ExecuteError == 1) { ExecuteError = 0; throw new Exception("Manual crash"); }
                else if (ExecuteError == 2) { throw new Exception("Manual crash"); }
                canvas.DrawString(NclearOS2.Sysinfo.Ram(), font, WhitePen, 10, 0);
            }
            
            Toast.Update();
            if (!HideCursor) { if (Loading) { canvas.DrawImageAlpha(Icons.cursorload, (int)MouseManager.X, (int)MouseManager.Y); } else { canvas.DrawImageAlpha(Icons.cursor, (int)MouseManager.X, (int)MouseManager.Y); } }
            
            GUI.canvas.DrawFilledRectangle(DarkPen, (int)GUI.screenX - 14, 0, 14, 7);
            Font.DrawNumbers(Convert.ToString(fps), Color.Yellow, (int)GUI.screenX - 13, 1);
            
            Heap.Collect();
            canvas.Display();
        }
        public static void Wait()
        {
            canvas.DrawImageAlpha(Icons.cursorload, (int)MouseManager.X, (int)MouseManager.Y);
            Toast.Update();
            canvas.Display();
        }
        public static void ShutdownGUI(bool restart = false) //to shutdown pc use Kernel.Shutdown() instead
        {
            Toast.Force("Stopping processes...");
            string str = ProcessManager.StopAll();
            if (str != null) //to do
            {
                Toast.Force(str);
                Thread.Sleep(1000);
            }
            canvas.DrawImage(Images.wallpaperBlur, 0, 0);
            //if (Kernel.useDisks && !Kernel.safeMode)
            //{
            //    Toast.Display("Saving user settings...");
            //    canvas.Display();
            //    Profiles.Save();
            //    Thread.Sleep(500);
            //    canvas.DrawImage(Images.wallpaperLock, 0, 0);
            //}
            Toast.Display(restart ? "Restarting..." : "Shutting down...");
            canvas.Display();
            Thread.Sleep(500);
            Kernel.GUIenabled = false;
        }
        public static string SetRes(Mode mode, bool fast = false)
        {
            try
            {
                canvas = (VBECanvas)FullScreenCanvas.GetFullScreenCanvas(mode);
                displayMode = mode;
                MouseManager.ScreenWidth = (uint)mode.Columns - 1;
                MouseManager.ScreenHeight = (uint)mode.Rows;
            }
            catch (Exception e)
            {
                throw new Exception("Error: Resolution " + mode.Columns + "x" + mode.Rows + " is not available; " + e);
            }
            if (!fast)
            {
                ApplyRes(new Bitmap(Resources.Wallpaper));
            }
            Kernel.GUIenabled = true;
            return "Successfully changed resolution to " + mode.Columns + "x" + mode.Rows; // mode.ColorDepth.ToString() not implemented
        }
        public static void ApplyRes(Bitmap wallpaper)
        {
            Images.wallpaper = PostProcess.ResizeBitmap(wallpaper, (uint)GUI.screenX, (uint)GUI.screenY);
            Images.wallpaperBlur = PostProcess.ResizeBitmap(PostProcess.DarkenBitmap(PostProcess.ApplyBlur(PostProcess.ResizeBitmap(wallpaper, 640, 360), 20), 0.5f), (uint)GUI.screenX, (uint)GUI.screenY);
            menu = new();
            for (int i = 0; i < ProcessManager.running.Count; i++)
            {
                if (ProcessManager.running[i] is Window w)
                { w.RefreshBorder(); }
            }
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
                    colorss = displayMode.ColorDepth;
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
    public class Buffer
    {
        public static Color[] GetBuffer(int StartX, int StartY, int x2, int y2)
        {
            Color[] colors = new Color[x2 * y2];
            for (int y = 0; y < y2; y++)
            {
                for (int x = 0; x < x2; x++)
                {
                    colors[y * x2 + x] = GUI.canvas.GetPointColor((int)(StartX + x), (int)StartY + y);
                }
            }
            return colors;
        }
        public static Bitmap GetBitmap(int StartX, int StartY, int x2, int y2)
        {
            int[] colors = new int[x2 * y2];
            for (int y = 0; y < y2; y++)
            {
                for (int x = 0; x < x2; x++)
                {
                    colors[y * x2 + x] = GUI.canvas.GetPointColor((int)(StartX + x), (int)StartY + y).ToArgb();
                }
            }
            return new Bitmap("");
        }
        public static void DrawBuffer(Color[] colors, int StartX, int StartY, int x2, int y2)
        {
            for (int y = 0; y < y2; y++)
            {
                for (int x = 0; x < x2; x++)
                {
                    GUI.canvas.DrawPoint(new Pen(colors[y * x2 + x]), StartX + x, StartY + y);
                }
            }
        }
        public static Bitmap ConvertToBitmap(Color[] colors, int x, int y)
        {
            // create a new byte array with enough space for all the colors
            byte[] byteArray = new byte[colors.Length * 4];

            // loop through the colors and copy each color to the byte array
            for (int i = 0; i < colors.Length; i++)
            {
                byteArray[i * 4 + 0] = colors[i].B; // blue component
                byteArray[i * 4 + 1] = colors[i].G; // green component
                byteArray[i * 4 + 2] = colors[i].R; // red component
                byteArray[i * 4 + 3] = colors[i].A; // alpha component
            }
            // create a new Bitmap object from the byte array
            return new Bitmap((uint)x, (uint)y, byteArray, GUI.displayMode.ColorDepth);
        }
    }
    public class PostProcess
    {
        public static Bitmap DarkenBitmap(Bitmap bitmap, float factor)
        {
            int[] originalData = bitmap.rawData;
            int dataSize = originalData.Length;

            // Iterate over each pixel and darken its brightness
            for (int i = 0; i < dataSize; i++)
            {
                int argb = originalData[i];

                // Extract the individual color components
                byte alpha = (byte)((argb >> 24) & 0xFF);
                byte red = (byte)((argb >> 16) & 0xFF);
                byte green = (byte)((argb >> 8) & 0xFF);
                byte blue = (byte)(argb & 0xFF);

                // Darken the color components by applying the specified factor
                red = (byte)(red * factor);
                green = (byte)(green * factor);
                blue = (byte)(blue * factor);

                // Combine the modified color components back into an ARGB value
                int modifiedArgb = (alpha << 24) | (red << 16) | (green << 8) | blue;

                // Update the pixel in the bitmap with the modified color
                originalData[i] = modifiedArgb;
            }

            // Create a new bitmap with the modified data
            bitmap = new Bitmap(bitmap.Width, bitmap.Height, GUI.displayMode.ColorDepth);
            bitmap.rawData = originalData;
            return bitmap;
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, uint nX, uint nY)
        {
            if (bmp.Width == nX && bmp.Height == nY)
            {
                return bmp;
            }
            Bitmap resized = new Bitmap(nX, nY, GUI.displayMode.ColorDepth);
            int[] origIndices = new int[nX * nY];

            for (int i = 0; i < nX; i++)
            {
                for (int j = 0; j < nY; j++)
                {
                    int origX = (int)(i * bmp.Width / nX);
                    int origY = (int)(j * bmp.Height / nY);
                    int origIndex = (int)(origX + origY * bmp.Width);
                    int resizedIndex = (int)(i + j * nX);
                    origIndices[resizedIndex] = origIndex;
                }
            }

            for (int i = 0; i < origIndices.Length; i++)
            {
                resized.rawData[i] = bmp.rawData[origIndices[i]];
            }

            return resized;
        }
        public static Bitmap CropBitmap(Bitmap bitmap, int x, int y, int width, int height)
        {
            int[] originalData = bitmap.rawData;
            int croppedWidth = width;
            int croppedHeight = height;
            int[] croppedData = new int[croppedWidth * croppedHeight];

            for (int row = 0; row < croppedHeight; row++)
            {
                for (int col = 0; col < croppedWidth; col++)
                {
                    int originalX = x + col;
                    int originalY = y + row;
                    int originalIndex = (int)(originalY * bitmap.Width + originalX);
                    int croppedIndex = row * croppedWidth + col;
                    croppedData[croppedIndex] = originalData[originalIndex];
                }
            }
            bitmap = new Bitmap((uint)croppedWidth, (uint)croppedHeight, GUI.displayMode.ColorDepth);
            bitmap.rawData = croppedData;
            return bitmap;
        }

        public static Color[] ApplyBlur(Color[] colors, int x2, int y2, int blurRadius)
        {
            // create a temporary color array to store the blurred colors
            Color[] blurredColors = new Color[colors.Length];

            // calculate the Gaussian kernel
            double[] kernel = new double[blurRadius * 2 + 1];
            double sigma = blurRadius / 3.0;
            double sum = 0;
            for (int i = 0; i < kernel.Length; i++)
            {
                double x = i - blurRadius;
                kernel[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
                sum += kernel[i];
            }

            // normalize the kernel
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] /= sum;
            }

            // loop through the colors and apply the blur effect
            for (int y = 0; y < y2; y++)
            {
                for (int x = 0; x < x2; x++)
                {
                    double r = 0, g = 0, b = 0;
                    for (int i = -blurRadius; i <= blurRadius; i++)
                    {
                        int idx = x + i;
                        if (idx < 0 || idx >= x2)
                        {
                            idx = x;
                        }
                        Color color = colors[y * x2 + idx];
                        double weight = kernel[i + blurRadius];
                        r += color.R * weight;
                        g += color.G * weight;
                        b += color.B * weight;
                    }
                    int idx2 = y * x2 + x;
                    blurredColors[idx2] = Color.FromArgb((int)r, (int)g, (int)b);
                }
            }
            return blurredColors;
        }
        public static Bitmap ApplyBlur(Bitmap bitmap, int blurRadius)
        {
            uint width = bitmap.Width;
            uint height = bitmap.Height;

            // create a temporary bitmap to store the blurred image
            Bitmap blurredBitmap = new Bitmap(width, height, GUI.displayMode.ColorDepth);
            int[] blurredData = blurredBitmap.rawData;
            int[] originalData = bitmap.rawData;

            // calculate the Gaussian kernel
            double[] kernel = new double[blurRadius * 2 + 1];
            double sigma = blurRadius / 3.0;
            double sum = 0;
            for (int i = 0; i < kernel.Length; i++)
            {
                double x = i - blurRadius;
                kernel[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
                sum += kernel[i];
            }

            // normalize the kernel
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] /= sum;
            }

            // loop through the pixels and apply the blur effect
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
                    long idx2 = y * width + x;
                    int blurredPixel = ((int)r << 16) | ((int)g << 8) | (int)b;
                    blurredData[idx2] = blurredPixel;
                }
            }

            return blurredBitmap;
        }


        public static Color[] ApplyAlpha(Color[] colors1, Color[] colors2, byte alpha)
        {
            Color[] result = new Color[colors1.Length];
            for (int i = 0; i < colors1.Length; i++)
            {
                //result[i] = GUI.canvas.AlphaBlend(colors1[i], colors2[i], alpha);
            }
            return result;
        }
    }
    public class Font
    {
        public static int fontX = GUI.font.Width;
        public static int fontY = GUI.font.Height;
        public static Dictionary<char, bool[]> charCache = new Dictionary<char, bool[]>();
        public static Dictionary<char, bool[]> numCache = new Dictionary<char, bool[]>();
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

        public static void DrawChar(char c, int color, int[] canvas, int canvasWidth, int x2, int y2)
        {
            int fontY = Font.fontY;
            int fontX = Font.fontX;
            if (c == ' ')
            {
                return;
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
        }
        public static void DrawString(string str, int color, int x2, int y2, int[] bitmap, int canvasWidth)
        {
            int ogX = x2;
            foreach (char c in str)
            {
                if (c == '\n') { y2 += 20; x2 = ogX; continue; }
                DrawChar(c, color, bitmap, canvasWidth, x2, y2);
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