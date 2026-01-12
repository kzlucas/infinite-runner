

using NUnit.Framework;
using UnityEngine;

public static class PaintManager
{
    public static float bucketFillPct = 0f;


    /// <summary>
    ///   Add paint to the bucket
    /// </summary>
    /// <param name="amount">Amount to add in pct</param>
    public static void AddPaint(float amount)
    {
        bucketFillPct += amount;
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
    }


    /// <summary>
    ///  Clear paint bucket
    /// </summary>
    public static void ClearBucket()
    {
        Debug.Log("[PaintManager] Clearing paint bucket.");
        bucketFillPct = 0f;
        UiManager.Instance.hud.UpdatePaintBucket(bucketFillPct);
    }



}
