using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiEndGame : UiPopin
{

    [HideInInspector] public Label label_paintCollectedCount;
    [HideInInspector] public Label label_lastBiomeReached;

    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () => StartCoroutine(_OnSceneLoaded());
    }

    private IEnumerator _OnSceneLoaded()
    {
        yield return new WaitUntil(() => docReady);

        label_paintCollectedCount = root.Q<Label>("label--paint-collected-count");
        label_lastBiomeReached = root.Q<Label>("label--last-biome-reached");
    }

    public override void OnOpen()
    {
        GameManager.Instance.PauseGame();

        label_paintCollectedCount.text = StatsRecorder.Instance.paintCollected.ToString();
        label_lastBiomeReached.text = StatsRecorder.Instance.lastBiomeReached;
    }

}