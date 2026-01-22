using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiSplashScreen : UiController
{
//     /*
//      *
//      * UI Toolkit is buggy in WebGL
//      * so we just launch the game on user interaction  
//      */
    
// #if UNITY_WEBGL && !UNITY_EDITOR
//     private void Update()
//     {
//         if (Mouse.current.leftButton.wasPressedThisFrame
//         || Touchscreen.current.primaryTouch.press.isPressed)
//         {
//             SceneLoader.Instance.Load("Game");
//         }
//     }
// #endif
}