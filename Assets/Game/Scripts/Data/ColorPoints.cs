using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ColorPoints : ScriptableObject
{
    [field: Header("Color Value Points")]
    
    [field: SerializeField, Tooltip("Число очков за цвет.")] 
    public List<ColorPoint> ColorValuePoints { get; private set; }

    public ColorPoints()
    {
        ColorValuePoints = new List<ColorPoint>();
        
        for (int i = 1; i < Enum.GetValues(typeof(CircleColor)).Length; i++)
        {
            var colorPoint = new ColorPoint
            {
                colorValue = 1,
                circleColor = (CircleColor)i
            };

            ColorValuePoints.Add(colorPoint);
        }
    }

    public int GetScoreForColor(CircleColor circleColor)
    {
        return ColorValuePoints.FirstOrDefault(colorPoint => colorPoint.circleColor == circleColor).colorValue;
    }
}

[Serializable]
public struct ColorPoint
{
    public CircleColor circleColor;
    public int colorValue;
}
