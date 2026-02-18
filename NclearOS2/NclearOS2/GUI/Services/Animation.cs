using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace NclearOS2.GUI
{
    public static class Animation2
    {
        public static List<Animator2> Queue = new();
        public static List<Animator2> Running = new();

        public static void Update()
        {
            for (int i = Running.Count - 1; i >= 0; i--)
            {
                var animator = Running[i];
                if (!GUI.animationEffects || animator.ProcessFrame())
                {
                    if (!GUI.animationEffects) { animator.FireStartAction(); }
                    animator.FireEndAction();
                    Running.RemoveAt(i);
                }
            }

            if (Queue.Count > 0)
            {
                var next = Queue[0];
                if (!GUI.animationEffects || next.ProcessFrame())
                {
                    next.FireEndAction();
                    Queue.RemoveAt(0);
                }
            }
        }

        public static Animator2 Animate(Bitmap target)
        {
            return new Animator2(target);
        }
    }

    public class Animator2
    {
        private Bitmap _target;
        private Bitmap _secondaryTarget;
        private Action _onStart;
        private Action _onProgress;
        private Action _onFinished;

        private int _duration = 300;
        private ulong _startTime = 0;

        private int? _startX, _startY, _endX, _endY;
        private float _startAlpha = 1.0f, _endAlpha = 1.0f;
        private bool _isDarken = false;
        private bool _isFade = false;

        public Animator2(Bitmap target)
        {
            _target = target;
        }

        public enum InterpolationMode
        {
            Linear,
            EaseIn,
            EaseOut
        }

        public InterpolationMode interpolator = InterpolationMode.EaseOut;

        public Animator2 SetInterpolator(InterpolationMode i)
        {
            interpolator = i;
            return this;
        }
        public Animator2 SetDuration(int duration)
        {
            _duration = duration;
            return this;
        }

        public Animator2 MoveTo(int? x, int? y)
        {
            if (x.HasValue) _endX = x.Value;
            if (y.HasValue) _endY = y.Value;
            return this;
        }

        public Animator2 StartAt(int x, int y)
        {
            _startX = x;
            _startY = y;
            return this;
        }

        public Animator2 Fade(float fromAlpha = 1f, float toAlpha = 0f)
        {
            _startAlpha = fromAlpha;
            _endAlpha = toAlpha;
            interpolator = InterpolationMode.Linear;
            return this;
        }

        public Animator2 Darken()
        {
            _isDarken = true;
            interpolator = InterpolationMode.Linear;
            return this;
        }

        public Animator2 CrossFadeWith(Bitmap secondaryBitmap)
        {
            _secondaryTarget = secondaryBitmap;
            _isFade = true;
            return this;
        }

        public Animator2 WithEndAction(Action callback)
        {
            _onFinished = callback;
            return this;
        }

        public Animator2 WithProgressAction(Action callback)
        {
            _onProgress = callback;
            return this;
        }

        public Animator2 WithStartAction(Action callback)
        {
            _onStart = callback;
            return this;
        }

        public void Start()
        {
            if (!_startX.HasValue) _startX = 0;
            if (!_startY.HasValue) _startY = 0;
            if (!_endX.HasValue) _endX = _startX;
            if (!_endY.HasValue) _endY = _startY;

            Animation2.Running.Add(this);
        }

        public void Queue()
        {
            if (!_startX.HasValue) _startX = 0;
            if (!_startY.HasValue) _startY = 0;
            if (!_endX.HasValue) _endX = _startX;
            if (!_endY.HasValue) _endY = _startY;

            Animation2.Queue.Add(this);
        }

        public void FireEndAction()
        {
            _onFinished?.Invoke();
        }
        public void FireStartAction()
        {
            _onStart?.Invoke();
        }

        public bool ProcessFrame()
        {
            _onProgress?.Invoke();

            if (_startTime == 0)
            {
                _startTime = CPU.GetCPUUptime();
                FireStartAction();
            }

            double elapsedSeconds = (CPU.GetCPUUptime() - _startTime) / 10000000000.0;

            float progress = (float)Math.Min(1.0f, elapsedSeconds * 1000 / _duration * 2); 

            float easedStep = interpolator switch
            {
                InterpolationMode.EaseIn => EaseIn(progress),
                InterpolationMode.EaseOut => EaseOut(progress),
                _ => progress
            };

            int currentX = (int)Lerp(_startX.Value, _endX.Value, easedStep);
            int currentY = (int)Lerp(_startY.Value, _endY.Value, easedStep);

            if (_isDarken)
            {
                _target.rawData = PostProcess.DarkenBitmap(_target.rawData, 1 - (progress * .05f));
            }

            if (_startAlpha != _endAlpha)
            {
                float currentAlpha = Lerp(_startAlpha, _endAlpha, progress);
                _target.rawData = PostProcess.ApplyAlpha(_target.rawData, currentAlpha);
                GUI.canvas.DrawImageAlpha(_target, currentX, currentY);
            }
            else
            {
                GUI.canvas.DrawImage(_target, currentX, currentY);
            }

            return progress >= 1.0f;
        }

        private float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        private float EaseOut(float t)
        {
            return 1.0f - (float)Math.Pow(t - 1, 4);
        }
        private float EaseIn(float t)
        {
            return (float)Math.Pow(t, 4);
        }
    }
}