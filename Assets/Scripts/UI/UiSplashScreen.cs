using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using Scene.Scripts;
using Components.Events;


[RequireComponent(typeof(UIDocument))]
public class UiSplashScreen : UiController
{



    public override void OnDocReady()
    {
        EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
        OnSceneLoadedEvent(new SceneLoadedEvent(SceneLoader.Instance.GetSceneName()));
    }


    private void OnSceneLoadedEvent(SceneLoadedEvent evt)
    {

        root.style.display = SceneLoader.Instance.GetSceneName() == "Splash Screen" 
                ? DisplayStyle.Flex 
                : DisplayStyle.None;
    }

}   