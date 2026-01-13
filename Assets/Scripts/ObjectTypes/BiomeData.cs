using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save Biome specific data
/// </summary>
[System.Serializable]
public class BiomeData
{
    public string name;
    public List<WorldSegment> segments;
    public Color colorSky = new Color(0f, .6f, 1f);
    public Color colorSkyHorizon = new Color(0f, .3f, .8f);
    public Color colorSkyGround = new Color(0f, 0f, .5f);
}