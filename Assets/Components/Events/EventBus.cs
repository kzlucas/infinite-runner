using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.Events
{
    /// <summary>
    /// Centralized event management system
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<object>> _handlers = new();
        private static readonly Dictionary<Type, List<object>> _onceHandlers = new();

        /// <summary>
        /// Subscribe to an event
        /// </summary>
        public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new List<object>();
                
            _handlers[eventType].Add(handler);
            
            Debug.Log($"[EventBus] Subscribed to {eventType.Name}");
        }

        /// <summary>
        /// Subscribe to an event that will auto-unsubscribe after first trigger
        /// </summary>
        public static void SubscribeOnce<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (!_onceHandlers.ContainsKey(eventType))
                _onceHandlers[eventType] = new List<object>();
                
            _onceHandlers[eventType].Add(handler);
        }

        /// <summary>
        /// Unsubscribe from an event
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handler);
                if (_handlers[eventType].Count == 0)
                    _handlers.Remove(eventType);
            }
        }

        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        public static void Publish<T>(T eventData) where T : IGameEvent
        {
            var eventType = typeof(T);
            
            // Handle regular subscribers
            if (_handlers.ContainsKey(eventType))
            {
                var handlersToCall = new List<object>(_handlers[eventType]);
                foreach (Action<T> handler in handlersToCall)
                {
                    try
                    {
                        handler?.Invoke(eventData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[EventBus] Error in event handler for {eventType.Name}: {ex.Message}");
                    }
                }
            }

            // Handle once subscribers
            if (_onceHandlers.ContainsKey(eventType))
            {
                var onceHandlersToCall = new List<object>(_onceHandlers[eventType]);
                _onceHandlers[eventType].Clear();
                
                foreach (Action<T> handler in onceHandlersToCall)
                {
                    try
                    {
                        handler?.Invoke(eventData);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[EventBus] Error in once event handler for {eventType.Name}: {ex.Message}");
                    }
                }
            }

            Debug.Log($"[EventBus] Published {eventType.Name}");
        }

        /// <summary>
        /// Clear all event subscriptions (useful for scene changes)
        /// </summary>
        public static void Clear()
        {
            _handlers.Clear();
            _onceHandlers.Clear();
            Debug.Log("[EventBus] Cleared all event subscriptions");
        }

        /// <summary>
        /// Get subscriber count for an event type
        /// </summary>
        public static int GetSubscriberCount<T>() where T : IGameEvent
        {
            var eventType = typeof(T);
            int count = 0;
            
            if (_handlers.ContainsKey(eventType))
                count += _handlers[eventType].Count;
                
            if (_onceHandlers.ContainsKey(eventType))
                count += _onceHandlers[eventType].Count;
                
            return count;
        }

        /// <summary>
        /// Log all active subscriptions
        /// </summary>
        public static void LogActiveSubscriptions()
        {
            Debug.Log($"[EventBus] Active subscriptions: {_handlers.Count} event types");
            foreach (var kvp in _handlers)
            {
                Debug.Log($"  {kvp.Key.Name}: {kvp.Value.Count} handlers");
            }
        }
    }
}