using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
///   Manager for multiple input handlers
/// </summary>
public class InputHandlersManager : Singleton<InputHandlersManager>
{
    public Vector2 mousePosition = Vector2.zero;
    [SerializeField] public List<InputHandler> inputHandlers = new List<InputHandler>();



    private void Start()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
    }
    
#if UNITY_EDITOR
    private static void OnExitPlayMode(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingPlayMode)
        {
            Instance.ClearAllHandlers();
            EditorApplication.playModeStateChanged -= OnExitPlayMode;
        }
    }
#endif

    public void Register(
        string label
        , InputActionReference actionRef
        , Action<Vector2> OnInput = null
        , Action<Vector2> OnUpdate = null
        , Action OnTrigger = null
        , Action OnRelease = null
        , Action OnHold = null
    )
    {
        if (inputHandlers.Exists(ih => ih.label == label))
        {
            // clear previous subscriptions
            Unregister(label);
        }

        // create new handler
        InputHandler newInputHandler = new InputHandler();
        newInputHandler.Init(
            label
            , actionRef
            , OnInput
            , OnUpdate
            , OnTrigger
            , OnRelease
            , OnHold
        );
        inputHandlers.Add(newInputHandler);
    }


    public void Unregister(string label)
    {
        InputHandler ih = inputHandlers.Find(ih => ih.label == label);
        if (ih != null)
        {
            ih.ClearSubscriptions();
            inputHandlers.Remove(ih);
        }
    }


    void ClearAllHandlers()
    {
        Debug.Log("[InputHandlersManager] Clearing all input handlers");
        foreach (InputHandler ih in inputHandlers)
        {
            ih.ClearSubscriptions();
        }
        inputHandlers.Clear();
    }

    void FixedUpdate()
    {
        foreach (InputHandler ih in inputHandlers)
        {
            if (ih.v2input != Vector2.zero)
            {
                ih.OnInput?.Invoke(ih.v2input.normalized);
            }
            ih.OnUpdate?.Invoke(ih.v2input.normalized);

            if (ih.isHolding)
            {
                ih.OnHold?.Invoke();
            }
        }

        mousePosition = Mouse.current.position.ReadValue();
    }
}


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

Debug.Log("[InputHandler] Initializing handler for: " + label);

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
