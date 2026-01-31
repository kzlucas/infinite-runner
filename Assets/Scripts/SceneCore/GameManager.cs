using System.Collections;
using Components.ServiceLocator.Scripts;
using InputsHandler;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : Singleton.Model<GameManager>
{

        
    [Header("Dependencies")]
    private InputHandlersManager InputHandlersManager => ServiceLocator.Get<InputHandlersManager>();
    
    
    [Header("References")]
    public InputActionReference reloadSceneActionRef;

    

    private void Start() => SceneLoader.Instance.OnSceneLoaded += OnSceneLoaded;
    private void OnSceneLoaded() => StartCoroutine(HandleSceneLoaded());

    private IEnumerator HandleSceneLoaded()
    {
        yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized);
        RegisterHandlers();
        if(SceneLoader.Instance.IsGameScene()) CrystalsManager.Reset();
    }


    private void RegisterHandlers()
    {
        InputHandlersManager.Register("Reload Scene", reloadSceneActionRef, OnTrigger: () =>
        {
            SceneLoader.Instance?.ReloadCurrentScene();
        });

    }


}