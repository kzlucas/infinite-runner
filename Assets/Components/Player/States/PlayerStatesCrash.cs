using UnityEngine;
using StateMachine;
using Components.Player;

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
            player.JumpParticles.Stop();
            player.SlideParticles.Stop();
            player.LaneChangeParticles.Stop();
            player.CrashParticules.transform.parent = null;
            player.CrashParticules.transform.position = player.transform.position + Vector3.up * 0.5f;
            player.CrashParticules.Play();

            // release control
            player.ControlReleased = true;

            // freeze camera
            Camera.main.GetComponent<Components.Camera.Behaviors>().FreezeCamera();

            // unfreeze rotation
            player.Rb.freezeRotation = false;

            // Apply random torque on crash
            float crashNgFactor = 10f;
            player.Rb.angularVelocity = new Vector3(Random.Range(-crashNgFactor, crashNgFactor), UnityEngine.Random.Range(-crashNgFactor, crashNgFactor), UnityEngine.Random.Range(-crashNgFactor, crashNgFactor));

            // Apply an upward force on crash
            player.Rb.linearVelocity = new Vector3(player.Rb.linearVelocity.x, 5f, 5f);

            // Stop animations
            player.transform.Find("Renderer").GetComponent<Animator>().enabled = false;
        }
    }
    
}