using UnityEngine;
using StateMachine;

namespace Player.States
{
    public class CrashState : BaseState
    {
        private Controller player;

        public CrashState(StateMachine.StateMachine stateMachine, Controller player) 
            : base(stateMachine, "Crash")
        {
            this.player = player;
        }

        public override void OnEnter()
        {
            // Play crash effects
            player.jumpParticles.Stop();
            player.slideParticles.Stop();
            player.laneChangeParticles.Stop();
            player.crashParticules.transform.parent = null;
            player.crashParticules.transform.position = player.transform.position + Vector3.up * 0.5f;
            player.crashParticules.Play();
            AudioManager.Instance.PlaySound("crash");

            // release control
            player.controlReleased = true;

            // freeze camera
            Camera.main.GetComponent<CameraBehaviors>().FreezeCamera();

            // unfreeze rotation
            player.rb.freezeRotation = false;

            // Apply random torque on crash
            float crashNgFactor = 10f;
            player.rb.angularVelocity = new Vector3(Random.Range(-crashNgFactor, crashNgFactor), UnityEngine.Random.Range(-crashNgFactor, crashNgFactor), UnityEngine.Random.Range(-crashNgFactor, crashNgFactor));

            // Apply an upward force on crash
            player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, 5f, 5f);

            // Stop animations
            player.transform.Find("Renderer").GetComponent<Animator>().enabled = false;
        }
    }
    
}