using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : Singleton<GameManager>
{

    public InputActionReference reloadSceneActionRef;


    private void OnDisable() => StopAllCoroutines();

    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            StopAllCoroutines();
            RegisterHandlers();
            if(SceneLoader.Instance.IsGameScene()) PaintManager.Reset();
        };
    }


    private void RegisterHandlers()
    {
#if UNITY_EDITOR
        InputHandlersManager.Instance.Register("Reload Scene", reloadSceneActionRef, OnTrigger: () =>
        {
            SceneLoader.Instance?.ReloadCurrentScene();
        });

#endif
    }


}