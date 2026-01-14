#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[ExecuteInEditMode]
public class RandomizeRotation : MonoBehaviour
{
    public bool randomizeX = false;
    public bool randomizeY = true;
    public bool randomizeZ = false;


    private void Start()
    {
        Randomize();
    }

    public void Randomize()
    {
#if UNITY_EDITOR
        bool isPrefabInstance = PrefabUtility.IsPartOfPrefabInstance(gameObject);
        if (isPrefabInstance) return;
#endif

        Vector3 newRotation = transform.rotation.eulerAngles;

        if (randomizeX)
        {
            newRotation.x = Random.Range(0f, 360f);
        }
        if (randomizeY)
        {
            newRotation.y = Random.Range(0f, 360f);
        }
        if (randomizeZ)
        {
            newRotation.z = Random.Range(0f, 360f);
        }

        transform.rotation = Quaternion.Euler(newRotation);
    }
}