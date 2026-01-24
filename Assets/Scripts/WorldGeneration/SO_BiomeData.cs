using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Create New Biome Data", menuName = "Biome Data/New Biome Data", order = 1)]
public class SO_BiomeData : ScriptableObject
{

    [SerializeField] private string _biomeName;
    public string BiomeName => _biomeName;


    [SerializeField] private List<WorldSegment> _segments;
    public List<WorldSegment> Segments => _segments;


    [SerializeField] private Color _colorPaint = new Color(0f, .6f, 1f);
    public Color ColorPaint => _colorPaint;


    [SerializeField] private Color _crystalColor = new Color(1f, 1f, 1f);
    public Color crystalColor => _crystalColor;


    [SerializeField] private Color _colorSky = new Color(0f, .6f, 1f);
    public Color ColorSky => _colorSky;


    [SerializeField] private Color _colorSkyHorizon = new Color(0f, .3f, .8f);
    public Color ColorSkyHorizon => _colorSkyHorizon;


    [SerializeField] private Color _colorSkyGround = new Color(0f, 0f, .5f);
    public Color ColorSkyGround => _colorSkyGround;
    

    [SerializeField] private Sprite _gaugeImage;
    public Sprite GaugeImage => _gaugeImage;


}