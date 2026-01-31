using System.Collections;
using Components.Events;
using Components.Player.Events;
using Components.UI.Scripts;
using UnityEngine;

namespace Components.EndGame.Scripts
{
    public class EndGameManager : MonoBehaviour, IGameService
    {

        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Scripts.ServiceLocator.Get<UiRegistry>();


        private void OnDestroy() => EventBus.Unsubscribe<Dead>(TriggerEndGame);
        private void Start() => EventBus.Subscribe<Dead>(TriggerEndGame);

        public void TriggerEndGame(Dead deadEvent)
        {
            StartCoroutine(DelayScreenOpening());
        }

        private IEnumerator DelayScreenOpening(float delay = .75f)
        {
            yield return new WaitForSecondsRealtime(delay);
            UiRegistry.EndGameScreen.Open();
        }
    }
}