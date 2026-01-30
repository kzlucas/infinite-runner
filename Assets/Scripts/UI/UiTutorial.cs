using System.Collections;
using Components.ServiceLocator.Scripts;
using Components.UI.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiTutorial : UiPopin
{
        
    [Header("Dependencies")]
    private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();


    public IEnumerator Start()
    {
        yield return new WaitUntil(() => docReady == true);
        var resumeButton = root.Q<Button>("btn--close-tuto");
        resumeButton.clicked += () =>  Close();
    }

    public override void OnOpen()
    {
        TimeScaleManager.Instance.PauseGame();
        UiRegistry.pauseMenu.isLocked = true;
    }

    public override void OnClose()
    {
        TimeScaleManager.Instance.ResumeGame();
        UiRegistry.pauseMenu.isLocked = false;
    }

}
