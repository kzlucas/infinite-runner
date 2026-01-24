

using UnityEngine;

public static class PaintManager
{
    public static int crystalsCollected = 0;
    public static int amountInBucket = 0;
    public static float bucketFillPct = 0f;



    public static void Reset()
    {
        crystalsCollected = 0;
        amountInBucket = 0;
        ClearBucket();
    }


    /// <summary>
    ///   Add paint to the bucket
    /// </summary>
    /// <param name="amount">Amount to add (usually 1)</param>
    public static void AddPaint(int amount)
    {
        amountInBucket += amount;
        bucketFillPct = amountInBucket / (float)BiomesData.Instance.current.CrystalsNeeded;
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
        crystalsCollected += amount;
        StatsRecorder.Instance.SetMaxCoinsCollected(crystalsCollected);
    }


    /// <summary>
    ///  Clear paint bucket
    /// </summary>
    private static void ClearBucket()
    {
        bucketFillPct = 0f;
        amountInBucket = 0;
        UiManager.Instance.hud.UpdatePaintBucket(bucketFillPct);
    }



}
