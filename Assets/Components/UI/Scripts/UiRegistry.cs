using System.Collections.Generic;
using System.Threading.Tasks;
using Components.Events;
using Components.Scenes;
using Components.UI.Scripts.Controllers;
using Components.UI.Scripts.Controllers.BaseClasses;
using UnityEngine;


namespace Components.UI.Scripts
{
    /// <summary>
    ///  Manages UI elements references in the game.
    /// </summary>
    public class UiRegistry : Singleton.Model<UiRegistry>, IInitializable, IGameService
    {

        [Header("Initialization")]
        public int InitPriority => 2;
        public System.Type[] InitDependencies => null;
        public bool IsReady { get; private set; } = false;


        [Header("UI Controllers")]
        public UiScreenOverlay ScreenOverlay;
        public UiPauseMenu PauseMenu;
        public UiHud Hud;
        public UiEndGame EndGameScreen;
        public UiSplashScreen SplashScreen;
        public UiCountdown Countdown;


        public async Task InitializeAsync()
        {
            EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);


            // Wait for all controllers to be ready
            GetReferences();
            var initializationTasks = new List<Task>();
            foreach (var uiController in new UiController[]
            {
                ScreenOverlay,
                PauseMenu,
                Hud,
                EndGameScreen,
                SplashScreen
            })
            {
                initializationTasks.Add(uiController.InitializeAsync());
            }

            await Task.WhenAll(initializationTasks);
            IsReady = true;
        }


        // Ensure references are up to date on scene load
        private void OnSceneLoadedEvent(SceneLoadedEvent evt)
        {
            GetReferences();
        }

        /// <summary>
        ///   Retrieves and assigns references to key UI components.
        /// </summary>
        private void GetReferences()
        {
            if (ScreenOverlay == null)
            {
                ScreenOverlay = transform.Find("Screen Overlay").GetComponent<UiScreenOverlay>();

                if (ScreenOverlay == null)
                    Debug.LogError("[UiRegistry] Screen Overlay is missing!");
            }

            if (PauseMenu == null)
            {
                PauseMenu = transform.Find("Pause Menu").GetComponent<UiPauseMenu>();

                if (PauseMenu == null)
                    Debug.LogError("[UiRegistry] Pause Menu is missing!");
            }

            if (Hud == null)
            {
                Hud = transform.Find("HUD").GetComponent<UiHud>();

                if (Hud == null)
                    Debug.LogError("[UiRegistry] HUD is missing!");
            }

            if (EndGameScreen == null)
            {
                EndGameScreen = transform.Find("End Game Screen").GetComponent<UiEndGame>();

                if (EndGameScreen == null)
                    Debug.LogError("[UiRegistry] End Game Screen is missing!");
            }

            if (SplashScreen == null)
            {
                SplashScreen = transform.Find("Splash Screen").GetComponent<UiSplashScreen>();

                if (SplashScreen == null)
                    Debug.LogError("[UiRegistry] Splash Screen is missing!");
            }


            if (Countdown == null)
            {
                Countdown = transform.Find("Countdown").GetComponent<UiCountdown>();

                if (Countdown == null)
                    Debug.LogError("[UiRegistry] Countdown is missing!");
            }


        }

    }
}