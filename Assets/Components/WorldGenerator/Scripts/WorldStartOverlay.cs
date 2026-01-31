using System.Collections;
using UnityEngine;

namespace WorldGenerator.Scripts
{

    /// <summary>
    ///  Overlay displayed at the start of the world
    /// </summary>
    public class WorldStartOverlay : MonoBehaviour
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
            Material mat = overlayObject.GetComponent<MeshRenderer>().sharedMaterial;
            Color color = mat.color;
            float duration = .5f;
            float elapsed = 0f;
            float startAlpha = .8f;
            float targetAlpha = 0f;

            mat.color = new Color(color.r, color.g, color.b, startAlpha);
            yield return new WaitForSeconds(0.4f);

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
}
