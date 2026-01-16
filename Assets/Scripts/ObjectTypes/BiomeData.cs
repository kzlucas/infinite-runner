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
    public Color colorPaint = new Color(0f, .6f, 1f);
    public Color colorSky = new Color(0f, .6f, 1f);
    public Color colorSkyHorizon = new Color(0f, .3f, .8f);
    public Color colorSkyGround = new Color(0f, 0f, .5f);

    public BiomeData(BiomeData other)
    {
        name = other.name;
        segments = new List<WorldSegment>(other.segments);
        colorPaint = other.colorPaint;
        colorSky = other.colorSky;
        colorSkyHorizon = other.colorSkyHorizon;
        colorSkyGround = other.colorSkyGround;
    }
}