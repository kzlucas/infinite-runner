using System.Collections;
using Components.TimeScale;
using UnityEngine;
using UnityEngine.UIElements;

namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiTutorial : BaseClasses.UiPopin
    {
        public IEnumerator Start()
        {
            yield return new WaitUntil(() => DocReady == true);
            var resumeButton = root.Q<Button>("btn--close-tuto");
            resumeButton.clicked += () => Close();
        }

        public override void OnOpen()
        {
            TimeScaleManager.Instance.PauseGame();
            UiRegistry.Instance.PauseMenu.IsLocked = true;
        }

        public override void OnClose()
        {
            TimeScaleManager.Instance.ResumeGame();
            UiRegistry.Instance.PauseMenu.IsLocked = false;
        }

    }
}
