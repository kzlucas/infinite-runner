using UnityEngine;



public class CollectibleHourGlass : Collectible
{    
    public override void OnCollide()
    {
        if (IsCollected) return;
        IsCollected = true;

        Debug.Log("[CollectibleHourGlass] Hour Glass collected");
        TimeScaleManager.Instance.SlowDownTime();
    }
}
