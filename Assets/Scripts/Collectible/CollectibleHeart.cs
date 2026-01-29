using UnityEngine;



public class CollectibleHeart : Collectible
{

    public override void OnCollide()
    {
        if (IsCollected) return;
        IsCollected = true;

        Debug.Log("[CollectibleHeart] Heart collected");
        var playerHealth = FindFirstObjectByType<Player.Health>();
        playerHealth.Heal(1);
    }
}
