using System.Collections;
using Components.Events;
using Components.Scenes;
using Components.Stats;
using Components.TimeScale;
using UnityEngine;
using UnityEngine.UIElements;


namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiEndGame : BaseClasses.UiPopin
    {

        [HideInInspector] public Label Label_distanceReached;
        [HideInInspector] public Label Label_crystalsCollectedCount;
        [HideInInspector] public Label Label_lastBiomeReached;

        [HideInInspector] public Label Label_bestDistanceReached;
        [HideInInspector] public Label Label_bestCrystalsCollectedCount;
        [HideInInspector] public Label Label_bestLastBiomeReached;


        private void Start() => EventBus.Subscribe<SceneLoadedEvent>(OnSceneLoadedEvent);
        private void OnDestroy() => EventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoadedEvent);            
        private void OnSceneLoadedEvent(SceneLoadedEvent e)
        {
            if (this == null || gameObject == null) return;
            StartCoroutine(OnSceneLoaded());
        }

        private IEnumerator OnSceneLoaded()
        {
            yield return new WaitUntil(() => DocReady);

            Label_crystalsCollectedCount = root.Q<Label>("label--crystals-collected-count");
            Label_lastBiomeReached = root.Q<Label>("label--last-biome-reached");
            Label_distanceReached = root.Q<Label>("label--distance-reached");

            Label_bestCrystalsCollectedCount = root.Q<Label>("label--best--crystals-collected-count");
            Label_bestLastBiomeReached = root.Q<Label>("label--best--last-biome-reached");
            Label_bestDistanceReached = root.Q<Label>("label--best--distance-reached");
        }

        public override void OnOpen()
        {
            TimeScaleManager.Instance.PauseGame();

            Label_crystalsCollectedCount.text = StatsRecorder.CurrentRunCoinsCollected.ToString();
            Label_lastBiomeReached.text = StatsRecorder.LastBiomeReached;
            Label_distanceReached.text = StatsRecorder.CurrentRunDistanceReached.ToString();

            Label_bestCrystalsCollectedCount.text = StatsRecorder.GetMaxCoinsCollected().ToString();
            Label_bestDistanceReached.text = StatsRecorder.GetMaxDistanceReached().ToString();
        }

    }
}