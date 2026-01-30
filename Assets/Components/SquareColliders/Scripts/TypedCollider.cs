using UnityEngine;
using System.Collections.Generic;


namespace Assets.Components.SquareColliders.Scripts
{
    
    public class TypedCollider
    {
        public Collider collider;
        public ColliderType.Type colliderType;
        public List<Vector3> points;

        public TypedCollider(Collider collider, ColliderType.Type colliderType, List<Vector3> points)
        {
            this.collider = collider;
            this.colliderType = colliderType;
            this.points = points;
        }
    }
}
