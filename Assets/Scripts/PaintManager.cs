

using UnityEngine;

public static class PaintManager
{
    public static float bucketFillPct = 0f;

    public static void AddPaint(float amount)
    {
        bucketFillPct += amount;
        bucketFillPct = Mathf.Clamp01(bucketFillPct);

        // Check if bucket is full
        if(bucketFillPct >= 1f)
        {   
            // Change to next biome
            BiomesData.Instance.SetNext();
            bucketFillPct = 0f;
        }

        // Update HUD
        UiManager.Instance.hud.UpdatePaintBucket(bucketFillPct);
    }



}
