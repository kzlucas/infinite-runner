using System.Collections;
using Components.EndGame.Scripts;
using Components.Events;
using Components.Player.Events;
using Components.Scenes;
using Components.ServiceLocator.Scripts;
using Components.Stats;
using Components.UI.Scripts;
using Components.UI.Scripts.Events;
using InputsHandler;
using Player.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
namespace Components.Player
{
    public class Controller : StateMachine.Behaviour
    {
        #region Fields

        
        [Header("Dependencies")]
        private EndGameManager EndGameManager => ServiceLocator.Scripts.ServiceLocator.Get<EndGameManager>();
        private InputHandlersManager InputHandlersManager => ServiceLocator.Scripts.ServiceLocator.Get<InputHandlersManager>();
        private PlayerHistory History => ServiceLocator.Scripts.ServiceLocator.Get<PlayerHistory>();


        [Header("References")]
        public readonly StateMachine.StateMachine sm = new();
        public Health Health;
        public Animator Animator;


        [Header("Collider")]

        /// <summary> Reference to the collision handling component</summary>
        public CollisionHandling CollisionHandler;

        /// <summary> Reference to the player's rigidbody</summary>
        public Rigidbody Rb;



        [Header("Flags")]

        /// <summary> Indicates whether player control has been released (e.g., after a crash)</summary>
        public bool ControlReleased = false;



        [Header("X Movement Settings")]

        /// <summary> X movement speed of the player</summary>
        public float XMoveSpeed = 15f;

        /// <summary> Forward movement speed of the player</summary>
        public float ZMoveSpeed = 15f;

        /// <summary> Maximum allowed X position</summary>
        public int MaxX = 4;

        /// <summary> Minimum allowed X position</summary>
        public int MinX = -4;

        /// <summary> Particle system effect run when changing lane</summary>
        public ParticleSystem LaneChangeParticles;

        /// <summary>Is the player currently changing lane flag</summary>
        public bool IsChangingLane = false;



        [Header("Jump Settings")]

        /// <summary> Is the player currently jumping</summary>
        public bool IsGrounded = false;

        /// <summary> Height of the jump</summary>
        public float JumpHeight = 2f;

        /// <summary> Particle system for jump effect</summary>
        public ParticleSystem JumpParticles;



        [Header("Slide Settings")]

        /// <summary> Is the player currently sliding</summary>
        public bool IsSliding = false;

        /// <summary> Particle system for slide effect</summary>
        public ParticleSystem SlideParticles;

        /// <summary> Slide coroutine</summary>
        public IEnumerator SlideRoutine;



        [Header("Crash Settings")]

        /// <summary> Particle system for crash effect</summary>
        public ParticleSystem CrashParticules;



        [Header("Input Action References")]

        /// <summary> Reference to the input action for movements</summary>
        public InputActionReference MoveActionRef;

        /// <summary> Reference to the input action for jump</summary>
        public InputActionReference JumpActionRef;

        /// <summary> Reference to the input action for slide</summary>
        public InputActionReference SlideActionRef;


        #endregion


        /// <summary>
        ///   Init component and Subscribe to input events
        /// </summary>
        private IEnumerator Start()
        {
            OnCountdownStarted(null);

            yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized);
            
            Debug.Log("[PlayerController] Initializing Player Controller");
            Components.Events.EventBus.Subscribe<Landed>(evt => sm.TransitionTo<LandState>());
            Components.Events.EventBus.Subscribe<CountdownStarted>(OnCountdownStarted);
            Components.Events.EventBus.Subscribe<CountdownFinished>(OnCountdownFinished);
            UiRegistry.Instance.Countdown.Run();

            /*
             *
             * Build States Machine 
             */

            // Register states
            sm.RegisterState(new IdleState(StateMachine, this));
            sm.RegisterState(new MoveState(StateMachine, this));
            sm.RegisterState(new JumpState(StateMachine, this));
            sm.RegisterState(new SlideState(StateMachine, this));
            sm.RegisterState(new CrashState(StateMachine, this));
            sm.RegisterState(new LandState(StateMachine, this));
            sm.RegisterState(new RewindingState(StateMachine, this));

            // Start with idle
            sm.Start<IdleState>();


            /*
             *
             * Map input events to state transitions 
             */

            InputHandlersManager.Register(
                "Jump"
                , JumpActionRef
                , OnHold: () => { if (CanJump()) sm.TransitionTo<JumpState>(); }
            );

            InputHandlersManager.Register(
                "Move"
                , MoveActionRef
                , OnUpdate: (v2) => { if (CanMove(v2)) { sm.GetState<MoveState>().inputMoveDir = v2; sm.TransitionTo<MoveState>(); } }
                , OnRelease: () => { sm.GetState<MoveState>().OnRelease(); }
            );

            InputHandlersManager.Register(
                "Slide"
                , SlideActionRef
                , OnTrigger: () => { if (CanSlide()) sm.TransitionTo<SlideState>(); }
                , OnHold: () => { if (CanSlide()) sm.TransitionTo<SlideState>(); }
                , OnRelease: () => { sm.GetState<SlideState>().OnRelease(); }
            );

        }


        public override void OnStateMachineDestroyDelegate()
        {
            Components.Events.EventBus.Unsubscribe<Landed>(evt => sm.TransitionTo<LandState>());
            Components.Events.EventBus.Unsubscribe<CountdownStarted>(OnCountdownStarted);
            Components.Events.EventBus.Unsubscribe<CountdownFinished>(OnCountdownFinished);
            Utils.PlayerController = null;
        }

        private void OnCountdownStarted(CountdownStarted e)
        {
            Animator.speed = 0f;
            ControlReleased = true;
            StopParticles();
            Rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
        private void OnCountdownFinished(CountdownFinished e)
        {
            Animator.speed = 1f;
            ControlReleased = false;
            Rb.constraints = RigidbodyConstraints.FreezeRotation;
        }


        /// <summary>
        ///  Check if the player can jump
        /// </summary>
        private bool CanJump()
        {
            return !(
                (this == null)
                || ControlReleased
                || (Time.timeScale == 0f)
                || IsSliding
                || (!IsGrounded)
            );
        }


        /// <summary>
        /// Check if the player can move
        /// </summary>
        private bool CanMove(Vector2 dir)
        {
            return !(
                (this == null)
                || ControlReleased
                || IsSliding
                || (Time.timeScale == 0f)
                || (dir.x == 0)
            );
        }


        /// <summary>
        /// Check if the player can slide
        /// </summary>
        private bool CanSlide()
        {
            return !(
                (this == null)
                || ControlReleased
                || IsSliding
                || (Time.timeScale == 0f)
                || (!IsGrounded)
            );
        }


        /// <summary>
        /// Stop all particle effects
        /// </summary>
        public void StopParticles()
        {
            LaneChangeParticles.Stop();
            JumpParticles.Stop();
            SlideParticles.Stop();
            CrashParticules.Stop();
        }


        /// <summary>
        /// - Contiusously push rigidbody toward Z
        /// - Update run stats
        /// </summary>
        private void FixedUpdate()
        {
            if (ControlReleased) return;

            // Slight downward force to stay grounded
            var gravityModifier = Rb.linearVelocity.y - .2f;

            // Apply constant forward movement
            Rb.linearVelocity = new Vector3(Rb.linearVelocity.x, gravityModifier, ZMoveSpeed);

            StatsRecorder.SetMaxDistanceReached((int)transform.position.z);
        }


    }
}