using UnityEngine;

public class AnimationEvent__CoinCaught : MonoBehaviour
{

    /// <summary>
    ///  Adds paint to the paint bucket when a coin is caught.
    /// </summary>
    public void Do()
    {
        PaintManager.AddPaint(1);
    }
}
