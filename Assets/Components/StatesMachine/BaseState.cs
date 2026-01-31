using System;

namespace StateMachine
{
    /// <summary>
    /// Abstract base class for states providing common functionality
    /// </summary>
    public abstract class BaseState : IState
    {
        /// <summary>
        /// Reference to the state machine that owns this state
        /// </summary>
        protected StateMachine stateMachine;
        
        /// <summary>
        /// Name identifier for this state
        /// </summary>
        public string StateName { get; protected set; }
        
        /// <summary>
        /// Constructor for BaseState
        /// </summary>
        /// <param name="stateMachine">The state machine that owns this state</param>
        /// <param name="stateName">Optional name for this state</param>
        public BaseState(StateMachine stateMachine, string stateName = null)
        {
            this.stateMachine = stateMachine;
            this.StateName = stateName ?? GetType().Name;
        }
        
        /// <summary>
        /// Called when entering this state
        /// </summary>
        public virtual void OnEnter() { }
        
        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        public virtual void OnUpdate() { }
        
        /// <summary>
        /// Called when exiting this state
        /// </summary>
        public virtual void OnExit() { }
        
        /// <summary>
        /// Helper method to transition to another state
        /// </summary>
        /// <param name="newState">The state to transition to</param>
        protected void TransitionTo(IState newState)
        {
            stateMachine.TransitionTo(newState);
        }
        
        /// <summary>
        /// Helper method to transition to another state by type
        /// </summary>
        /// <typeparam name="T">The state type to transition to</typeparam>
        protected void TransitionTo<T>() where T : IState
        {
            stateMachine.TransitionTo<T>();
        }
    }
}