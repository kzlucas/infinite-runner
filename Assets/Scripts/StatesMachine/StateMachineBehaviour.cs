using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// MonoBehaviour wrapper for the StateMachine that integrates with Unity's lifecycle
    /// </summary>
    public class Behaviour : MonoBehaviour
    {
        /// <summary>
        /// The internal state machine instance
        /// </summary>
        [SerializeField] private StateMachine stateMachine;
        
        /// <summary>
        /// Whether to automatically update the state machine in Update()
        /// </summary>
        [SerializeField] private bool autoUpdate = true;
        
        /// <summary>
        /// Whether to show debug information in the console
        /// </summary>
        [SerializeField] private bool debugMode = false;
        
        /// <summary>
        /// Access to the internal state machine
        /// </summary>
        public StateMachine StateMachine
        {
            get
            {
                if (stateMachine == null)
                {
                    stateMachine = new StateMachine();
                    
                    // Subscribe to state transitions for debugging
                    if (debugMode)
                    {
                        stateMachine.OnStateTransition += OnStateTransitionDebug;
                    }
                }
                return stateMachine;
            }
        }
        
        /// <summary>
        /// Get the current state name for inspector display
        /// </summary>
        public string CurrentStateName => StateMachine.CurrentStateName;
        
        protected virtual void Awake()
        {
            // Initialize the state machine
            _ = StateMachine;
        }
        
        protected virtual void Update()
        {
            if (autoUpdate)
            {
                StateMachine.Update();
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (stateMachine != null)
            {
                stateMachine.OnStateTransition -= OnStateTransitionDebug;
                stateMachine.Stop();
            }
        }
        
        /// <summary>
        /// Register a state with the state machine
        /// </summary>
        /// <param name="state">The state to register</param>
        public void RegisterState(IState state)
        {
            StateMachine.RegisterState(state);
        }
        
        /// <summary>
        /// Register multiple states at once
        /// </summary>
        /// <param name="states">Array of states to register</param>
        public void RegisterStates(params IState[] states)
        {
            StateMachine.RegisterStates(states);
        }
        
        /// <summary>
        /// Start the state machine with an initial state
        /// </summary>
        /// <param name="initialState">The initial state</param>
        public void StartStateMachine(IState initialState)
        {
            StateMachine.Start(initialState);
        }
        
        /// <summary>
        /// Start the state machine with an initial state by type
        /// </summary>
        /// <typeparam name="T">The state type</typeparam>
        public void StartStateMachine<T>() where T : IState
        {
            StateMachine.Start<T>();
        }
        
        /// <summary>
        /// Start the state machine with an initial state by name
        /// </summary>
        /// <param name="stateName">The state name</param>
        public void StartStateMachine(string stateName)
        {
            StateMachine.Start(stateName);
        }
        
        /// <summary>
        /// Manually update the state machine (useful when autoUpdate is false)
        /// </summary>
        public void UpdateStateMachine()
        {
            StateMachine.Update();
        }
        
        /// <summary>
        /// Transition to a new state
        /// </summary>
        /// <param name="newState">The state to transition to</param>
        public void TransitionTo(IState newState)
        {
            StateMachine.TransitionTo(newState);
        }
        
        /// <summary>
        /// Transition to a new state by type
        /// </summary>
        /// <typeparam name="T">The state type</typeparam>
        public void TransitionTo<T>() where T : IState
        {
            StateMachine.TransitionTo<T>();
        }
        
        /// <summary>
        /// Transition to a new state by name
        /// </summary>
        /// <param name="stateName">The state name</param>
        public void TransitionTo(string stateName)
        {
            StateMachine.TransitionTo(stateName);
        }
        
        /// <summary>
        /// Stop the state machine
        /// </summary>
        public void StopStateMachine()
        {
            StateMachine.Stop();
        }
        
        /// <summary>
        /// Debug callback for state transitions
        /// </summary>
        /// <param name="fromState">Previous state</param>
        /// <param name="toState">New state</param>
        private void OnStateTransitionDebug(IState fromState, IState toState)
        {
            string fromName = fromState?.GetType().Name ?? "None";
            string toName = toState?.GetType().Name ?? "None";
            
            if (debugMode)
            {
                Debug.Log($"[{gameObject.name}] State Machine: {fromName} -> {toName}", this);
            }
        }
        
        /// <summary>
        /// Editor-only method to display current state in inspector
        /// </summary>
        #if UNITY_EDITOR
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying && stateMachine != null)
            {
                // This will show the current state in the scene view when selected
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, 
                    $"Current State: {CurrentStateName}");
            }
        }
        #endif
    }
}