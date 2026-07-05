using System;
using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class MatchLine
    {
        public MatchLine(MatchLineType type, CellColor color, IReadOnlyList<BoardPosition> positions)
        {
            if (color == CellColor.None)
                throw new ArgumentException("Matched lines must have a color.", nameof(color));

            if (positions == null || positions.Count == 0)
                throw new ArgumentException("A matched line must include positions.", nameof(positions));

            Type = type;
            Color = color;
            Positions = positions;
        }

        public MatchLineType Type { get; }
        public CellColor Color { get; }
        public IReadOnlyList<BoardPosition> Positions { get; }
    }
}
