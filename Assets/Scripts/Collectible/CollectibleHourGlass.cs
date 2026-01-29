using UnityEngine;



public class CollectibleHourGlass : Collectible
{
   
    public override void OnCollide()
    {
        Debug.Log("[CollectibleHourGlass] Hour Glass collected");
        TimeScaleManager.Instance.SlowDownTime();
    }
}
