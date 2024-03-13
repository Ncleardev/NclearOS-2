using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static Cosmos.Common.BinaryHelper;

namespace NclearOS2.GUI
{
    public static class Animation
    {
        public enum Property
        {
            Stationary = 0,
            Alpha = 1,
            TranslationX = 2,
            TranslationY = 3,
            Translation = 4,
            ScaleX = 5,
            ScaleY = 6,
            Scale = 7,
            Darken = 8,
            Fade = 9
        }

        public static List<Animator> Queue = new(); 
        public static List<Animator> Running = new();

        public static void Update()
        {
            foreach (var animator in Running)
            {
                if (GUI.animationEffects == false || animator.Update()) { animator.onFinished?.Invoke(); Running.Remove(animator); }
            }
            while(Queue.Count != 0)
            {
                if (GUI.animationEffects == false || Queue[0].Update()) { Queue[0].onFinished?.Invoke(); Queue.RemoveAt(0); } else { return; }
            }
        }
    }
    internal class AnimationDebug : Window
    {
        internal AnimationDebug() : base("Animation Debug", 400, 200, Icons.info, ProcessManager.Priority.Realtime) { }
        internal override void Update()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            byte i = 0;
            foreach (var animator in Animation.Running.Concat(Animation.Queue).ToArray())
            {
                i++;
                Print(animator, i);
            }
        }
        private void Print(Animator a, int i)
        {
            DrawStringAlpha(a.elapsed.ToString(), Color.White.ToArgb(), 10, i * 20 + 10); DrawStringAlpha(a.duration.ToString(), Color.White.ToArgb(), 50, i * 20 + 10); DrawStringAlpha(a.startValue +"", Color.White.ToArgb(), x - 150, i * 20 + 10); DrawStringAlpha(a.step+"", Color.White.ToArgb(), x - 70, i * 20 + 10);
        }
        internal override int Start()
        {
            Background(GUI.DarkGrayPen.ValueARGB);
            return 0;
        }


    }
    public class Animator
    {
        public bool inverted;
        public bool inverted2;
        public float startValue;
        public float startValue2;
        public float endValue;
        public float endValue2;
        public float step;
        public float step2;
        public short pos;
        public int duration;
        public Animation.Property property;
        public Bitmap obj;
        public Bitmap obj2;
        public short elapsed = 0;
        public Action onFinished;

        //TranslationX, TranslationY
        public Animator(Bitmap obj, Animation.Property property, short endValue = 100, short startValue = 0, short duration = 50, short pos = 0, Action onFinished = null)
        {
            inverted = startValue > endValue;
            this.obj = obj;
            this.property = property;
            this.endValue = endValue;
            this.startValue = startValue;
            this.pos = pos;
            this.duration = duration;
            this.onFinished = onFinished;
        }
        //TranslationX&Y
        public Animator(short duration, Bitmap obj, short endValueX = 100, short endValueY = 100, short startValueX = 0, short startValueY = 0, Action onFinished = null)
        {

            if (Math.Abs(startValueX - endValueX) < 5) { property = Animation.Property.TranslationY; pos = startValueX; startValue = startValueY; endValue = endValueY; }
            else if (Math.Abs(startValueY - endValueY) < 5) { property = Animation.Property.TranslationX; pos = startValueY; startValue = startValueX; endValue = endValueX; }
            else
            {
                property = Animation.Property.Translation;
                endValue = endValueX;
                startValue = startValueX;
                endValue2 = endValueY;
                startValue2 = startValueY;
                inverted2 = startValue2 > endValue2;
            }
            inverted = startValue > endValue;
            this.obj = obj;
            this.duration = duration;
            this.onFinished = onFinished;
        }
        //Stationary
        public Animator(short x, short y, short duration, Bitmap obj, Action onFinished = null)
        {
            property = Animation.Property.Stationary;
            this.startValue = x;
            this.startValue2 = y;
            this.duration = duration * 100;
            this.obj = obj;
            this.onFinished = onFinished;
        }
        //Alpha
        public Animator(Bitmap obj, short x, short y, short duration, Action onFinished = null)
        {
            property = Animation.Property.Alpha;
            this.startValue = x;
            this.startValue2 = y;
            this.duration = duration;
            this.obj = obj;
            this.onFinished = onFinished;
        }
        //ScaleX, ScaleY
        public Animator(Animation.Property property, short x, short y, short duration, bool isIncreasing, Bitmap obj, Action onFinished = null)
        {
            this.property = property;
            startValue = x;
            startValue2 = y;
            this.duration = duration * 100;
            inverted = isIncreasing;
            this.obj = obj;
            this.onFinished = onFinished;
        }
        //Darken
        public Animator(short x, short y, Bitmap obj, short duration, Action onFinished = null)
        {
            property = Animation.Property.Darken;
            this.startValue = x;
            this.startValue2 = y;
            this.duration = duration;
            this.obj = obj;
            this.onFinished = onFinished;
        }
        //Fade
        public Animator(Bitmap obj, Bitmap obj2, short x, short y, short duration, Action onFinished = null)
        {
            property = Animation.Property.Fade;
            this.startValue = x;
            this.startValue2 = y;
            this.duration = duration;
            this.obj = obj;
            this.obj2 = obj2;
            this.onFinished = onFinished;
        }

        public bool Update()
        {
            switch (property)
            {
                case Animation.Property.Stationary:
                    float frameTime = 1.0f / GUI.fps * 200;
                    step = frameTime / duration * 2; step2 += step;
                    GUI.canvas.DrawImage(obj, (int)startValue, (int)startValue2);
                    return step2 >= 1;
                case Animation.Property.TranslationX:
                case Animation.Property.TranslationY:
                case Animation.Property.Translation:
                    frameTime = 1.0f / GUI.fps * 200;
                    switch (property)
                    {
                        case Animation.Property.TranslationX:
                            GUI.canvas.DrawImage(obj, (int)startValue, pos);
                            step = inverted ?
                                Math.Clamp((endValue - startValue) * EaseOut(elapsed / duration) * frameTime / 30f, -100, -1) :
                                Math.Clamp((endValue - startValue) * EaseOut(elapsed / duration) * frameTime / 30f, 1, 100);
                            break;
                        case Animation.Property.TranslationY:
                            GUI.canvas.DrawImage(obj, pos, (int)startValue);
                            step = inverted ?
                                Math.Clamp((endValue - startValue) * EaseOut(elapsed / duration) * frameTime / 30f, -100, -1) :
                                Math.Clamp((endValue - startValue) * EaseOut(elapsed / duration) * frameTime / 30f, 1, 100);
                            break;
                        case Animation.Property.Translation:
                            GUI.canvas.DrawImage(obj, (int)startValue, (int)startValue2);
                            step = inverted ?
                                Math.Clamp((endValue - startValue) * EaseOut(elapsed / duration) * frameTime / 30f, -100, -0.1f) :
                                Math.Clamp((endValue - startValue) * EaseOut(elapsed / duration) * frameTime / 30f, 0.1f, 100);
                            step2 = inverted2 ?
                                Math.Clamp((endValue2 - startValue2) * EaseOut(elapsed / duration) * frameTime / 30f, -100, -0.1f) :
                                Math.Clamp((endValue2 - startValue2) * EaseOut(elapsed / duration) * frameTime / 30f, 0.1f, 100);
                            startValue2 += step2;
                            break;
                    }
                    if ((inverted && startValue <= endValue) || (!inverted && startValue >= endValue)) { return true; }
                    startValue += step;
                    elapsed++;
                    return false;
                case Animation.Property.Alpha:
                    frameTime = 1.0f / GUI.fps * 200;
                    step = frameTime / duration * 2; step2 += step;
                    obj.rawData = PostProcess.ApplyAlpha(obj.rawData, 1 - step);
                    GUI.canvas.DrawImageAlpha(obj, (int)startValue, (int)startValue2);
                    return step2 >= 1;
                case Animation.Property.ScaleX:
                case Animation.Property.ScaleY:
                    frameTime = 1.0f / GUI.fps * 200;
                    step = frameTime / duration * 2; step2 += step;
                    //obj.rawData = PostProcess.ResizeBitmap(obj, )
                    GUI.canvas.DrawImage(obj, (int)startValue, (int)startValue2);
                    return step2 >= 1;
                case Animation.Property.Darken:
                    frameTime = 1.0f / GUI.fps * 200;
                    step = frameTime / duration * 2; step2 += step;
                    obj.rawData = PostProcess.DarkenBitmap(obj.rawData, 1 - step);
                    GUI.canvas.DrawImage(obj, (int)startValue, (int)startValue2);
                    return step2 >= 1;
                case Animation.Property.Fade:
                    frameTime = 1.0f / GUI.fps * 200;
                    step = frameTime / duration * 2; step2 += step;
                    //obj.rawData = PostProcess.FadeBitmaps(obj.rawData, obj2.rawData, 1 - step);
                    GUI.canvas.DrawImage(obj, (int)startValue, (int)startValue2);
                    return step2 >= 1;
            }
            return false;
        }
        static float EaseOut(float t)
        {
            return 2.0f - t * t * t * t;
        }
    }

}
