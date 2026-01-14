using System.Collections;
using UnityEngine;

public class WorldStartOverlay : Singleton<WorldStartOverlay>
{
    private GameObject playerObject;
    public GameObject overlayObject;


    /// <summary>
    /// Initialize references after scene load
    /// and hide overlay
    /// </summary>
    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += () =>
        {
            playerObject = GameObject.FindWithTag("Player");
            Hide();
        };
    }

    /// <summary>
    /// Set overlay position and color
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    public void Set(Vector3 position, Color color)
    {
        overlayObject.GetComponent<MeshRenderer>().sharedMaterial.color = color;
        transform.position = position;
        Show();
        StartCoroutine(LerpAlpha());
    }

    public void Show()
    {
        overlayObject.SetActive(true);
    }

    public void Hide()
    {
        overlayObject.SetActive(false);
    }



    /// <summary>
    ///  Lerp overlay alpha to 0
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpAlpha()
    {
        yield return new WaitForSeconds(0.2f);

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
        Hide();
    }

}
