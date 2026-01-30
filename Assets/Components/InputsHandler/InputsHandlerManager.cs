using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;
using InputsHandler;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InputsHandler
{

    /// <summary>
    ///   Manager for multiple input handlers
    /// </summary>
    public class InputHandlersManager : Singleton<InputHandlersManager>, IService
    {
        public Vector2 mousePosition = Vector2.zero;
        [SerializeField] public static List<InputHandler> inputHandlers = new List<InputHandler>();



        private void Start()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnExitPlayMode;
#endif
        }

#if UNITY_EDITOR
        private static void OnExitPlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAllHandlers();
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


        private static void ClearAllHandlers()
        {
            Debug.Log("[InputHandlersManager] Clearing all input handlers");
            foreach (InputHandler ih in inputHandlers)
            {
                ih.ClearSubscriptions();
            }
            inputHandlers.Clear();
        }

        private void FixedUpdate()
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
}
