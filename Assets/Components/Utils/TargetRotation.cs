using UnityEngine;

namespace Components.Utils
{
    /// <summary>
    ///  Component that rotates an object towards a target rotation
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class TargetRotation : MonoBehaviour
    {
        public Vector3 targetRotation = Vector3.zero;
        public float speedToTarget = 5f;
        public bool localSpace = false;

        private void Update()
        {
            if (localSpace)
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(targetRotation), speedToTarget * Time.deltaTime);
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), speedToTarget * Time.deltaTime);
        }

    }
}
