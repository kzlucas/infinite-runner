using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiHud : UiController
{
    [HideInInspector] public VisualElement bucket;


    public override void OnDocReady()
    {
        StartCoroutine(_OnDocReady());
    }

    private IEnumerator _OnDocReady()
    {
        // Wait until doc is ready and get ref
        yield return new WaitUntil(() => docReady);
        bucket = root.Q<VisualElement>("bucket");
    }

    /// <summary>
    /// Updates the paint bucket UI fill percentage.
    /// </summary>
    /// <param name="fillPct"></param>
    public void UpdatePaintBucket(float fillPct)
    {
        bucket.style.width = Length.Percent(fillPct * 100f);
    }
}
