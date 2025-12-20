using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class UiPauseMenu : UiPopin
{
    public override void OnOpen()
    {
        GameManager.Instance.PauseGame();
    }

    public override void OnClose()
    {
        GameManager.Instance.ResumeGame();
    }

}