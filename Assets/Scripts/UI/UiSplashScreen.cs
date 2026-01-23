using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


[RequireComponent(typeof(UIDocument))]
public class UiSplashScreen : UiController
{


    private void Start()
    {
        // Show splash screen only in match scene
        SceneLoader.Instance.OnSceneLoaded += async () =>
        {
            await InitializeAsync();
            root.style.display = SceneLoader.Instance.GetSceneName() == "Splash Screen" 
                    ? DisplayStyle.None 
                    : DisplayStyle.Flex;
        };
    }

}