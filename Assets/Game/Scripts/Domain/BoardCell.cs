namespace Pendulum.Domain
{
    public readonly struct BoardCell
    {
        public BoardCell(BoardPosition position, CellColor color)
        {
            Position = position;
            Color = color;
        }

        public BoardPosition Position { get; }
        public CellColor Color { get; }
    }
}
