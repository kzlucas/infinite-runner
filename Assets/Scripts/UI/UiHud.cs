using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiHud : UiController
{
    [HideInInspector] public VisualElement bucketContainer;
    [HideInInspector] public VisualElement bucketFill;
    [HideInInspector] public Label bucketCounter;
    private int bucketFullPxWidth = 239;


    public override void OnDocReady()
    {
        StartCoroutine(_OnDocReady());
    }

    private IEnumerator _OnDocReady()
    {
        bucketContainer = root.Q<VisualElement>("bucket-container");
        bucketFill = root.Q<VisualElement>("bucket");
        bucketCounter = root.Q<Label>("bucket-counter");
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
        yield return new WaitUntil(() => docReady && bucketFill != null);
        bucketFill.style.width = fillPct * (float)bucketFullPxWidth;
        bucketCounter.text = Mathf.RoundToInt(fillPct * 100f).ToString() + "%";

        SetPaintBucketColor(
            BiomesData.Instance.current.ColorPaint,
            BiomesData.Instance.current.GaugeImage
        );
    }

    /// <summary>
    /// Sets the paint bucket color and gauge image.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="gaugeImage"></param>

    private void SetPaintBucketColor(Color color, Sprite gaugeImage)
    {
        StartCoroutine(_SetPaintBucketColor(color, gaugeImage));
    }
    private IEnumerator _SetPaintBucketColor(Color color, Sprite gaugeImage)
    {
        yield return new WaitUntil(() => docReady && bucketFill != null && bucketContainer != null);
        bucketFill.style.unityBackgroundImageTintColor = color;
        bucketContainer.style.backgroundImage = new StyleBackground(gaugeImage);

    }
}
