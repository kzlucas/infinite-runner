using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
public class PlayerController : MonoBehaviour
{

    [Header("X Movement Settings")]

    /// <summary> Movement speed of the player</summary>
    public float moveSpeed = 15f;

    /// <summary> Current target X position to move toward</summary>
    private int currentX = 0;

    /// <summary> Maximum allowed X position</summary>
    private int maxX = 4;

    /// <summary> Minimum allowed X position</summary>
    private int minX = -4;

    /// <summary> Acceleration factor based on input</summary>
    private float moveAcceleration = 1f;

    /// <summary> Acceleration factor when input is up</summary>
    public float moveAccelerationUpFactor = 2f;

    /// <summary> Deceleration factor when input is down</summary>
    public float moveAccelerationDownFactor = 0.5f;


    [Header("Jump Settings")]

    /// <summary> Is the player currently jumping</summary>
    public bool isJumping = false;

    /// <summary> Height of the jump</summary>
    public float jumpHeight = 2f;

    /// <summary> Duration of the jump ascent</summary>
    public float jumpAscentDuration = 0.5f;

    /// <summary> Duration of the jump descent</summary>
    public float jumpDescentDuration = 0.25f;

    /// <summary> Easing curve for jump ascent</summary>
    public AnimationCurve jumpAscendEasingCurve;

    /// <summary> Easing curve for jump descent</summary>
    public AnimationCurve jumpDescendEasingCurve;


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
        // Subscribe to move input events
        InputHandlersManager.Instance.Register("Move", moveActionRef, OnUpdate: OnMoveUpdate);
        InputHandlersManager.Instance.Register("Jump", jumpActionRef, OnTrigger: OnJumpTrigger);
    }


    /// <summary>
    /// Contiusously translate on Z
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * moveAcceleration);
    }


    /// <summary>
    ///  Handle player movement input
    /// </summary>
    /// <param name="direction"></param>
    private void OnMoveUpdate(Vector2 direction)
    {
        // When input up or down, update acceleration factor
        if (direction.y > 0)
        {
            moveAcceleration = moveAccelerationUpFactor;
        }
        else if (direction.y < 0)
        {
            moveAcceleration = moveAccelerationDownFactor;
        }
        else
        {
            moveAcceleration = 1f;
        }

        // Move the player towards the target X position
        Vector3 targetPosition = new Vector3(currentX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);


        // Do not process new input until the player reaches the target X position
        if (Math.Abs(transform.position.x - currentX) > 0.1f)
        {
            return;
        }

        // Update target X position based on input direction
        if (direction.x > 0)
        {
            // Move right
            if (currentX < maxX)
            {
                currentX++;
                currentX++;
            }
        }
        else if (direction.x < 0)
        {
            // Move left
            if (currentX > minX)
            {
                currentX--;
                currentX--;
            }
        }
    }

    private void OnJumpTrigger()
    {
        if (isJumping)
            return;
        else
            StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        isJumping = true;
        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", true);
        float targetY = transform.position.y + jumpHeight;

        // Ascend
        float elapsedTime = 0f;
        while (transform.position.y < targetY)
        {
            float factor = jumpAscendEasingCurve.Evaluate(elapsedTime / jumpAscentDuration);
            transform.position = new Vector3(
                transform.position.x
                , Mathf.Lerp(0, targetY, factor)
                , transform.position.z
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Descend
        elapsedTime = 0f;
        while (transform.position.y > 0f)
        {
            var factor = jumpDescendEasingCurve.Evaluate(elapsedTime / jumpDescentDuration);
            transform.position = new Vector3(
                transform.position.x
                , Mathf.Lerp(jumpHeight, 0f, factor)
                , transform.position.z
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure exact ground position
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        transform.Find("Renderer").GetComponent<Animator>().SetBool("isJumping", false);
        isJumping = false;
    }
}