using UnityEngine;

[ExecuteInEditMode]
public class RandomizePosition : MonoBehaviour
{
    public Vector3Int randomizedAxis;


    private void Start()
    {
        Vector3 newPosition = transform.localPosition;

        if (randomizedAxis.x != 0)
            newPosition.x = Random.Range(-randomizedAxis.x, randomizedAxis.x) * 2;
        if (randomizedAxis.y != 0)
            newPosition.y = Random.Range(-randomizedAxis.y, randomizedAxis.y) * 2;
        if (randomizedAxis.z != 0)
            newPosition.z = Random.Range(-randomizedAxis.z, randomizedAxis.z) * 2;

        transform.localPosition = newPosition;
    }
}
