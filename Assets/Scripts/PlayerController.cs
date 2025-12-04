using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
///  Controller for the player character
/// </summary>
public class PlayerController : MonoBehaviour
{

    /// <summary> Reference to the input action for movements</summary>
    public InputActionReference moveActionRef;


    /// <summary>
    ///   Subscribe to input events
    /// </summary>
    private void Start()
    {
        // Subscribe to move input events
        InputHandlersManager.Instance.Register("Move", moveActionRef, OnUpdate: OnMove);
    }


    /// <summary>
    /// Contiusously translate on Z
    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 10f);
    }


    /// <summary>
    ///  Handle player movement input
    /// </summary>
    /// <param name="direction"></param>
    private void OnMove(Vector2 direction)
    {
        // Move the player based on input direction
        transform.Translate(new Vector3(direction.x, 0, 0) * Time.deltaTime * 10f);
    }
}