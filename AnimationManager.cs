using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EncryptedDiary
{
    public class AnimationManager
    {
        public delegate void AnimationCallback(double value);
          private class Animation
        {
            public double From { get; set; }
            public double To { get; set; }
            public int Duration { get; set; } // In milliseconds
            public DateTime StartTime { get; set; }
            public AnimationCallback Callback { get; set; } = null!;
            public bool IsCompleted { get; set; }
            public string Tag { get; set; } = string.Empty;
        }
          private readonly List<Animation> animations = new List<Animation>();
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        
        public AnimationManager()
        {
            timer.Interval = 16; // ~60 fps
            timer.Tick += Timer_Tick;
        }
        
        private void Timer_Tick(object? sender, EventArgs e)
        {
            bool hasActiveAnimations = false;
            
            for (int i = animations.Count - 1; i >= 0; i--)
            {
                Animation animation = animations[i];
                
                if (animation.IsCompleted)
                {
                    animations.RemoveAt(i);
                    continue;
                }
                
                double elapsedTime = (DateTime.Now - animation.StartTime).TotalMilliseconds;
                double progress = Math.Min(1.0, elapsedTime / animation.Duration);
                
                // Apply easing
                double easedProgress = EaseInOut(progress);
                
                // Calculate current value
                double currentValue = animation.From + (animation.To - animation.From) * easedProgress;
                
                // Call the callback
                animation.Callback(currentValue);
                
                // Check if animation is complete
                if (progress >= 1.0)
                {
                    animation.IsCompleted = true;
                }
                else
                {
                    hasActiveAnimations = true;
                }
            }
            
            if (!hasActiveAnimations)
            {
                timer.Stop();
            }
        }
        
        private double EaseInOut(double t)
        {
            return t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
        }
        
        public void StartAnimation(double from, double to, int duration, AnimationCallback callback, string? tag = null)
        {
            if (duration <= 0) 
            {
                callback(to);
                return;
            }
            
            // Check if animation with same tag exists, and remove it if found
            if (!string.IsNullOrEmpty(tag))
            {
                for (int i = animations.Count - 1; i >= 0; i--)
                {
                    if (animations[i].Tag == tag)
                    {
                        animations.RemoveAt(i);
                    }
                }
            }
              // Create and add new animation
            animations.Add(new Animation
            {
                From = from,
                To = to,
                Duration = duration,
                StartTime = DateTime.Now,
                Callback = callback,
                IsCompleted = false,
                Tag = tag ?? string.Empty
            });
            
            if (!timer.Enabled)
            {
                timer.Start();
            }
        }
        
        public void StopAll()
        {
            animations.Clear();
            timer.Stop();
        }
        
        public bool IsAnimating => timer.Enabled;
          public void ForceComplete(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return;
                
            for (int i = animations.Count - 1; i >= 0; i--)
            {
                Animation animation = animations[i];
                if (animation.Tag == tag)
                {
                    animation.Callback(animation.To);
                    animations.RemoveAt(i);
                }
            }
        }
    }
}
