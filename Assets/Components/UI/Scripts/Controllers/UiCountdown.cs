using Components.Events;
using Components.UI.Scripts.Events;
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
            Debug.Log("[UiCountdown] Countdown started");
            animationFinished = false;
            canvas.enabled = true;
            animator.SetTrigger("Run");
            EventBus.Publish(new CountdownStarted(Time.time));
        }


        public void CountdownFinished()
        {
            Debug.Log("[UiCountdown] Countdown finished");
            animationFinished = true;
            canvas.enabled = false;
            EventBus.Publish(new CountdownFinished(Time.time));
        }
    }
}