using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class MatchDetector
    {
        public IReadOnlyList<MatchLine> FindMatches(BoardModel board)
        {
            var matches = new List<MatchLine>();

            AddHorizontalMatches(board, matches);
            AddVerticalMatches(board, matches);
            AddDiagonalMatches(board, matches);

            return matches;
        }

        private static void AddHorizontalMatches(BoardModel board, ICollection<MatchLine> matches)
        {
            for (var row = 0; row < board.Rows; row++)
            {
                var positions = new List<BoardPosition>();
                var firstColor = board.GetCell(0, row);
                if (firstColor == CellColor.None) continue;

                for (var column = 0; column < board.Columns; column++)
                {
                    if (board.GetCell(column, row) != firstColor)
                    {
                        positions.Clear();
                        break;
                    }

                    positions.Add(new BoardPosition(column, row));
                }

                if (positions.Count == board.Columns)
                {
                    matches.Add(new MatchLine(MatchLineType.Horizontal, firstColor, positions));
                }
            }
        }

        private static void AddVerticalMatches(BoardModel board, ICollection<MatchLine> matches)
        {
            for (var column = 0; column < board.Columns; column++)
            {
                var positions = new List<BoardPosition>();
                var firstColor = board.GetCell(column, 0);
                if (firstColor == CellColor.None) continue;

                for (var row = 0; row < board.Rows; row++)
                {
                    if (board.GetCell(column, row) != firstColor)
                    {
                        positions.Clear();
                        break;
                    }

                    positions.Add(new BoardPosition(column, row));
                }

                if (positions.Count == board.Rows)
                {
                    matches.Add(new MatchLine(MatchLineType.Vertical, firstColor, positions));
                }
            }
        }

        private static void AddDiagonalMatches(BoardModel board, ICollection<MatchLine> matches)
        {
            if (board.Columns != board.Rows) return;

            AddMainDiagonalMatch(board, matches);
            AddAntiDiagonalMatch(board, matches);
        }

        private static void AddMainDiagonalMatch(BoardModel board, ICollection<MatchLine> matches)
        {
            var positions = new List<BoardPosition>();
            var firstColor = board.GetCell(0, 0);
            if (firstColor == CellColor.None) return;

            for (var index = 0; index < board.Columns; index++)
            {
                if (board.GetCell(index, index) != firstColor)
                {
                    return;
                }

                positions.Add(new BoardPosition(index, index));
            }

            matches.Add(new MatchLine(MatchLineType.MainDiagonal, firstColor, positions));
        }

        private static void AddAntiDiagonalMatch(BoardModel board, ICollection<MatchLine> matches)
        {
            var positions = new List<BoardPosition>();
            var firstColor = board.GetCell(0, board.Rows - 1);
            if (firstColor == CellColor.None) return;

            for (var column = 0; column < board.Columns; column++)
            {
                var row = board.Rows - 1 - column;
                if (board.GetCell(column, row) != firstColor)
                {
                    return;
                }

                positions.Add(new BoardPosition(column, row));
            }

            matches.Add(new MatchLine(MatchLineType.AntiDiagonal, firstColor, positions));
        }
    }
}
