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

    private void Reset()
    {
        EnsureDefaultValues();
    }

    private void OnValidate()
    {
        EnsureDefaultValues();
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
        EnsureDefaultValues();

        foreach (ColorPoint colorPoint in _colorValuePoints)
        {
            scoreValues[CircleColorMapper.ToCellColor(colorPoint.circleColor)] = colorPoint.colorValue;
        }

        return scoreValues;
    }

    private void EnsureDefaultValues()
    {
        _colorValuePoints ??= new List<ColorPoint>();

        for (int i = 1; i < Enum.GetValues(typeof(CircleColor)).Length; i++)
        {
            CircleColor circleColor = (CircleColor)i;
            if (_colorValuePoints.Any(colorPoint => colorPoint.circleColor == circleColor))
                continue;

            _colorValuePoints.Add(new ColorPoint
            {
                colorValue = 1,
                circleColor = circleColor
            });
        }
    }
}

[Serializable]
public struct ColorPoint
{
    public CircleColor circleColor;
    public int colorValue;
}
