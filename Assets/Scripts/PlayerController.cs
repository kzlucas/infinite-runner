using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Fields

    /// <summary> Indicates whether player control has been released (e.g., after a crash)</summary>
    [HideInInspector] public bool controlReleased = false;

    private Rigidbody rb;

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

    /// <summary> Duration of the slide action</summary>
    public float slideDuration = 1f;

    /// <summary> Particle system for slide effect</summary>
    public ParticleSystem slideParticles;




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




    #endregion
    

    /// <summary>
    ///   Subscribe to input events
    /// </summary>
    private void Start()
    {
        // do not play particles at start
        jumpParticles.Stop();
        slideParticles.Stop();
        crashParticules.Stop();

        // store original collider size
        capsuleCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = capsuleCollider.height;
        originalColliderCenterY = capsuleCollider.center.y;

        // attach rigidbody
        rb = gameObject.GetOrAddComponent<Rigidbody>();

        // Subscribe to move input events
        InputHandlersManager.Instance.Register("Move", moveActionRef, OnUpdate: OnMoveUpdate);
        InputHandlersManager.Instance.Register("Jump", jumpActionRef, OnTrigger: OnJumpTrigger);
        InputHandlersManager.Instance.Register("Slide", slideActionRef, OnTrigger: OnSlideTrigger);
    }

    /// <summary>
    ///  Called when the player crashes
    /// </summary>
    public void OnCrash()
    {
        // Play crash effects
        jumpParticles.Stop();
        slideParticles.Stop();
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
    private void Update()
    {
        if(controlReleased) return;

        // Apply constant forward movement
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y , zMoveSpeed);
    }


    /// <summary>
    ///  Handle player movement input
    /// </summary>
    /// <param name="direction"></param>
    private void OnMoveUpdate(Vector2 direction)
    {
        if(controlReleased) return;

        // Do not process new input until the player reaches the target X position
        if (Math.Abs(transform.position.x - targetXPosition) > 0.05f)
        {
            return;
        }

        // Update target X position based on input direction
        if (direction.x > 0 || (direction.x < 0))
        {
            var dirX = direction.x > 0 ? 1 : -1;
            targetXPosition =  (int)Mathf.Round(transform.position.x + (dirX * 2f));
            targetXPosition = Mathf.Clamp(targetXPosition, minX, maxX);
            StartCoroutine(GoToLaneRoutine(dirX));
        }

    }

    /// <summary>
    /// Move the player to the target lane smoothly
    /// </summary>
    /// <param name="dirX"></param>
    /// <returns></returns>
    private IEnumerator GoToLaneRoutine(int dirX)
    {
        float initialDistance = Math.Abs(targetXPosition - transform.position.x);
        while (Math.Abs(targetXPosition - transform.position.x) > 0.05f)
        {
            if(controlReleased) yield break;

            float currentDistance = Math.Abs(targetXPosition - transform.position.x);
            float easeMoveSpeed =  xMoveSpeed * currentDistance / initialDistance; 
            easeMoveSpeed = Mathf.Max(easeMoveSpeed, 1f); // minimum speed
            rb.linearVelocity = new Vector3(dirX * easeMoveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
            yield return null;
        }
        if(controlReleased) yield break;
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, rb.linearVelocity.z);
        rb.position = new Vector3(targetXPosition, rb.position.y, rb.position.z);
    }


    /// <summary>
    ///  Handle player jump input
    /// </summary>
    private void OnJumpTrigger()
    {
        if(controlReleased) return;
        if (!isGrounded) return;
        if (isSliding) return;
        else StartCoroutine(JumpRoutine());
    }


    /// <summary>
    ///  Handle the jump action
    /// </summary>
    private IEnumerator JumpRoutine()
    {
        AudioManager.Instance.PlaySound("jump");

        // Start jump animation
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", true);
        
        // Apply jump velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
        
        // Play jump particles
        jumpParticles.transform.parent = null; // detach from player
        jumpParticles.transform.position = transform.position + Vector3.up * 0.3f; // slightly above ground
        jumpParticles.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, rb.linearVelocity.z);
        jumpParticles.Play();

        // Wait until the player is back on the ground
        if(controlReleased) yield break;
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0f);
        if(controlReleased) yield break;
        yield return new WaitUntil(() => isGrounded);

        // Reset jump animation
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", false);

        // reattach ps to player to clean up hierarchy
        jumpParticles.transform.parent = transform; 

        yield break;
    }


    /// <summary>
    ///  Handle player slide input
    /// </summary>
    private void OnSlideTrigger()
    {
        if(controlReleased) return;
        if (isSliding) return;
        if (!isGrounded) return;
        StartCoroutine(SlideRoutine());
    }

    /// <summary>
    ///  Handle the slide action
    /// </summary>
    private IEnumerator SlideRoutine()
    {
        AudioManager.Instance.PlaySound("slide");

        isSliding = true;
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", true);
        SetColliderToSlidingPosition();

        // Play slide particles
        slideParticles.Play();

        // wait for slide duration
        yield return new WaitForSeconds(slideDuration);

        // stop slide
        isSliding = false;
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isSliding", false);
        SetColliderToNormalPosition();

        // disable slide particles
        slideParticles.Stop();
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