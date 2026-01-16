using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class RandomizePosition : MonoBehaviour
{
    public bool staticPosition = false;
    public Vector3Int randomizeAxisMin;
    public Vector3Int randomizeAxisMax;
    public Vector3Int randomizeAxisStep = new Vector3Int(2, 2, 2);
    private bool isRandomized = false;


    private void Awake()
    {
        if(staticPosition)
        {
            isRandomized = true;
        }
    }

    private void Start()
    {
        StartRandomization();
    }

    private void StartRandomization()
    {
        if(isRandomized)
        {
            return;
        }

        /*
         *
         * Get possible slots on X axis 
         */
        
        List<int> slotX = new List<int>();
        for (int x = randomizeAxisMin.x; x <= randomizeAxisMax.x; x += randomizeAxisStep.x)
        {
            slotX.Add(x);
        }

        /*
         *
         * Ensure no siblings have the same X position 
         */
        
        var containerWorldSegment = transform.parent;
        List<RandomizePosition> randomizePositionnedSiblings = containerWorldSegment.GetComponentsInChildren<RandomizePosition>()
            .ToList()
            .FindAll(rp => rp.isRandomized && rp != this);

        foreach (var sibling in randomizePositionnedSiblings)
        {
            int siblingX = (int)sibling.transform.localPosition.x;
            if (slotX.Contains(siblingX))
            {
                slotX.Remove(siblingX);
            }
        }

        if(slotX.Count == 0)
        {
            Debug.LogWarning($"[RandomizePosition] No available X slot for {gameObject.name} in {containerWorldSegment.name}");
            return;
        }

        /*
         *
         * Randomize position 
         */
        
        Vector3 newPosition = new Vector3(0, 0, 0);

        int randomX = slotX[Random.Range(0, slotX.Count)];

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

        isRandomized = true;
    }
}
