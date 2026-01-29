using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiEndGame : UiPopin
{

    [HideInInspector] public Label label_distanceReached;
    [HideInInspector] public Label label_crystalsCollectedCount;
    [HideInInspector] public Label label_lastBiomeReached;

    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () => StartCoroutine(_OnSceneLoaded());
    }

    private IEnumerator _OnSceneLoaded()
    {
        yield return new WaitUntil(() => docReady);

        label_crystalsCollectedCount = root.Q<Label>("label--paint-collected-count");
        label_lastBiomeReached = root.Q<Label>("label--last-biome-reached");
        label_distanceReached = root.Q<Label>("label--distance-reached");
    }

    public override void OnOpen()
    {
        TimeScaleManager.Instance.PauseGame();

        label_crystalsCollectedCount.text = StatsRecorder.Instance.currentRunCoinsCollected.ToString();
        label_lastBiomeReached.text = StatsRecorder.Instance.lastBiomeReached;
        label_distanceReached.text = StatsRecorder.Instance.currentRunDistanceReached.ToString();
    }

}