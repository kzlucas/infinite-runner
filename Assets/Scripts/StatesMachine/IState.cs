namespace StateMachine
{
    /// <summary>
    /// Interface defining the basic state contract
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Called when entering this state
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        void OnUpdate();
        
        /// <summary>
        /// Called when exiting this state
        /// </summary>
        void OnExit();
    }
}