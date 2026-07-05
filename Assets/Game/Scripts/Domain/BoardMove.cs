namespace Pendulum.Domain
{
    public readonly struct BoardMove
    {
        public BoardMove(BoardPosition source, BoardPosition target)
        {
            Source = source;
            Target = target;
        }

        public BoardPosition Source { get; }
        public BoardPosition Target { get; }
    }
}
