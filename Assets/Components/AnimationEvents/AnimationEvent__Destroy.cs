using UnityEngine;

public class AnimationEvent__Destroy : MonoBehaviour
{
    public GameObject toDestroyGo;

    /// <summary>
    ///   Destroys the GameObject assigned to toDestroyGo.
    /// </summary>
    public void DestroySettedGameObject()
    {
        Destroy(toDestroyGo);
    }
}
