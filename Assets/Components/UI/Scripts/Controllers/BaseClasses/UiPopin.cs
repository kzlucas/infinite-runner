using System;
using System.Collections;
using System.Collections.Generic;
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
        [HideInInspector] public VisualElement Popin;
        private List<Button> _buttons = new List<Button>();
        private List<Toggle> _toggles = new List<Toggle>();


        public override void OnDocReady()
        {
            StartCoroutine(_OnDocReady());
        }

        private IEnumerator _OnDocReady()
        {
            root.style.display = DisplayStyle.None;

            // Wait until doc is ready and get ref
            Popin = root.Q<VisualElement>("popin");
            root.Query<Button>(className: "action-button").ToList(_buttons);
            root.Query<Toggle>(className: "action-toggle").ToList(_toggles);
            
            // don't animate open/close on scene load
            Popin.RemoveFromClassList("animate");
            yield return new WaitForEndOfFrame();

            // Set initial state
            if (OpenOnSceneLoad) Open();
            else Close();

            // Wait frames to ensure open/close classes are applied before adding animate class, 
            yield return new WaitForEndOfFrame();
            Popin.AddToClassList("animate");
            root.style.display = DisplayStyle.Flex;

            // Callback
            yield return new WaitForEndOfFrame();
            WhenReady?.Invoke();
        }



        public void Open()
        {
            if (Popin == null)
            {
                WhenReady += () => { Open(); WhenReady = null; };
                return;
            }

            // Debug.Log("[UiPopin] Open popin: " + gameObject.name);
            Popin.AddToClassList("open");
            Popin.RemoveFromClassList("close");
            Popin.pickingMode = PickingMode.Position;
            IsOpen = true;
            foreach (var btn in _buttons) btn.focusable = true;
            foreach (var tgl in _toggles) tgl.focusable = true;
            OnOpen();
        }

        public virtual void OnOpen() { }

        public void Close()
        {
            if (Popin == null)
            {
                WhenReady += () => { Close(); WhenReady = null; };
                return;
            }

            // Debug.Log("[UiPopin] Close popin: " +  gameObject.name);
            Popin.AddToClassList("close");
            Popin.RemoveFromClassList("open");
            Popin.pickingMode = PickingMode.Ignore;
            IsOpen = false;
            foreach (var btn in _buttons) btn.focusable = false;
            foreach (var tgl in _toggles) tgl.focusable = false;
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