using UnityEngine;


namespace Components.Utils
{

    /// <summary>
    ///  Component that moves an object towards a target position or another object's position
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class TargetPosition : MonoBehaviour
    {
        public GameObject targetObject = null;
        public Vector3 targetPosition = Vector3.zero;
        public float speedToTarget = 5f;
        public bool localSpace = false;

        private void Update()
        {
            Vector3 _targetPos;
            if (targetObject == null)
                _targetPos = targetPosition;
            else
                _targetPos = targetPosition + targetObject.transform.position;

            if (localSpace)
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, _targetPos, speedToTarget * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, _targetPos, speedToTarget * Time.deltaTime);
        }

    }
}
