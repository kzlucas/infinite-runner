using StateMachine;

namespace Player.States
{
    public class RewindingState : BaseState
    {
        private Controller player;

        public RewindingState(StateMachine.StateMachine stateMachine, Controller player)
            : base(stateMachine, "Rewinding")
        {
            this.player = player;
        }

        public override void OnEnter()
        {
            player.StopParticles();
        }
    }

}