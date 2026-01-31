using UnityEngine;

public class AnimationEvent__Destroy : MonoBehaviour
{
    public GameObject ToDestroyGo;

    /// <summary>
    ///   Destroys the GameObject assigned to toDestroyGo.
    /// </summary>
    public void DestroySettedGameObject()
    {
        Destroy(ToDestroyGo);
    }
}
