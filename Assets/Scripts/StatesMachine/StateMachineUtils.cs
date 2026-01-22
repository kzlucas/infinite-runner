using System;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Utility class providing common state machine patterns and helpers
    /// </summary>
    public static class StateMachineUtils
    {
        /// <summary>
        /// Create a simple condition that checks if a float value meets a threshold
        /// </summary>
        /// <param name="getValue">Function to get the current value</param>
        /// <param name="threshold">Threshold to compare against</param>
        /// <param name="comparison">Type of comparison to perform</param>
        /// <returns>Condition function</returns>
        public static Func<bool> FloatCondition(Func<float> getValue, float threshold, ComparisonType comparison = ComparisonType.GreaterThan)
        {
            return () =>
            {
                float value = getValue();
                return comparison switch
                {
                    ComparisonType.GreaterThan => value > threshold,
                    ComparisonType.GreaterThanOrEqual => value >= threshold,
                    ComparisonType.LessThan => value < threshold,
                    ComparisonType.LessThanOrEqual => value <= threshold,
                    ComparisonType.Equal => Mathf.Approximately(value, threshold),
                    ComparisonType.NotEqual => !Mathf.Approximately(value, threshold),
                    _ => false
                };
            };
        }
        
        /// <summary>
        /// Create a simple condition that checks if a bool value is true
        /// </summary>
        /// <param name="getValue">Function to get the current boolean value</param>
        /// <returns>Condition function</returns>
        public static Func<bool> BoolCondition(Func<bool> getValue)
        {
            return getValue;
        }
        
        /// <summary>
        /// Create a condition that checks if a key is pressed
        /// </summary>
        /// <param name="key">KeyCode to check</param>
        /// <returns>Condition function</returns>
        public static Func<bool> KeyPressCondition(KeyCode key)
        {
            return () => Input.GetKeyDown(key);
        }
        
        /// <summary>
        /// Create a condition that checks if a key is held
        /// </summary>
        /// <param name="key">KeyCode to check</param>
        /// <returns>Condition function</returns>
        public static Func<bool> KeyHoldCondition(KeyCode key)
        {
            return () => Input.GetKey(key);
        }
        
        /// <summary>
        /// Create a timer-based condition
        /// </summary>
        /// <param name="duration">Duration in seconds</param>
        /// <returns>Timer condition that becomes true after the specified duration</returns>
        public static TimerCondition CreateTimer(float duration)
        {
            return new TimerCondition(duration);
        }
        
        /// <summary>
        /// Combine multiple conditions with AND logic
        /// </summary>
        /// <param name="conditions">Conditions to combine</param>
        /// <returns>Combined condition function</returns>
        public static Func<bool> And(params Func<bool>[] conditions)
        {
            return () =>
            {
                foreach (var condition in conditions)
                {
                    if (!condition()) return false;
                }
                return true;
            };
        }
        
        /// <summary>
        /// Combine multiple conditions with OR logic
        /// </summary>
        /// <param name="conditions">Conditions to combine</param>
        /// <returns>Combined condition function</returns>
        public static Func<bool> Or(params Func<bool>[] conditions)
        {
            return () =>
            {
                foreach (var condition in conditions)
                {
                    if (condition()) return true;
                }
                return false;
            };
        }
        
        /// <summary>
        /// Negate a condition
        /// </summary>
        /// <param name="condition">Condition to negate</param>
        /// <returns>Negated condition function</returns>
        public static Func<bool> Not(Func<bool> condition)
        {
            return () => !condition();
        }
    }
    
    /// <summary>
    /// Types of numerical comparisons
    /// </summary>
    public enum ComparisonType
    {
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Equal,
        NotEqual
    }
    
    /// <summary>
    /// Timer-based condition that becomes true after a specified duration
    /// </summary>
    public class TimerCondition
    {
        private float startTime;
        private float duration;
        private bool hasStarted;
        
        public TimerCondition(float duration)
        {
            this.duration = duration;
            Reset();
        }
        
        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            startTime = Time.time;
            hasStarted = true;
        }
        
        /// <summary>
        /// Reset the timer
        /// </summary>
        public void Reset()
        {
            hasStarted = false;
            startTime = 0f;
        }
        
        /// <summary>
        /// Check if the timer condition is met
        /// </summary>
        /// <returns>True if the duration has elapsed</returns>
        public bool IsComplete()
        {
            if (!hasStarted)
            {
                Start();
                return false;
            }
            
            return Time.time - startTime >= duration;
        }
        
        /// <summary>
        /// Get the remaining time
        /// </summary>
        /// <returns>Remaining time in seconds</returns>
        public float RemainingTime()
        {
            if (!hasStarted) return duration;
            
            float elapsed = Time.time - startTime;
            return Mathf.Max(0f, duration - elapsed);
        }
        
        /// <summary>
        /// Get the progress of the timer (0 to 1)
        /// </summary>
        /// <returns>Progress value between 0 and 1</returns>
        public float Progress()
        {
            if (!hasStarted) return 0f;
            
            float elapsed = Time.time - startTime;
            return Mathf.Clamp01(elapsed / duration);
        }
        
        /// <summary>
        /// Implicit conversion to boolean for easy use in conditions
        /// </summary>
        /// <param name="timer">Timer condition</param>
        /// <returns>True if timer is complete</returns>
        public static implicit operator bool(TimerCondition timer)
        {
            return timer.IsComplete();
        }
        
        /// <summary>
        /// Implicit conversion to Func&lt;bool&gt; for use in state machine transitions
        /// </summary>
        /// <param name="timer">Timer condition</param>
        /// <returns>Function that returns true if timer is complete</returns>
        public static implicit operator Func<bool>(TimerCondition timer)
        {
            return () => timer.IsComplete();
        }
    }
}