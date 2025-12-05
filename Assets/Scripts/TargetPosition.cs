using UnityEngine;

[RequireComponent(typeof(Transform))]
public class TargetPosition : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speedToTarget = 5f;
    public bool localSpace = false;

    private void Update()
    {
        if (localSpace)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speedToTarget * Time.deltaTime);
        else
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speedToTarget * Time.deltaTime);
    }

}
