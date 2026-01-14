using System.Threading.Tasks;
using UnityEngine;

/// <summary>
///  Manages UI elements references in the game.
/// </summary>
public class UiManager : Singleton<UiManager>, IInitializable
{
    public int initPriority => 0;
    public System.Type[] initDependencies => null;


    [Header("UI Controllers")]
    public UiController screenOverlay;
    public UiPopin pauseMenu;
    public UiHud hud;
    public UiEndGame endGameScreen;
    public UiScreenOverlay screenFader;

    public Task InitializeAsync()
    {
        GetReferences();
        return Task.CompletedTask;
    }

    private void GetReferences()
    {
        if(screenOverlay == null)
        {   
            screenOverlay = transform.Find("Screen Overlay").GetComponent<UiController>();
            
            if(screenOverlay == null)
                Debug.LogError("[UiManager] Screen Overlay is missing!");            
        }

        if(pauseMenu == null)
        {   
            pauseMenu = transform.Find("Pause Menu").GetComponent<UiPopin>();
            
            if(pauseMenu == null)
                Debug.LogError("[UiManager] Pause Menu is missing!");            
        }

        if(hud == null)
        {   
            hud = transform.Find("HUD").GetComponent<UiHud>();
            
            if(hud == null)
                Debug.LogError("[UiManager] HUD is missing!");            
        }

        if(endGameScreen == null)
        {   
            endGameScreen = transform.Find("End Game Screen").GetComponent<UiEndGame>();
            
            if(endGameScreen == null)
                Debug.LogError("[UiManager] End Game Screen is missing!");            
        }

        if(screenFader == null)
        {   
            screenFader = transform.Find("Screen Overlay").GetComponent<UiScreenOverlay>();
            
            if(screenFader == null)
                Debug.LogError("[UiManager] Screen Fader is missing!");            
        }
   
    }

}