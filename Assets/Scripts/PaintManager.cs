

using UnityEngine;

public static class PaintManager
{
    public static int paintCollected = 0;
    public static float bucketFillPct = 0f;

    public static void Reset()
    {
        paintCollected = 0;
        ClearBucket();
    }


    /// <summary>
    ///   Add paint to the bucket
    /// </summary>
    /// <param name="amount">Amount to add (usually 1)</param>
    public static void AddPaint(int amount)
    {
        bucketFillPct += amount / 10f;
        bucketFillPct = Mathf.Clamp01(bucketFillPct);

        // Check if bucket is full
        if(bucketFillPct >= 1f)
        {   
            // Change to next biome
            var isLast = !BiomesData.Instance.SetNext();
            if(!isLast) ClearBucket();
            AudioManager.Instance.PlaySound("biome-change");
        }

        // Update HUD
        UiManager.Instance.hud.UpdatePaintBucket(bucketFillPct);

        paintCollected += amount;
        StatsRecorder.Instance.SetMaxCoinsCollected(paintCollected);
    }


    /// <summary>
    ///  Clear paint bucket
    /// </summary>
    private static void ClearBucket()
    {
        Debug.Log("[PaintManager] Clearing paint bucket.");
        bucketFillPct = 0f;
        UiManager.Instance.hud.UpdatePaintBucket(bucketFillPct);
    }



}
