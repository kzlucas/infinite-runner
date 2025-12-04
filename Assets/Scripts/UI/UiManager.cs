using UnityEngine;

/// <summary>
///  Manages all UI elements references in the game.
/// </summary>
public class UiManager : Singleton<UiManager>
{


    [Header("UI Controllers")]
    public UiController screenOverlay;



    public override void Awake()
    {
        base.Awake();

        if(screenOverlay == null)
        {   
            screenOverlay = transform.Find("Screen Overlay").GetComponent<UiController>();
            
            if(screenOverlay == null)
                Debug.LogError("[UiManager] Screen Overlay is missing!");            
        }

    }
}