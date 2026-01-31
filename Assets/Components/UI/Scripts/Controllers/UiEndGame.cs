using System.Collections;
using Components.Stats;
using Components.TimeScale;
using UnityEngine;
using UnityEngine.UIElements;


namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiEndGame : BaseClasses.UiPopin
    {

        [HideInInspector] public Label label_distanceReached;
        [HideInInspector] public Label label_crystalsCollectedCount;
        [HideInInspector] public Label label_lastBiomeReached;

        [HideInInspector] public Label label_bestDistanceReached;
        [HideInInspector] public Label label_bestCrystalsCollectedCount;
        [HideInInspector] public Label label_bestLastBiomeReached;


        private void Start() => SceneLoader.Instance.OnSceneLoaded += HandleSceneLoaded;
        private void OnDestroy() => SceneLoader.Instance.OnSceneLoaded -= HandleSceneLoaded;
        private void HandleSceneLoaded()
        {
            if (this == null || gameObject == null) return;
            StartCoroutine(OnSceneLoaded());
        }

        private IEnumerator OnSceneLoaded()
        {
            yield return new WaitUntil(() => docReady);

            label_crystalsCollectedCount = root.Q<Label>("label--crystals-collected-count");
            label_lastBiomeReached = root.Q<Label>("label--last-biome-reached");
            label_distanceReached = root.Q<Label>("label--distance-reached");

            label_bestCrystalsCollectedCount = root.Q<Label>("label--best--crystals-collected-count");
            label_bestLastBiomeReached = root.Q<Label>("label--best--last-biome-reached");
            label_bestDistanceReached = root.Q<Label>("label--best--distance-reached");
        }

        public override void OnOpen()
        {
            TimeScaleManager.Instance.PauseGame();

            label_crystalsCollectedCount.text = StatsRecorder.currentRunCoinsCollected.ToString();
            label_lastBiomeReached.text = StatsRecorder.lastBiomeReached;
            label_distanceReached.text = StatsRecorder.currentRunDistanceReached.ToString();

            label_bestCrystalsCollectedCount.text = StatsRecorder.GetMaxCoinsCollected().ToString();
            label_bestDistanceReached.text = StatsRecorder.GetMaxDistanceReached().ToString();
        }

    }
}