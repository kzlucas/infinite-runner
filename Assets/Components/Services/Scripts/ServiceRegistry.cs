using System.Threading.Tasks;
using UnityEngine;


namespace Components.ServiceLocator.Scripts
{
    /// <summary>
    /// Manages service registration and initialization
    /// Integrates with the existing IInitializable system
    /// </summary>
    public class ServiceRegistry : MonoBehaviour, IInitializable
    {
        public int initPriority => -100; // Initialize very early
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
                if (interfaceType != typeof(IInitializable)) // Skip common interfaces
                {
                    ServiceLocator.Register(interfaceType, service);
                }
            }

            Debug.Log($"[ServiceRegistry] Registered service: {serviceType.Name}");
        }

        private async Task RegisterFoundServices()
        {
            // Register AudioManager
            var audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null)
            {
                ServiceLocator.Register<AudioManager>(audioManager);
            }

            // Register UiManager
            var uiManager = FindObjectOfType<UiManager>();
            if (uiManager != null)
            {
                ServiceLocator.Register<UiManager>(uiManager);
            }

            // Register GameManager
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                ServiceLocator.Register<GameManager>(gameManager);
            }

            // Register SceneLoader
            var sceneLoader = FindObjectOfType<SceneLoader>();
            if (sceneLoader != null)
            {
                ServiceLocator.Register<SceneLoader>(sceneLoader);
            }

            // Register EndGameManager
            var endGameManager = FindObjectOfType<EndGameManager>();
            if (endGameManager != null)
            {
                ServiceLocator.Register<EndGameManager>(endGameManager);
            }

            // Register SquareCollidersMerger
            var squareCollidersMerger = FindObjectOfType<SquareCollidersMerger>();
            if (squareCollidersMerger != null)
            {
                ServiceLocator.Register<SquareCollidersMerger>(squareCollidersMerger);
            }

            // Register InputHandlersManager
            var inputManager = FindObjectOfType<InputHandlersManager>();
            if (inputManager != null)
            {
                ServiceLocator.Register<InputHandlersManager>(inputManager);
            }

            await Task.CompletedTask;
        }

        private void OnDestroy()
        {
            // Clean up services when scene changes
            ServiceLocator.Clear();
        }

#if UNITY_EDITOR
        [ContextMenu("Log Registered Services")]
        private void LogServices()
        {
            ServiceLocator.LogRegisteredServices();
        }
#endif
    }
}