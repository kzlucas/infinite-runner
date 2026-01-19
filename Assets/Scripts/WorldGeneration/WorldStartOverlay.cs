using System.Collections;
using UnityEngine;

public class WorldStartOverlay : Singleton<WorldStartOverlay>
{
    public GameObject overlayObject;


    /// <summary>
    /// Set overlay position and color
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public void Set(Vector3 position, Color color)
    {
        overlayObject.GetComponent<MeshRenderer>().sharedMaterial.color = color;
        transform.position = position;
        StartCoroutine(LerpAlpha());
    }


    /// <summary>
    ///  Lerp overlay alpha to 0
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpAlpha()
    {
        yield return new WaitForSeconds(0.4f);

        Material mat = overlayObject.GetComponent<MeshRenderer>().sharedMaterial;
        Color color = mat.color;
        float duration = .5f;
        float elapsed = 0f;
        float startAlpha = 1f;
        float targetAlpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            mat.color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        mat.color = new Color(color.r, color.g, color.b, targetAlpha);
    }

}
