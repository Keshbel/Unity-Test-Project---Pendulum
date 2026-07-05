using System;
using System.Collections.Generic;
using System.Linq;
using Pendulum.Domain;
using UnityEngine;

[CreateAssetMenu]
public class ColorPoints : ScriptableObject
{
    [Header("Color Value Points")]
    
    [SerializeField, Tooltip("Score awarded for each circle color.")]
    private List<ColorPoint> _colorValuePoints;

    public IReadOnlyList<ColorPoint> ColorValuePoints => _colorValuePoints;

    public ColorPoints()
    {
        _colorValuePoints = new List<ColorPoint>();
        
        for (int i = 1; i < Enum.GetValues(typeof(CircleColor)).Length; i++)
        {
            ColorPoint colorPoint = new ColorPoint
            {
                colorValue = 1,
                circleColor = (CircleColor)i
            };

            _colorValuePoints.Add(colorPoint);
        }
    }

    public int GetScoreForColor(CircleColor circleColor)
    {
        return _colorValuePoints.FirstOrDefault(colorPoint => colorPoint.circleColor == circleColor).colorValue;
    }

    public ScoreCalculator CreateScoreCalculator()
    {
        return new ScoreCalculator(ToScoreValues());
    }

    public Dictionary<CellColor, int> ToScoreValues()
    {
        Dictionary<CellColor, int> scoreValues = new Dictionary<CellColor, int>();

        foreach (ColorPoint colorPoint in _colorValuePoints)
        {
            scoreValues[CircleColorMapper.ToCellColor(colorPoint.circleColor)] = colorPoint.colorValue;
        }

        return scoreValues;
    }
}

[Serializable]
public struct ColorPoint
{
    public CircleColor circleColor;
    public int colorValue;
}
