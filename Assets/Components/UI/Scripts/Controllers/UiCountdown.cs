using UnityEngine;


namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(Canvas))]
    public class UiCountdown : MonoBehaviour
    {
        private Canvas canvas;
        public Animator animator;
        public bool animationFinished = false;


        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            animator = GetComponent<Animator>();
            animationFinished = true;
            canvas.enabled = false;
        }

        public void Run()
        {
            animationFinished = false;
            canvas.enabled = true;
            animator.SetTrigger("Run");
            Player.Utils.Locate().Animator.speed = 0f;
        }


        public void CountdownFinished()
        {
            Debug.Log("[UiCountdown] Countdown finished");
            animationFinished = true;
            canvas.enabled = false;
            Player.Utils.Locate().Animator.speed = 1f;
        }
    }
}