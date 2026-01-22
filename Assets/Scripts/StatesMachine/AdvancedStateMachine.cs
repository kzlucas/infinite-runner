using System;
using System.Collections.Generic;

namespace StateMachine
{
    /// <summary>
    /// Advanced state machine with transition rules and conditions
    /// </summary>
    public class AdvancedStateMachine : StateMachine
    {
        /// <summary>
        /// Dictionary of transition rules
        /// </summary>
        private Dictionary<Type, Dictionary<Type, Func<bool>>> transitions = new Dictionary<Type, Dictionary<Type, Func<bool>>>();
        
        /// <summary>
        /// Dictionary of global transition conditions that apply to all states
        /// </summary>
        private Dictionary<Type, Func<bool>> globalTransitions = new Dictionary<Type, Func<bool>>();
        
        /// <summary>
        /// Add a transition rule between two states
        /// </summary>
        /// <typeparam name="TFrom">Source state type</typeparam>
        /// <typeparam name="TTo">Target state type</typeparam>
        /// <param name="condition">Condition function that must return true for transition to occur</param>
        public void AddTransition<TFrom, TTo>(Func<bool> condition = null) 
            where TFrom : IState 
            where TTo : IState
        {
            Type fromType = typeof(TFrom);
            Type toType = typeof(TTo);
            
            if (!transitions.ContainsKey(fromType))
            {
                transitions[fromType] = new Dictionary<Type, Func<bool>>();
            }
            
            transitions[fromType][toType] = condition ?? (() => true);
        }
        
        /// <summary>
        /// Add a global transition that can be triggered from any state
        /// </summary>
        /// <typeparam name="TTo">Target state type</typeparam>
        /// <param name="condition">Condition function that must return true for transition to occur</param>
        public void AddGlobalTransition<TTo>(Func<bool> condition) where TTo : IState
        {
            globalTransitions[typeof(TTo)] = condition;
        }
        
        /// <summary>
        /// Remove a transition rule
        /// </summary>
        /// <typeparam name="TFrom">Source state type</typeparam>
        /// <typeparam name="TTo">Target state type</typeparam>
        public void RemoveTransition<TFrom, TTo>() 
            where TFrom : IState 
            where TTo : IState
        {
            Type fromType = typeof(TFrom);
            Type toType = typeof(TTo);
            
            if (transitions.ContainsKey(fromType))
            {
                transitions[fromType].Remove(toType);
                
                if (transitions[fromType].Count == 0)
                {
                    transitions.Remove(fromType);
                }
            }
        }
        
        /// <summary>
        /// Remove a global transition
        /// </summary>
        /// <typeparam name="TTo">Target state type</typeparam>
        public void RemoveGlobalTransition<TTo>() where TTo : IState
        {
            globalTransitions.Remove(typeof(TTo));
        }
        
        /// <summary>
        /// Check if a transition is allowed from current state to target state
        /// </summary>
        /// <typeparam name="TTo">Target state type</typeparam>
        /// <returns>True if transition is allowed</returns>
        public bool CanTransitionTo<TTo>() where TTo : IState
        {
            if (CurrentState == null) return false;
            
            Type currentStateType = CurrentState.GetType();
            Type targetStateType = typeof(TTo);
            
            // Check global transitions first
            if (globalTransitions.ContainsKey(targetStateType))
            {
                return globalTransitions[targetStateType]?.Invoke() ?? true;
            }
            
            // Check specific transitions
            if (transitions.ContainsKey(currentStateType) && 
                transitions[currentStateType].ContainsKey(targetStateType))
            {
                return transitions[currentStateType][targetStateType]?.Invoke() ?? true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Attempt to transition to a new state, respecting transition rules
        /// </summary>
        /// <typeparam name="T">Target state type</typeparam>
        /// <returns>True if transition was successful</returns>
        public bool TryTransitionTo<T>() where T : IState
        {
            if (CanTransitionTo<T>())
            {
                TransitionTo<T>();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Update method that also checks for automatic transitions
        /// </summary>
        public new void Update()
        {
            // Call base update
            base.Update();
            
            // Check for automatic transitions
            CheckAutomaticTransitions();
        }
        
        /// <summary>
        /// Check for automatic transitions based on conditions
        /// </summary>
        private void CheckAutomaticTransitions()
        {
            if (CurrentState == null) return;
            
            Type currentStateType = CurrentState.GetType();
            
            // Check global transitions first
            foreach (var globalTransition in globalTransitions)
            {
                if (globalTransition.Value?.Invoke() == true)
                {
                    var targetState = GetState(globalTransition.Key.Name);
                    if (targetState != null)
                    {
                        TransitionTo(targetState);
                        return;
                    }
                }
            }
            
            // Check specific transitions
            if (transitions.ContainsKey(currentStateType))
            {
                foreach (var transition in transitions[currentStateType])
                {
                    if (transition.Value?.Invoke() == true)
                    {
                        var targetState = GetState(transition.Key.Name);
                        if (targetState != null)
                        {
                            TransitionTo(targetState);
                            return;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Get all possible transitions from the current state
        /// </summary>
        /// <returns>Array of possible target state types</returns>
        public Type[] GetPossibleTransitions()
        {
            if (CurrentState == null) return new Type[0];
            
            Type currentStateType = CurrentState.GetType();
            var possibleTransitions = new List<Type>();
            
            // Add global transitions
            foreach (var globalTransition in globalTransitions)
            {
                if (globalTransition.Value?.Invoke() == true)
                {
                    possibleTransitions.Add(globalTransition.Key);
                }
            }
            
            // Add specific transitions
            if (transitions.ContainsKey(currentStateType))
            {
                foreach (var transition in transitions[currentStateType])
                {
                    if (transition.Value?.Invoke() == true)
                    {
                        possibleTransitions.Add(transition.Key);
                    }
                }
            }
            
            return possibleTransitions.ToArray();
        }
        
        /// <summary>
        /// Clear all transition rules
        /// </summary>
        public void ClearTransitions()
        {
            transitions.Clear();
            globalTransitions.Clear();
        }
        
        /// <summary>
        /// Clear all states and transitions
        /// </summary>
        public new void Clear()
        {
            base.Clear();
            ClearTransitions();
        }
    }
}