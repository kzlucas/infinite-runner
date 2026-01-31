using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Components.ServiceLocator.Scripts
{
    /// <summary>
    /// Manages service registration and initialization
    /// </summary>
    public class ServiceRegistry : MonoBehaviour, IInitializable
    {
        Type[] TypesToFind = new Type[]{};

        public int initPriority => -1;
        public System.Type[] initDependencies => null;

        [Header("Services to Auto-Register")]
        [SerializeField] private MonoBehaviour[] autoRegisterServices;

        public async Task InitializeAsync()
        {
            Debug.Log("[ServiceRegistry] Initializing services...");

            // Register this registry first
            ServiceLocator.Register<ServiceRegistry>(this);

            // Auto-register marked services
            foreach (var service in autoRegisterServices)
            {
                if (service != null)
                {
                    RegisterService(service);
                }
            }

            // Register common services found in scene
            await RegisterFoundServices();

            Debug.Log("[ServiceRegistry] Service registration complete");
        }

        private void RegisterService(MonoBehaviour service)
        {
            var serviceType = service.GetType();

            // Register by concrete type
            ServiceLocator.Register(serviceType, service);

            // Register by interfaces
            var interfaces = serviceType.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                // if (interfaceType != typeof(IInitializable)) // Skip common interfaces
                // {
                ServiceLocator.Register(interfaceType, service);
                // }
            }

            Debug.Log($"[ServiceRegistry] Registered service: {serviceType.Name}");
        }

        private async Task RegisterFoundServices()
        {

            foreach (var type in TypesToFind)
            {
                var serviceInstance = FindFirstObjectByType(type);
                if (serviceInstance != null)
                {
                    ServiceLocator.Register(type, serviceInstance);
                }
            }

            await Task.CompletedTask;
        }

        private void OnDestroy()
        {
            // Clean up services when scene changes
            ServiceLocator.Clear();
        }

        public static void LogServices()
        {
            ServiceLocator.LogRegisteredServices();
        }
    }
}