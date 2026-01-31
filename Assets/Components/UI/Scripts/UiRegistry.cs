using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Components.UI.Scripts
{
    /// <summary>
    ///  Manages UI elements references in the game.
    /// </summary>
    public class UiRegistry : Singleton.Model<UiRegistry>, IInitializable, IGameService
    {

        [Header("Initialization")]
        public int initPriority => 2;
        public System.Type[] initDependencies => null;
        public bool isReady { get; private set; } = false;


        [Header("UI Controllers")]
        public UiScreenOverlay screenOverlay;
        public UiPauseMenu pauseMenu;
        public UiHud hud;
        public UiEndGame endGameScreen;
        public UiSplashScreen splashScreen;
        public UiCountdown countdown;


        public async Task InitializeAsync()
        {
            // Wait for all controllers to be ready
            GetReferences();
            var initializationTasks = new List<Task>();
            foreach (var uiController in new UiController[]
            {
                screenOverlay,
                pauseMenu,
                hud,
                endGameScreen,
                splashScreen
            })
            {
                initializationTasks.Add(uiController.InitializeAsync());
            }

            await Task.WhenAll(initializationTasks);
            isReady = true;
        }

        private void GetReferences()
        {
            if (screenOverlay == null)
            {
                screenOverlay = transform.Find("Screen Overlay").GetComponent<UiScreenOverlay>();

                if (screenOverlay == null)
                    Debug.LogError("[UiRegistry] Screen Overlay is missing!");
            }

            if (pauseMenu == null)
            {
                pauseMenu = transform.Find("Pause Menu").GetComponent<UiPauseMenu>();

                if (pauseMenu == null)
                    Debug.LogError("[UiRegistry] Pause Menu is missing!");
            }

            if (hud == null)
            {
                hud = transform.Find("HUD").GetComponent<UiHud>();

                if (hud == null)
                    Debug.LogError("[UiRegistry] HUD is missing!");
            }

            if (endGameScreen == null)
            {
                endGameScreen = transform.Find("End Game Screen").GetComponent<UiEndGame>();

                if (endGameScreen == null)
                    Debug.LogError("[UiRegistry] End Game Screen is missing!");
            }

            if (splashScreen == null)
            {
                splashScreen = transform.Find("Splash Screen").GetComponent<UiSplashScreen>();

                if (splashScreen == null)
                    Debug.LogError("[UiRegistry] Splash Screen is missing!");
            }


            if (countdown == null)
            {
                countdown = transform.Find("Countdown").GetComponent<UiCountdown>();

                if (countdown == null)
                    Debug.LogError("[UiRegistry] Countdown is missing!");
            }


        }

    }
}