using System.Collections.Generic;
using System.Threading.Tasks;
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
            // Wait for all controllers to be ready
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
    }
}