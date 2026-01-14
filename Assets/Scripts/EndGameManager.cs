using System;
using System.Collections;
using UnityEngine;


public class EndGameManager : Singleton<EndGameManager>
{
    public Action OnEndGame;


    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () => StopAllCoroutines();
    }


    public void TriggerEndGame()
    {
        OnEndGame?.Invoke();
        StartCoroutine(DelayScreenOpening());
    }

    private IEnumerator DelayScreenOpening(float delay = .75f)
    {
        yield return new WaitForSeconds(delay);
        UiManager.Instance.endGameScreen.Open();
    }
}