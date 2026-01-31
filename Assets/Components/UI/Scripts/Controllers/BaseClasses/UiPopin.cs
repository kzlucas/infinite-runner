using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Components.UI.Scripts.Controllers.BaseClasses
{
    [RequireComponent(typeof(UIDocument))]
    public class UiPopin : BaseClasses.UiController, IOpenable
    {
        private Action WhenReady { get; set; }
        public bool IsOpen { get; set; } = false;
        public bool OpenOnSceneLoad = false;
        [HideInInspector] public VisualElement popin;


        public override void OnDocReady()
        {
            StartCoroutine(_OnDocReady());
        }

        private IEnumerator _OnDocReady()
        {
            root.style.display = DisplayStyle.None;

            // Wait until doc is ready and get ref
            popin = root.Q<VisualElement>("popin");

            // don't animate open/close on scene load
            popin.RemoveFromClassList("animate");
            yield return new WaitForEndOfFrame();

            // Set initial state
            if (OpenOnSceneLoad) Open();
            else Close();

            // Wait frames to ensure open/close classes are applied before adding animate class, 
            yield return new WaitForEndOfFrame();
            popin.AddToClassList("animate");
            root.style.display = DisplayStyle.Flex;

            // Callback
            yield return new WaitForEndOfFrame();
            WhenReady?.Invoke();
        }



        public void Open()
        {
            if (popin == null)
            {
                WhenReady += () => { Open(); WhenReady = null; };
                return;
            }

            // Debug.Log("[UiPopin] Open popin: " + gameObject.name);
            popin.AddToClassList("open");
            popin.RemoveFromClassList("close");
            popin.pickingMode = PickingMode.Position;
            IsOpen = true;
            OnOpen();
        }

        public virtual void OnOpen() { }

        public void Close()
        {
            if (popin == null)
            {
                WhenReady += () => { Close(); WhenReady = null; };
                return;
            }

            // Debug.Log("[UiPopin] Close popin: " +  gameObject.name);
            popin.AddToClassList("close");
            popin.RemoveFromClassList("open");
            popin.pickingMode = PickingMode.Ignore;
            IsOpen = false;
            OnClose();
        }

        public virtual void OnClose() { }

        public virtual void Toggle()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }


    }
}