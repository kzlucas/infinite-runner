using System.Collections.Generic;
using UnityEngine;

namespace Assets.Components.SquareColliders.Scripts
{

    /// <summary>
    /// Behavior for managing square colliders
    /// </summary>
    public class SquareCollidersBehaviour: MonoBehaviour
    {

        /// <summary>
        /// Clears all merged square colliders
        /// </summary>
        public void Clear()
        {
            // Destroy existing merged colliders
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                // Get the child at the current index
                Transform child = transform.GetChild(i);

                // Destroy the child GameObject
                if (Application.isEditor && !Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
            }   
        }

        /// <summary>
        /// Pushes a new merged collider into the manager
        /// </summary>
        /// <param name="mesh">Mesh for the collider</param>
        /// <param name="typedCollider">Typed collider information</param>
        /// <returns></returns>
        public MeshCollider Push(Mesh mesh, TypedCollider typedCollider)
        {
            GameObject go = new GameObject("MergedCollider");
            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;


            // Set collider type
            var colliderTypeComp = go.AddComponent<ColliderType>();
            colliderTypeComp.colliderType = typedCollider.colliderType;

            return meshCollider;
        }

        /// <summary> 
        /// Collects all square colliders in the scene
        /// </summary>
        public List<Collider> Collect()
        {
            var squareColliders = new List<Collider>();
            GameObject[] colliderObjects = GameObject.FindGameObjectsWithTag("Composite Square Collider");
            foreach (var obj in colliderObjects)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null)
                {
                    squareColliders.Add(col);
                    col.enabled = true;
                }
            }

            return squareColliders;
        }

        /// <summary> 
        /// Deactivates all given square colliders in input array
        /// </summary>
        public void DeactivateAll(List<Collider> squareColliders)
        {
            foreach (Collider c in squareColliders)
            {
                if (c != null)
                {
                    c.enabled = false;
                }
            }
        }
    }
}