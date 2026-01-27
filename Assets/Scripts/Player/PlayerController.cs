using System;
using System.Collections;
using Player.States;
using StateMachine;
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


        public readonly AdvancedStateMachine sm = new();
        public Health health;
        public PlayerHistory history;


        [Header("Collider")]

        /// <summary> Reference to the collision handling component</summary>
        public CollisionHandling collisionHandler;

        /// <summary> Reference to the player's rigidbody</summary>
        public Rigidbody rb;



        [Header("Flags")]

        /// <summary> Indicates whether player control has been released (e.g., after a crash)</summary>
        [HideInInspector] public bool controlReleased = false;



        [Header("X Movement Settings")]

        /// <summary> Input direction for movement along X axis (-1 left, 1 right)</summary>
        public Vector2 inputMoveDir = Vector2.zero;

        /// <summary> X movement speed of the player</summary>
        public float xMoveSpeed = 15f;

        /// <summary> Forward movement speed of the player</summary>
        public float zMoveSpeed = 15f;

        /// <summary> Maximum allowed X position</summary>
        public int maxX = 4;

        /// <summary> Minimum allowed X position</summary>
        public int minX = -4;

        /// <summary> Particle system effect run when changing lane</summary>
        public ParticleSystem laneChangeParticles;

        /// <summary>Is the player currently changing lane flag</summary>
        public bool isChangingLane = false;



        [Header("Jump Settings")]

        /// <summary> Is the player currently jumping</summary>
        public bool isGrounded = false;

        /// <summary> Height of the jump</summary>
        public float jumpHeight = 2f;

        /// <summary> Particle system for jump effect</summary>
        public ParticleSystem jumpParticles;



        [Header("Slide Settings")]

        /// <summary> Is the player currently sliding</summary>
        public bool isSliding = false;

        /// <summary> Particle system for slide effect</summary>
        public ParticleSystem slideParticles;

        /// <summary> Slide coroutine</summary>
        public IEnumerator slideRoutine;



        [Header("Crash Settings")]

        /// <summary> Particle system for crash effect</summary>
        public ParticleSystem crashParticules;



        [Header("Input Action References")]

        /// <summary> Reference to the input action for movements</summary>
        public InputActionReference moveActionRef;

        /// <summary> Reference to the input action for jump</summary>
        public InputActionReference jumpActionRef;

        /// <summary> Reference to the input action for slide</summary>
        public InputActionReference slideActionRef;





        #endregion


        private void OnDisable() => StopAllCoroutines();



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

            InputHandlersManager.Instance.Register(
                "Jump"
                , jumpActionRef
                , OnTrigger: () =>
                {
                    if (CanJump()) sm.TransitionTo<JumpState>();
                }
            );

            InputHandlersManager.Instance.Register(
                "Move"
                , moveActionRef
                , OnUpdate: (v2) =>
                {
                    inputMoveDir = v2;
                    if (CanMove(v2)) sm.TransitionTo<MoveState>();
                }
                , OnRelease: () =>
                {
                    sm.GetState<MoveState>().OnRelease();
                }
            );

            InputHandlersManager.Instance.Register(
                "Slide"
                , slideActionRef
                , OnTrigger: () =>
                {
                    if (CanSlide()) sm.TransitionTo<SlideState>();
                },
                OnHold: () =>
                {
                    if (CanSlide()) sm.TransitionTo<SlideState>();
                },
                OnRelease: () =>
                {
                    sm.GetState<SlideState>().OnRelease();
                }
            );



            /*
             *
             * Initialize component references and variables 
             */


            // do not play particles at start
            StopParticles();

            // Subscribe/unsub to landing event
            collisionHandler.OnLanded += sm.TransitionTo<LandState>;
            SceneLoader.Instance.OnSceneExit += () => collisionHandler.OnLanded -= sm.TransitionTo<LandState>;

            // Set z position slightly forward at begining
            transform.position = transform.position + Vector3.forward;

            // freeze position during scene initialization then unfreeze
            rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized == true);

            // Initial position adjust to avoid clipping with ground
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);

            // Start game countdown
            UiManager.Instance.countdown.Run();
            yield return new WaitUntil(() => UiManager.Instance.countdown.animationFinished == true);

            // Let's play
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            // Initial record
            history.Record();
        }


        /// <summary>
        ///  Check if the player can jump
        /// </summary>
        private bool CanJump()
        {
            return !(
                (this == null)
                || controlReleased
                || (Time.timeScale == 0f)
                || isSliding
                || (!isGrounded)
            ) ;
        }


        /// <summary>
        /// Check if the player can move
        /// </summary>
        private bool CanMove(Vector2 dir)
        {
            return !(
                (this == null)
                || controlReleased
                || isSliding
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
                || controlReleased
                || isSliding
                || (Time.timeScale == 0f)
                || (!isGrounded)
            );
        }


        /// <summary>
        /// Stop all particle effects
        /// </summary>
        public void StopParticles()
        {
            laneChangeParticles.Stop();
            jumpParticles.Stop();
            slideParticles.Stop();
            crashParticules.Stop();
        }


        /// <summary>
        /// - Contiusously push rigidbody toward Z
        /// - Update run stats
        /// </summary>
        private void FixedUpdate()
        {
            if (controlReleased) return;

            // Slight downward force to stay grounded
            var gravityModifier = rb.linearVelocity.y - .2f;

            // Apply constant forward movement
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, gravityModifier, zMoveSpeed);

            StatsRecorder.Instance.SetMaxDistanceReached((int)transform.position.z);
        }


        /// <summary>
        /// Trigger crash event and transition to crash state
        /// </summary>
        public void TriggerCrashEvent()
        {
            if (this == null) return;
            if (controlReleased) return;
            sm.TransitionTo<CrashState>();
            EndGameManager.Instance.TriggerEndGame();
        }


    }
}