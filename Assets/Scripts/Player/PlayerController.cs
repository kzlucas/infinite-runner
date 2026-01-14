using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Fields


    [Header("References")]

    /// <summary> Reference to the collision handling component</summary>
    public PlayerCollisionHandling collisionHandler;
    private Rigidbody rb;



    [Header("Flags")]

    /// <summary> Indicates whether player control has been released (e.g., after a crash)</summary>
    [HideInInspector] public bool controlReleased = false;



    [Header("X Movement Settings")]

    public int targetXPosition = 0;

    /// <summary> X movement speed of the player</summary>
    public float xMoveSpeed = 15f;

    /// <summary> Forward movement speed of the player</summary>
    public float zMoveSpeed = 15f;

    /// <summary> Maximum allowed X position</summary>
    private int maxX = 4;

    /// <summary> Minimum allowed X position</summary>
    private int minX = -4;

    /// <summary> Reference to the lane change coroutine</summary>
    private IEnumerator goToLaneRoutine;

    /// <summary> Particle system effect run when changing lane</summary>
    public ParticleSystem laneChangeParticles;



    [Header("Jump Settings")]

    /// <summary> Is the player currently jumping</summary>
    public bool isGrounded = false;

    /// <summary> Height of the jump</summary>
    public float jumpHeight = 2f;

    /// <summary> Particle system for jump effect</summary>
    public ParticleSystem jumpParticles;

    /// <summary> Maximum number of consecutive jumps allowed between landings (2 basicly means double jump allowed, set to 1 for disabling))</summary>
    public int maxJumpCount = 2;

    /// <summary> Current number of consecutive jumps performed without touching ground</summary>
    [HideInInspector] public int currentJumpCount = 0;



    [Header("Slide Settings")]

    /// <summary> Is the player currently sliding</summary>
    public bool isSliding = false;

    /// <summary> Particle system for slide effect</summary>
    public ParticleSystem slideParticles;

    /// <summary> Slide coroutine</summary>
    private IEnumerator slideRoutine;




    [Header("Input Action References")]

    /// <summary> Reference to the input action for movements</summary>
    public InputActionReference moveActionRef;

    /// <summary> Reference to the input action for jump</summary>
    public InputActionReference jumpActionRef;

    /// <summary> Reference to the input action for slide</summary>
    public InputActionReference slideActionRef;




    [Header("Crash Settings")]

    /// <summary> Particle system for crash effect</summary>
    public ParticleSystem crashParticules;



    [Header("Collider")]

    /// <summary> Reference to the player's collider</summary>
    public CapsuleCollider capsuleCollider;

    /// <summary> Reference to the player's collider normal dimensions: height</summary>
    public float originalColliderHeight;

    /// <summary> Reference to the player's collider normal dimensions: center Y position</summary>
    public float originalColliderCenterY;

    public string guid;

    #endregion


    private void OnDisable() => StopAllCoroutines();



    /// <summary>
    ///   Init component and Subscribe to input events
    /// </summary>
    private IEnumerator Start()
    {
        // do not play particles at start
        jumpParticles.Stop();
        slideParticles.Stop();
        crashParticules.Stop();
        laneChangeParticles.Stop();

        // store original collider size
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = capsuleCollider.height;
        originalColliderCenterY = capsuleCollider.center.y;

        // attach rigidbody
        rb = gameObject.GetOrAddComponent<Rigidbody>();

        // Subscribe to move input events
        InputHandlersManager.Instance.Register("Move", moveActionRef, OnUpdate: OnMoveUpdate);
        InputHandlersManager.Instance.Register("Jump", jumpActionRef, OnTrigger: OnJumpTrigger);
        InputHandlersManager.Instance.Register("Slide", slideActionRef, OnTrigger: OnSlideTrigger, OnRelease: OnSlideRelease);
        
        // Subscribe/unsub to landing event
        collisionHandler.OnLanded += OnLandedHandler;
        SceneLoader.Instance.OnSceneExit += () => collisionHandler.OnLanded -= OnLandedHandler;

        // freeze position during game initialization then unfreeze
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        yield return new WaitUntil(() => SceneInitializer.Instance.isInitialized == true);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    /// <summary>
    ///  Called when the player crashes
    /// </summary>
    public void OnCrash()
    {
        // Play crash effects
        jumpParticles.Stop();
        slideParticles.Stop();
        laneChangeParticles.Stop();
        crashParticules.transform.parent = null;
        crashParticules.transform.position = transform.position + Vector3.up * 0.5f;
        crashParticules.Play();
        AudioManager.Instance.PlaySound("crash");

        // release control
        controlReleased = true;

        // freeze camera
        Camera.main.GetComponent<CameraBehaviors>().FreezeCamera();

        // unfreeze rotation
        rb.freezeRotation = false;

        // Apply random torque on crash
        float crashNgFactor = 10f;
        rb.angularVelocity = new Vector3(UnityEngine.Random.Range(-crashNgFactor, crashNgFactor), UnityEngine.Random.Range(-crashNgFactor, crashNgFactor), UnityEngine.Random.Range(-crashNgFactor, crashNgFactor));

        // Apply an upward force on crash
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 5f, 5f);

        // Stop animations
        transform.Find("Renderer").GetComponent<Animator>().enabled = false;
    }


    /// <summary>
    /// - Contiusously push rigidbody toward Z
    /// - Check grounded state
    /// </summary>
    private void FixedUpdate()
    {
        if (controlReleased) return;

        // Slight downward force to stay grounded
        var gravityModifier = rb.linearVelocity.y - .2f; 
        
        // Apply constant forward movement
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, gravityModifier, zMoveSpeed);
    }


    /// <summary>
    ///  Handle player movement input
    /// </summary>
    /// <param name="direction"></param>
    private void OnMoveUpdate(Vector2 direction)
    {
        if (this == null) return;
        if (controlReleased) return;

        // Do not process new input until reaching target position 
        // (0.1 tolerance to prevent input spam)
        if (Math.Abs(transform.position.x - targetXPosition) > 0.1f)
        {
            return;
        }

        // Update target X position based on input direction
        if (direction.x > 0 || (direction.x < 0))
        {
            var dirX = direction.x > 0 ? 1 : -1;
            targetXPosition = (int)Mathf.Round(transform.position.x + (dirX * 2f));
            targetXPosition = Mathf.Clamp(targetXPosition, minX, maxX);

            goToLaneRoutine.Replace(GoToLaneRoutine());
        }

    }

    /// <summary>
    /// Move the player to the target lane smoothly
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoToLaneRoutine()
    {
        // Play lane change sfx effect
        var sign = targetXPosition - transform.position.x > 0 ? 1 : -1;
        if(sign > 0) laneChangeParticles.transform.rotation = Quaternion.Euler(-90, 0, 80);
        else laneChangeParticles.transform.rotation = Quaternion.Euler(-90, 0, -80);
        laneChangeParticles.Play();


        // Handle case where player is already very close to target
        float initialDistance = Math.Abs(targetXPosition - transform.position.x);
        if (initialDistance < 0.1f)
        {
            rb.position = new Vector3(targetXPosition, rb.position.y, rb.position.z);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
            yield break;
        }

        while (Math.Abs(targetXPosition - transform.position.x) > 0.05f)
        {
            if (controlReleased) yield break;
            
            // Stop particles when close enough
            if (Math.Abs(targetXPosition - transform.position.x) < 0.5f) 
                laneChangeParticles.Stop();

            float currentDistance = Math.Abs(targetXPosition - transform.position.x);
            float direction = targetXPosition - transform.position.x > 0 ? 1 : -1;

            // Ease-out movement: faster when far, slower when close
            float easeMultiplier = currentDistance / initialDistance;
            easeMultiplier = Mathf.Clamp(easeMultiplier, 0.3f, 1f); // minimum 30% speed to avoid getting stuck

            float targetSpeed = direction * easeMultiplier * xMoveSpeed;

            // Prevent overshooting by capping speed when very close
            if (currentDistance < 0.5f)
            {
                float maxSpeedForDistance = currentDistance * 10f; // max speed based on distance
                targetSpeed = Mathf.Clamp(targetSpeed, -maxSpeedForDistance, maxSpeedForDistance);
            }

            rb.linearVelocity = new Vector3(targetSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            yield return null;
        }

        if (controlReleased) yield break;

        // Snap to exact position and stop horizontal movement
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
        rb.position = new Vector3(targetXPosition, rb.position.y, rb.position.z);

    }


    /// <summary>
    ///  Handle player jump input
    /// </summary>
    private void OnJumpTrigger()
    {
        if (this == null) return;
        if (controlReleased) return;
        if (isSliding) return;
        if (currentJumpCount >= maxJumpCount) return;

        // start new jump routine                
        Jump();
    }



    /// <summary>
    ///  Play jump particules at specified offset
    /// </summary>
    private void PlayJumpParticules(float zOffset = 0f)
    {
        jumpParticles.transform.parent = null; // detach from player
        jumpParticles.transform.position = transform.position + Vector3.up * zOffset; // slightly above ground
        jumpParticles.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, rb.linearVelocity.z);
        jumpParticles.Play();
    }


    /// <summary>
    ///  Handle the jump action
    /// </summary>
    private void Jump()
    {
        currentJumpCount++;

        // Play jump sound
        AudioManager.Instance.PlaySound("jump");

        // Start jump animation
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", true);

        // Physic jump
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

        // Play jump particles
        PlayJumpParticules(0.3f);
    }



    /// <summary>
    ///  Handle actions to perform when the player lands
    /// </summary>
    private void OnLandedHandler()
    {
        if (this == null) return;

        // Play land sound
        AudioManager.Instance.PlaySound("land");

        // Play again jump particles on landing
        PlayJumpParticules();

        // reattach ps to player to clean up hierarchy
        jumpParticles.transform.parent = transform;

        // Reset jump count when landing
        currentJumpCount = 0;
    }




    /// <summary>
    ///  Handle player slide input
    /// </summary>
    private void OnSlideTrigger()
    {
        if (this == null) return;
        if (controlReleased) return;
        if (isSliding) return;
        if (!isGrounded) return;

        slideRoutine.Replace(SlideRoutine());
    }

    /// <summary>
    ///  Handle player slide release input
    /// </summary>
    private void OnSlideRelease()
    {
        if (this == null) return;
        if (controlReleased) return;
        if (!isSliding) return;

        // stop slide
        isSliding = false;
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", false);
        SetColliderToNormalPosition();

        // disable slide particles
        slideParticles.Stop();
    }

    /// <summary>
    ///  Handle the slide action
    /// </summary>
    private IEnumerator SlideRoutine()
    {
        AudioManager.Instance.PlaySound("slide");

        isSliding = true;
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", true);
        SetColliderToSlidingPosition(); // @improve: match the real mesh dimension accross frames to avoid pass through obstacle colliders during animation transition

        // Play slide particles
        slideParticles.Play();

        yield break;
    }



    /// <summary>
    /// Set the player's collider to the sliding position
    /// </summary>
    private void SetColliderToSlidingPosition()
    {
        capsuleCollider.height = originalColliderHeight / 2f;  // ~0.5
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y - originalColliderHeight / 4f, capsuleCollider.center.z); // ~0.3
    }


    /// <summary>
    /// Set the player's collider back to the normal position
    /// </summary>
    private void SetColliderToNormalPosition()
    {
        capsuleCollider.height = originalColliderHeight;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, originalColliderCenterY, capsuleCollider.center.z);
    }
}