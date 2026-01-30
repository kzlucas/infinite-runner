

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
        public string label;

        // Reference to the input action
        public InputActionReference actionRef;

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
        public bool isHolding = false;



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
            label = _label;
            actionRef = _actionRef;
            OnInput = _OnInput;
            OnUpdate = _OnUpdate;
            OnTrigger = _OnTrigger;
            OnRelease = _OnRelease;
            OnHold = _OnHold;

            actionRef.action.performed += OnPerformed;
            actionRef.action.canceled += OnCanceled;
            actionRef.action.Enable();
        }

        void OnPerformed(InputAction.CallbackContext context)
        {
            isHolding = true;

            if (actionRef.action.type.ToString() == "Value")
                v2input = actionRef.action.ReadValue<Vector2>();

            if (actionRef.action.type.ToString() == "Button")
                OnTrigger?.Invoke();
        }


        void OnCanceled(InputAction.CallbackContext context)
        {
            isHolding = false;
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

            if (actionRef == null) return;

            actionRef.action.performed -= OnPerformed;
            actionRef.action.canceled -= OnCanceled;
            actionRef.action.Disable();
        }

    }
}
