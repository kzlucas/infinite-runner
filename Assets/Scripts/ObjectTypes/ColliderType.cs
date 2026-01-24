using UnityEngine;

public class ColliderType : MonoBehaviour
{
    public enum Type
    {
        Platform,
        DeathZone,
        Collectible,
        ZoneChange,
        None,
    }

    public Type colliderType = Type.None;

    public string soundToPlayOnCollision = "";

}
