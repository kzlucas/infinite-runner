using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Core state machine class that manages state transitions and execution
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// Dictionary of all registered states by their type
        /// </summary>
        private Dictionary<Type, IState> states = new Dictionary<Type, IState>();
        
        /// <summary>
        /// Dictionary of states by their name
        /// </summary>
        private Dictionary<string, IState> statesByName = new Dictionary<string, IState>();
        
        /// <summary>
        /// Currently active state
        /// </summary>
        private IState currentState;
        
        /// <summary>
        /// Event triggered when a state transition occurs
        /// </summary>
        public event Action<IState, IState> OnStateTransition;
        
        /// <summary>
        /// Get the currently active state
        /// </summary>
        public IState CurrentState => currentState;
        
        /// <summary>
        /// Get the name of the currently active state
        /// </summary>
        public string CurrentStateName => GetStateName(currentState);
        
        /// <summary>
        /// Register a state instance with the state machine
        /// </summary>
        /// <param name="state">The state instance to register</param>
        public void RegisterState(IState state)
        {
            Type stateType = state.GetType();
            
            if (states.ContainsKey(stateType))
            {
                Debug.LogWarning($"State of type {stateType.Name} is already registered. Replacing existing state.");
            }
            
            states[stateType] = state;
            
            // Register by name if it's a BaseState
            if (state is BaseState baseState)
            {
                statesByName[baseState.StateName] = state;
            }
            else
            {
                statesByName[stateType.Name] = state;
            }
        }
        
        /// <summary>
        /// Register multiple states at once
        /// </summary>
        /// <param name="statesToRegister">Array of states to register</param>
        public void RegisterStates(params IState[] statesToRegister)
        {
            foreach (var state in statesToRegister)
            {
                RegisterState(state);
            }
        }
        
        /// <summary>
        /// Start the state machine with an initial state
        /// </summary>
        /// <param name="initialState">The initial state to start with</param>
        public void Start(IState initialState)
        {
            currentState = initialState;
            currentState?.OnEnter();
        }
        
        /// <summary>
        /// Start the state machine with an initial state by type
        /// </summary>
        /// <typeparam name="T">The state type to start with</typeparam>
        public void Start<T>() where T : IState
        {
            if (states.TryGetValue(typeof(T), out IState state))
            {
                Start(state);
            }
            else
            {
                Debug.LogError($"State of type {typeof(T).Name} is not registered.");
            }
        }
        
        /// <summary>
        /// Start the state machine with an initial state by name
        /// </summary>
        /// <param name="stateName">The name of the state to start with</param>
        public void Start(string stateName)
        {
            if (statesByName.TryGetValue(stateName, out IState state))
            {
                Start(state);
            }
            else
            {
                Debug.LogError($"State with name '{stateName}' is not registered.");
            }
        }
        
        /// <summary>
        /// Update the current state (should be called every frame)
        /// </summary>
        public void Update()
        {
            currentState?.OnUpdate();
        }
        
        /// <summary>
        /// Transition to a new state
        /// </summary>
        /// <param name="newState">The state to transition to</param>
        public void TransitionTo(IState newState)
        {
            if (newState == null)
            {
                Debug.LogError("Cannot transition to null state.");
                return;
            }
            
            IState previousState = currentState;
            
            // Exit current state
            currentState?.OnExit();
            
            // Set new state
            currentState = newState;
            
            // Enter new state
            currentState.OnEnter();
            
            // Trigger transition event
            OnStateTransition?.Invoke(previousState, currentState);
            
            // Debug.Log($"State transition: {GetStateName(previousState)} -> {GetStateName(currentState)}");
        }
        
        /// <summary>
        /// Transition to a new state by type
        /// </summary>
        /// <typeparam name="T">The state type to transition to</typeparam>
        public void TransitionTo<T>() where T : IState
        {
            if (states.TryGetValue(typeof(T), out IState state))
            {
                TransitionTo(state);
            }
            else
            {
                Debug.LogError($"State of type {typeof(T).Name} is not registered.");
            }
        }
        
        /// <summary>
        /// Transition to a new state by name
        /// </summary>
        /// <param name="stateName">The name of the state to transition to</param>
        public void TransitionTo(string stateName)
        {
            if (statesByName.TryGetValue(stateName, out IState state))
            {
                TransitionTo(state);
            }
            else
            {
                Debug.LogError($"State with name '{stateName}' is not registered.");
            }
        }
        
        /// <summary>
        /// Check if a state is registered
        /// </summary>
        /// <typeparam name="T">The state type to check</typeparam>
        /// <returns>True if the state is registered</returns>
        public bool HasState<T>() where T : IState
        {
            return states.ContainsKey(typeof(T));
        }
        
        /// <summary>
        /// Check if a state is registered by name
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if the state is registered</returns>
        public bool HasState(string stateName)
        {
            return statesByName.ContainsKey(stateName);
        }
        
        /// <summary>
        /// Get a registered state by type
        /// </summary>
        /// <typeparam name="T">The state type to get</typeparam>
        /// <returns>The state instance or null if not found</returns>
        public T GetState<T>() where T : class, IState
        {
            if (states.TryGetValue(typeof(T), out IState state))
            {
                return state as T;
            }
            return null;
        }
        
        /// <summary>
        /// Get a registered state by name
        /// </summary>
        /// <param name="stateName">The name of the state to get</param>
        /// <returns>The state instance or null if not found</returns>
        public IState GetState(string stateName)
        {
            statesByName.TryGetValue(stateName, out IState state);
            return state;
        }
        
        /// <summary>
        /// Check if currently in a specific state type
        /// </summary>
        /// <typeparam name="T">The state type to check</typeparam>
        /// <returns>True if currently in the specified state</returns>
        public bool IsInState<T>() where T : IState
        {
            return currentState?.GetType() == typeof(T);
        }
        
        /// <summary>
        /// Check if currently in a specific state by name
        /// </summary>
        /// <param name="stateName">The name of the state to check</param>
        /// <returns>True if currently in the specified state</returns>
        public bool IsInState(string stateName)
        {
            return GetStateName(currentState) == stateName;
        }
        
        /// <summary>
        /// Stop the state machine
        /// </summary>
        public void Stop()
        {
            currentState?.OnExit();
            currentState = null;
        }
        
        /// <summary>
        /// Clear all registered states
        /// </summary>
        public void Clear()
        {
            Stop();
            states.Clear();
            statesByName.Clear();
        }
        
        /// <summary>
        /// Get all registered state names
        /// </summary>
        /// <returns>Array of state names</returns>
        public string[] GetRegisteredStateNames()
        {
            var names = new string[statesByName.Count];
            statesByName.Keys.CopyTo(names, 0);
            return names;
        }
        
        /// <summary>
        /// Helper method to get state name safely
        /// </summary>
        /// <param name="state">The state to get name for</param>
        /// <returns>State name or "None" if state is null</returns>
        private string GetStateName(IState state)
        {
            if (state == null) return "None";
            
            if (state is BaseState baseState)
            {
                return baseState.StateName;
            }
            
            return state.GetType().Name;
        }
    }
}