using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
public class PlayerController : MonoBehaviour
{
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


    [Header("Input Action References")]

    /// <summary> Reference to the input action for movements</summary>
    public InputActionReference moveActionRef;

    /// <summary> Reference to the input action for jump</summary>
    public InputActionReference jumpActionRef;


    /// <summary>
    ///   Subscribe to input events
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Subscribe to move input events
        InputHandlersManager.Instance.Register("Move", moveActionRef, OnUpdate: OnMoveUpdate);
        InputHandlersManager.Instance.Register("Jump", jumpActionRef, OnTrigger: OnJumpTrigger);
    }

    /// <summary>
    ///  Called when the player crashes
    /// </summary>
    public void OnCrash()
    {
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

    private void OnJumpTrigger()
    {
        if(controlReleased) return;
        if (!isGrounded) return;
        else StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", true);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);

        if(controlReleased) yield break;
        yield return new WaitUntil(() => rb.linearVelocity.y <= 0f);
        if(controlReleased) yield break;
        yield return new WaitUntil(() => isGrounded);

        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", false);

        yield break;
    }
}