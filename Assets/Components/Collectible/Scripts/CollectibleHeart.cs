using UnityEngine;

namespace Components.Collectible
{

    public class CollectibleHeart : Collectible
    {

        public override void OnCollide()
        {
            if (IsCollected) return;
            IsCollected = true;

            Debug.Log("[CollectibleHeart] Heart collected");
            Components.Player.Utils.PlayerController.GetComponent<Player.Health>().Heal(1);
        }
    }
}
