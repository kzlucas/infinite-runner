using System;
using System.Threading.Tasks;
using UnityEngine;

public class WorldGenerationManager : MonoBehaviour, IInitializable
{
    
    public int Priority => 0;
    public Type[] Dependencies => null;
    

    public async Task InitializeAsync()
    {
        Debug.Log("[WorldGenerationManager] init start...");

        await Task.Delay(500); 

        Debug.Log("[WorldGenerationManager] init finished!");
    }
}
