using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiScreenOverlay : UiController, IOpenable
{
    public string colorString = "black";
    public bool isOpen { get; set; } = true;
    public bool fadeOutOnStart = true;


    private IEnumerator Start()
    {
        
        if(!fadeOutOnStart) yield break;
        SceneLoader.Instance.OnSceneLoaded += OnSceneLoaded;
        OnSceneLoaded();
    }


    /// <summary>
    ///   When a new scene has been loaded, initialize the fade screen to default state then close it.
    /// </summary>
    /// <returns></returns>
    public void OnSceneLoaded()
    {
        StartCoroutine(_OnSceneLoaded());
    }
    private IEnumerator _OnSceneLoaded()
    {
        yield return new WaitUntil(() => docReady);
        var screen = root.Q<VisualElement>("screen");
        screen.AddToClassList(colorString);
        screen.RemoveFromClassList("fade-out");
        screen.RemoveFromClassList("fade-in");
        screen.MarkDirtyRepaint();
        yield return new WaitForEndOfFrame();
        Close();
    }

    public void Open()
    {
        Debug.Log("[UiScreenOverlay] Opening overlay.");
        var screen = root.Q<VisualElement>("screen");
        screen.AddToClassList(colorString);
        screen.AddToClassList("fade-in");
        screen.RemoveFromClassList("fade-out");
        isOpen = true;
    }

    public void Close()
    {
        Debug.Log("[UiScreenOverlay] Closing overlay.");
        var screen = root.Q<VisualElement>("screen");
        screen.AddToClassList(colorString);
        screen.AddToClassList("fade-out");
        screen.RemoveFromClassList("fade-in");
        isOpen = false;
    }

}