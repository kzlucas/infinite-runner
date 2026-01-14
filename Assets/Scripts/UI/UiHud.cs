using System.Collections;
using Unity.VisualScripting;
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
        bucket = root.Q<VisualElement>("bucket");
        yield return null;
    }

    /// <summary>
    /// Updates the paint bucket UI fill percentage.
    /// </summary>
    /// <param name="fillPct"></param>
    public void UpdatePaintBucket(float fillPct)
    {
        StartCoroutine(_UpdatePaintBucket(fillPct));
    }

    private IEnumerator _UpdatePaintBucket(float fillPct)
    {
        yield return new WaitUntil(() => docReady && bucket != null);
        bucket.style.width = Length.Percent(fillPct * 100f);
    }
}
