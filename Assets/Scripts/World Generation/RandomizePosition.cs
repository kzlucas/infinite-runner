using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class RandomizePosition : MonoBehaviour
{
    public Vector3Int randomizeAxisMin;
    public Vector3Int randomizeAxisMax;
    public Vector3Int randomizeAxisStep = new Vector3Int(2, 2, 2);
    private int attempts = 0;
    private bool isRandomized = false;


    private void Start()
    {
        StartRandomization();
    }

    private void StartRandomization()
    {
        attempts++;
        Vector3 newPosition = new Vector3(0, 0, 0);

        int randomX = Random.Range(
            (int)(randomizeAxisMin.x / randomizeAxisStep.x),
            (int)(randomizeAxisMax.x / randomizeAxisStep.x) + 1
        ) * randomizeAxisStep.x;

        int randomY = Random.Range(
            (int)(randomizeAxisMin.y / randomizeAxisStep.y),
            (int)(randomizeAxisMax.y / randomizeAxisStep.y) + 1
        ) * randomizeAxisStep.y;

        int randomZ = Random.Range(
            (int)(randomizeAxisMin.z / randomizeAxisStep.z),
            (int)(randomizeAxisMax.z / randomizeAxisStep.z) + 1
        ) * randomizeAxisStep.z;

        newPosition.x += randomX;
        newPosition.y += randomY;
        newPosition.z += randomZ;

        transform.localPosition = newPosition;

        /*
         *
         * Ensure no siblings have the same position
         * If so, re-randomize 
         */
        
        if (attempts > 10)
        {
            // prevent infinite loops 
            // (if too many siblings, should not happen)
            Destroy(this);
            return; 
        }
        var containerWorldSegment = transform.parent;
        var randomizePositionnedSiblings = containerWorldSegment.GetComponentsInChildren<RandomizePosition>();
        foreach (var sibling in randomizePositionnedSiblings)
        {
            if (sibling != this && sibling.transform.localPosition == transform.localPosition && sibling.isRandomized)
            {
                // re-randomize
                StartRandomization();
                break;
            }
        }

        isRandomized = true;
    }
}
