using Components.Events;
using Components.UI.Scripts.Events;
using UnityEngine;


namespace Components.UI.Scripts.Controllers
{
    [RequireComponent(typeof(Canvas))]
    public class UiCountdown : MonoBehaviour
    {
        private Canvas _canvas;
        public Animator Animator;
        public bool AnimationFinished = false;


        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            Animator = GetComponent<Animator>();
            AnimationFinished = true;
            _canvas.enabled = false;
        }

        public void Run()
        {
            Debug.Log("[UiCountdown] Countdown started");
            AnimationFinished = false;
            _canvas.enabled = true;
            Animator.SetTrigger("Run");
            EventBus.Publish(new CountdownStarted(Time.time));
        }


        public void CountdownFinished()
        {
            Debug.Log("[UiCountdown] Countdown finished");
            AnimationFinished = true;
            _canvas.enabled = false;
            EventBus.Publish(new CountdownFinished(Time.time));
        }
    }
}