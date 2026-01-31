using System.Collections.Generic;
using Assets.Components.SquareColliders.Scripts;
using UnityEngine;

namespace Components.Player
{
    public class Utils
    {

        private static Controller _playerController;
        public static Controller PlayerController
        {
            get
            {
                if (_playerController == null)
                {
                    _playerController = GameObject.FindFirstObjectByType<Player.Controller>();
                }
                return _playerController;  
            }
            set
            {
                _playerController = value;
            }
        }



        /// <summary>
        ///  Check if there is an obstacle in front of the player
        /// </summary>
        /// <param name="distance">The distance to check for obstacles.</param>
        public static bool CheckIfObstacleInFront(Transform transform, float distance)
        {
            RaycastHit hit;
            List<Vector3> rays = new()
            {
                transform.position + (Vector3.forward * .2f) + (Vector3.up * 0.2f)
                , transform.position + (Vector3.right * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.2f)
                , transform.position + (Vector3.left * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.2f)
                , transform.position + (Vector3.forward * .2f) + (Vector3.up * 0.5f)
                , transform.position + (Vector3.right * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.5f)
                , transform.position + (Vector3.left * 1f) + (Vector3.forward * .2f) + (Vector3.up * 0.5f)
                , transform.position + (Vector3.forward * .2f) + (Vector3.up * 1f)
                , transform.position + (Vector3.right * 1f) + (Vector3.forward * .2f) + (Vector3.up * 1f)
                , transform.position + (Vector3.left * 1f) + (Vector3.forward * .2f) + (Vector3.up * 1f)
            };

            var raysDir = Vector3.forward * distance;
            foreach (var start in rays)
            {
                Debug.DrawLine(start, start + raysDir, Color.red);
                if (Physics.Raycast(start, raysDir, out hit, distance))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool CheckIfGroundInFront(Transform transform, float distance)
        {
            RaycastHit hit;
            var ray = transform.position + (Vector3.forward * .5f) + (Vector3.up * 0.5f);
            var raysDir = (Vector3.forward * distance) - (Vector3.up * .6f);
            Debug.DrawLine(ray, ray + raysDir, Color.yellow);
            if (Physics.Raycast(ray, raysDir, out hit, distance))
            {
                var c = hit.collider.GetComponent<ColliderType>();
                if (c != null && c.colliderType == ColliderType.Type.Platform)
                    return true;
            }
            return false;
        }
    }
}