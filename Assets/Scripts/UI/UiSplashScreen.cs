using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


[RequireComponent(typeof(UIDocument))]
public class UiSplashScreen : UiController
{

    public override void OnDocReady()
    {
        root.style.display = SceneLoader.Instance.GetSceneName() == "Splash Screen" 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
    }

}