using System.Collections;
using Components.EndGame.Scripts;
using Components.ServiceLocator.Scripts;
using Components.Stats;
using Components.UI.Scripts;
using InputsHandler;
using Player.States;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
namespace Player
{
    public class Controller : StateMachine.Behaviour
    {
        #region Fields

        
        [Header("Dependencies")]
        private UiRegistry UiRegistry => ServiceLocator.Get<UiRegistry>();
        private EndGameManager EndGameManager => ServiceLocator.Get<EndGameManager>();
        private InputHandlersManager InputHandlersManager => ServiceLocator.Get<InputHandlersManager>();


        [Header("References")]
        public readonly StateMachine.StateMachine sm = new();
        public Health Health;
        public PlayerHistory History;
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



            /*
             *
             * Initialize component references and variables 
             */


            // do not play particles at start
            StopParticles();

            // Subscribe/unsub to landing event
            CollisionHandler.OnLanded += sm.TransitionTo<LandState>;
            SceneLoader.Instance.OnSceneExit += () => CollisionHandler.OnLanded -= sm.TransitionTo<LandState>;

            // Set z position slightly forward at begining
            transform.position = transform.position + Vector3.forward;

            // freeze position during scene initialization then unfreeze
            Rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized == true);

            // Initial position adjust to avoid clipping with ground
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);

            // Start game countdown
            UiRegistry.Countdown.Run();
            yield return new WaitUntil(() => UiRegistry.Countdown.animationFinished == true);

            // Let's play
            Rb.constraints = RigidbodyConstraints.FreezeRotation;

            // Initial record
            History.Record();
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


        /// <summary>
        /// Trigger crash event and transition to crash state
        /// </summary>
        public void TriggerCrashEvent()
        {
            if (this == null) return;
            if (ControlReleased) return;
            sm.TransitionTo<CrashState>();
            EndGameManager.TriggerEndGame();
        }


    }
}