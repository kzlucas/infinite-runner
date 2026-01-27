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

    private void OnDestroy() => StopAllCoroutines();



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
        screen.RemoveFromClassList("white");
        screen.RemoveFromClassList("fade-out");
        screen.RemoveFromClassList("fade-in");
        screen.MarkDirtyRepaint();
        yield return new WaitForEndOfFrame();
        Close();
    }

    public void Open()
    {
        var screen = root.Q<VisualElement>("screen");
        screen.AddToClassList(colorString);
        screen.AddToClassList("fade-in");
        screen.RemoveFromClassList("fade-out");
        isOpen = true;
    }

    public void Close()
    {
        var screen = root.Q<VisualElement>("screen");
        screen.AddToClassList(colorString);
        screen.AddToClassList("fade-out");
        screen.RemoveFromClassList("fade-in");
        isOpen = false;
    }


    public void Flash(string color, float duration = 0.1f)
    {
        StartCoroutine(_Flash(color, duration));
    }
    private IEnumerator _Flash(string color, float fadesDuration = .1f, float pauseDuration = 0f)
    {
        var screen = root.Q<VisualElement>("screen");
        screen.RemoveFromClassList(colorString); 
        screen.AddToClassList(color);
        screen.AddToClassList("fade-in-fast");
        screen.RemoveFromClassList("fade-out-fast");
        yield return new WaitForSecondsRealtime(fadesDuration + pauseDuration);
        screen.AddToClassList("fade-out-fast");
        screen.RemoveFromClassList("fade-in-fast");
        yield return new WaitForSecondsRealtime(fadesDuration);
        screen.RemoveFromClassList(color);
        screen.AddToClassList(colorString);
    }
}