using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiPopin : UiController, IOpenable
{
    public bool isOpen { get; set; } = false;
    public bool openOnSceneLoad = false;
    private VisualElement popin;



    private void Start()
    {
        Debug.Log("[UiPopin] Start..");
        SceneLoader.Instance.OnSceneLoaded += () => _ = StartCoroutine(OnSceneLoaded());
    }

    public override void OnDocReady()
    {
        Debug.Log("[UiPopin] Document is ready.");
        StartCoroutine(OnSceneLoaded());
    }

    private IEnumerator OnSceneLoaded()
    {
        Debug.Log("[UiPopin] OnSceneLoaded..");
        yield return new WaitUntil(() => docReady);
        popin = root.Q<VisualElement>("popin");

        if (openOnSceneLoad)
            Open();

        else
            Close();


        // Wait frames to ensure open/close classes are applied before adding animate class, 
        yield return new WaitForEndOfFrame(); 
        
        popin.AddToClassList("animate");
    }



    public void Open()
    {
        Debug.Log("[UiPopin] Opening popin.");
        popin.AddToClassList("open");
        popin.RemoveFromClassList("close");
        isOpen = true;
    }

    public void Close()
    {
        Debug.Log("[UiPopin] Closing popin.");
        popin.AddToClassList("close");
        popin.RemoveFromClassList("open");
        isOpen = false;
    }

    public void Toggle()
    {
        if (isOpen)
            Close();
        else
            Open();
    }


}