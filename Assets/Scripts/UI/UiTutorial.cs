using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiTutorial : UiPopin
{
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => docReady == true);
        var resumeButton = root.Q<Button>("btn--close-tuto");
        resumeButton.clicked += () =>  Close();
    }

    public override void OnOpen()
    {
        GameManager.Instance.PauseGame();
        UiManager.Instance.pauseMenu.isLocked = true;
    }

    public override void OnClose()
    {
        GameManager.Instance.ResumeGame();
        UiManager.Instance.pauseMenu.isLocked = false;
    }

}
