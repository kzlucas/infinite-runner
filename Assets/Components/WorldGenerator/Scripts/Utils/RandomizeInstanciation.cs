using UnityEngine;


namespace WorldGenerator.Scripts.Utils
{

    /// <summary>
    ///   Helper class to randomize instantiation of game object.
    /// </summary>
    public class RandomizeInstanciation : MonoBehaviour
    {

        [Range(0.0f, 100.0f)]
        public float percentChanceToInstantiate = 50.0f;


        private void Awake()
        {
            float roll = Random.Range(0.0f, 100.0f);
            if (roll > percentChanceToInstantiate)
            {
                Destroy(gameObject);
            }
        }
    }
}
