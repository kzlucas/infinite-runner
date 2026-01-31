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
            Player.Utils.Locate().GetComponent<Player.Health>().Heal(1);
        }
    }
}
