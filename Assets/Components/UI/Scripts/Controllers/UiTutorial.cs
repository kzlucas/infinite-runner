using System.Collections;
using Components.TimeScale;
using UnityEngine;
using UnityEngine.UIElements;

namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(UIDocument))]
    public class UiTutorial : BaseClasses.UiPopin
    {

        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Scripts.ServiceLocator.Get<UiRegistry>();


        public IEnumerator Start()
        {
            yield return new WaitUntil(() => docReady == true);
            var resumeButton = root.Q<Button>("btn--close-tuto");
            resumeButton.clicked += () => Close();
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
}
