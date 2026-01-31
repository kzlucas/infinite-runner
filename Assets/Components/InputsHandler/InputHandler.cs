

using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace InputsHandler
{

    /// <summary>
    ///  Individual input handler
    /// </summary>
    [Serializable]
    public class InputHandler
    {
        // Unique label for the input handler
        public string Label;

        // Reference to the input action
        public InputActionReference ActionRef;

        // Current input value
        public Vector2 v2input = Vector2.zero;

        // Invoked when axis is not at it start position
        public Action<Vector2> OnInput;

        // Invoked every frame
        public Action<Vector2> OnUpdate;

        // Invoked when basic button pressed
        public Action OnTrigger;

        // Invoked when basic button released
        public Action OnRelease;

        // Invoked when basic button held
        public Action OnHold;

        // Flag indicating if the input is being held
        public bool IsHolding = false;



        public void Init(
            string _label
            , InputActionReference _actionRef
            , Action<Vector2> _OnInput = null
            , Action<Vector2> _OnUpdate = null
            , Action _OnTrigger = null
            , Action _OnRelease = null
            , Action _OnHold = null
        )
        {
            Label = _label;
            ActionRef = _actionRef;
            OnInput = _OnInput;
            OnUpdate = _OnUpdate;
            OnTrigger = _OnTrigger;
            OnRelease = _OnRelease;
            OnHold = _OnHold;

            ActionRef.action.performed += OnPerformed;
            ActionRef.action.canceled += OnCanceled;
            ActionRef.action.Enable();
        }

        void OnPerformed(InputAction.CallbackContext context)
        {
            IsHolding = true;

            if (ActionRef.action.type.ToString() == "Value")
                v2input = ActionRef.action.ReadValue<Vector2>();

            if (ActionRef.action.type.ToString() == "Button")
                OnTrigger?.Invoke();
        }


        void OnCanceled(InputAction.CallbackContext context)
        {
            IsHolding = false;
            v2input = Vector2.zero;
            OnRelease?.Invoke();
        }

        public void ClearSubscriptions()
        {
            OnInput = null;
            OnUpdate = null;
            OnTrigger = null;
            OnRelease = null;
            OnHold = null;

            if (ActionRef == null) return;

            ActionRef.action.performed -= OnPerformed;
            ActionRef.action.canceled -= OnCanceled;
            ActionRef.action.Disable();
        }

    }
}
