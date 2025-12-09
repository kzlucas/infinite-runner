using UnityEngine;

public class ColliderType : MonoBehaviour
{
    public enum Type
    {
        Ground,
        Obstacle,
        Collectible,
        Wall,
        None,
    }

    public Type colliderType = Type.None;

    public string soundToPlayOnCollision = "";

}
