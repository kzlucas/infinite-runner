using UnityEngine;

public class ColliderType : MonoBehaviour
{
    public enum Type
    {
        Ground,
        Obstacle,
        Collectible,
        Wall,
        ZoneChange,
        None,
    }

    public Type colliderType = Type.None;

    public string soundToPlayOnCollision = "";

}
