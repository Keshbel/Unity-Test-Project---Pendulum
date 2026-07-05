using Pendulum.Domain;

public static class CircleColorMapper
{
    public static CellColor ToCellColor(CircleColor circleColor)
    {
        return circleColor switch
        {
            CircleColor.Red => CellColor.Red,
            CircleColor.Green => CellColor.Green,
            CircleColor.Blue => CellColor.Blue,
            _ => CellColor.None
        };
    }
}
