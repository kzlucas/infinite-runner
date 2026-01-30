using System;
using System.Collections;
using Components.UI.Scripts;
using Interfaces;
using UnityEngine;

namespace Components.EndGame.Scripts
{
    public class EndGameManager : MonoBehaviour, IService
    {

        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Scripts.ServiceLocator.Get<UiRegistry>();


        [Header("End Game Events")]
        public Action OnEndGame;
        private void OnDestroy() => OnEndGame = null;
        private void Start() => SceneLoader.Instance.OnSceneLoaded += () => StopAllCoroutines();


        public void TriggerEndGame()
        {
            OnEndGame?.Invoke();
            StartCoroutine(DelayScreenOpening());
        }


        private IEnumerator DelayScreenOpening(float delay = .75f)
        {
            yield return new WaitForSecondsRealtime(delay);
            UiRegistry.endGameScreen.Open();
        }
    }
}