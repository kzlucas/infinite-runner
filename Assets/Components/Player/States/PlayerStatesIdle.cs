using StateMachine;

namespace Player.States
{
    public class IdleState : BaseState
    {
        private Controller player;

        public IdleState(StateMachine.StateMachine stateMachine, Controller player) 
            : base(stateMachine, "Idle")
        {
            this.player = player;
        }

        public override void OnEnter()
        {
        }

        public override void OnUpdate()
        {
        }

        public override void OnExit()
        {
        }
    }
    
}