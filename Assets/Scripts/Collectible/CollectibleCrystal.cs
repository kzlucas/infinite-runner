using System.Linq;
using UnityEngine;



public class CollectibleCrystal : Collectible
{
   

    private void Start()
    {
        
        // Set material color based on biome
        var color = BiomesData.Instance.current.crystalColor;
        var materials = GetComponentsInChildren<Renderer>().SelectMany(r => r.materials);
        foreach (var mat in materials)
        {
            mat.SetColor("_BaseColor", color);
        }

        // Set light emission color based on biome
        var light = GetComponentInChildren<Light>();
        if (light != null)
        {
            light.color = color;
        }
    }
}
