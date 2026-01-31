using UnityEngine;

namespace Assets.Components.SquareColliders.Scripts
{
    public class ColliderType : MonoBehaviour
    {
        public enum Type
        {
            Platform,
            MovingPlatform,
            DeathZone,
            Collectible,
            ZoneChange,
            None,
        }

        public Type colliderType = Type.None;

        public string soundToPlayOnCollision = "";

    }
}
