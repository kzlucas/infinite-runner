using System.Linq;
using UnityEngine;


public class CollectibleCrystal : Collectible
{

    /// <summary>
    ///   Initializes the collectible crystal by setting its color and light emission based on the current biome.
    /// </summary>
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


    /// <summary>
    ///  Adds crystal to the crystal bucket when a crystal is caught.
    /// </summary>
    public override void OnCollide()
    {
        if (IsCollected) return;
        IsCollected = true;

        CrystalsManager.AddCrystals(1);
    }
}
