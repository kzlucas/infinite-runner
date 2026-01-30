using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components.ServiceLocator.Scripts
{

    /// <summary>
    /// Service Locator for dependency management
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _serviceFactories = new Dictionary<Type, object>();

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Register a service instance
        /// </summary>
        /// <typeparam name="T">Service interface or type</typeparam>
        /// <param name="service">Service implementation</param>
        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (Instance._services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Service {type.Name} is already registered. Overwriting.");
            }
            
            Instance._services[type] = service;
            Debug.Log($"[ServiceLocator] Registered service: {type.Name}");
        }

        /// <summary>
        /// Register a service instance by type
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="service">Service implementation</param>
        public static void Register(Type serviceType, object service)
        {
            if (Instance._services.ContainsKey(serviceType))
            {
                Debug.LogWarning($"[ServiceLocator] Service {serviceType.Name} is already registered. Overwriting.");
            }
            
            Instance._services[serviceType] = service;
            Debug.Log($"[ServiceLocator] Registered service: {serviceType.Name}");
        }

        /// <summary>
        /// Register a service factory for lazy initialization
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="factory">Factory function</param>
        public static void RegisterFactory<T>(Func<T> factory)
        {
            var type = typeof(T);
            Instance._serviceFactories[type] = factory;
            Debug.Log($"[ServiceLocator] Registered factory for: {type.Name}");
        }

        /// <summary>
        /// Get a service instance
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance</returns>
        public static T Get<T>()
        {

            var type = typeof(T);

            // Check if service is already registered
            if (Instance._services.TryGetValue(type, out var service))
            {
                return (T)service;
            }

            // Check if factory is available
            if (Instance._serviceFactories.TryGetValue(type, out var factory))
            {
                var createdService = ((Func<T>)factory)();
                Instance._services[type] = createdService;
                return createdService;
            }

            // Try to find service in scene if it's a MonoBehaviour
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                var found = UnityEngine.Object.FindFirstObjectByType(type);
                if (found != null)
                {
                    Instance._services[type] = found;
                    
                    return (T)(object)found;
                }
            }

            // Try to find service in scene if it's a MonoBehaviour
            if (typeof(MonoBehaviour).IsAssignableFrom(type))
            {
                var found = UnityEngine.Object.FindFirstObjectByType(type);
                if (found != null)
                {
                    Debug.LogWarning($"[ServiceLocator] Service {type.Name} was not registered but found in scene. Auto-registering.");
                    Instance._services[type] = found;
                    return (T)(object)found;
                }
            }

            throw new InvalidOperationException($"[ServiceLocator] Service {type.Name} not registered and could not be found.");
        }

        /// <summary>
        /// Try to get a service, returns null if not found
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>Service instance or null</returns>
        public static T TryGet<T>() where T : class
        {
            try
            {
                return Get<T>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Check if service is registered
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <returns>True if registered</returns>
        public static bool IsRegistered<T>()
        {
            var type = typeof(T);
            return Instance._services.ContainsKey(type) || Instance._serviceFactories.ContainsKey(type);
        }

        /// <summary>
        /// Unregister a service
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        public static void Unregister<T>()
        {
            var type = typeof(T);
            Instance._services.Remove(type);
            Instance._serviceFactories.Remove(type);
            Debug.Log($"[ServiceLocator] Unregistered service: {type.Name}");
        }

        /// <summary>
        /// Clear all services (useful for testing or scene transitions)
        /// </summary>
        public static void Clear()
        {
            Instance._services.Clear();
            Instance._serviceFactories.Clear();
            Debug.Log("[ServiceLocator] Cleared all services");
        }

        /// <summary>
        /// Reset the service locator (creates new instance)
        /// </summary>
        public static void Reset()
        {
            _instance = null;
            Debug.Log("[ServiceLocator] Reset service locator");
        }

    #if UNITY_EDITOR
        /// <summary>
        /// Debug method to list all registered services
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogRegisteredServices()
        {
            Debug.Log($"[ServiceLocator] Registered Services ({Instance._services.Count}):");
            foreach (var kvp in Instance._services)
            {
                Debug.Log($"  - {kvp.Key.Name}: {kvp.Value?.GetType().Name ?? "null"}");
            }
            
            Debug.Log($"[ServiceLocator] Registered Factories ({Instance._serviceFactories.Count}):");
            foreach (var kvp in Instance._serviceFactories)
            {
                Debug.Log($"  - {kvp.Key.Name}");
            }
        }
    #endif
    }
}