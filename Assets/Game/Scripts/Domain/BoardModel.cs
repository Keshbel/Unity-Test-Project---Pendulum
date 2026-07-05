using System;
using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class BoardModel
    {
        private readonly CellColor[,] _cells;

        public BoardModel(int columns, int rows)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException(nameof(columns));

            if (rows <= 0)
                throw new ArgumentOutOfRangeException(nameof(rows));

            Columns = columns;
            Rows = rows;
            _cells = new CellColor[columns, rows];
        }

        public int Columns { get; }
        public int Rows { get; }

        public CellColor GetCell(int column, int row)
        {
            ValidatePosition(column, row);
            return _cells[column, row];
        }

        public void SetCell(int column, int row, CellColor color)
        {
            ValidatePosition(column, row);
            _cells[column, row] = color;
        }

        public BoardCell GetBoardCell(int column, int row)
        {
            return new BoardCell(new BoardPosition(column, row), GetCell(column, row));
        }

        public bool IsFull()
        {
            for (var column = 0; column < Columns; column++)
            {
                for (var row = 0; row < Rows; row++)
                {
                    if (_cells[column, row] == CellColor.None)
                        return false;
                }
            }

            return true;
        }

        public BoardModel Clone()
        {
            var clone = new BoardModel(Columns, Rows);

            for (var column = 0; column < Columns; column++)
            {
                for (var row = 0; row < Rows; row++)
                {
                    clone.SetCell(column, row, _cells[column, row]);
                }
            }

            return clone;
        }

        public IReadOnlyList<BoardMove> RemoveAndCollapse(IEnumerable<BoardPosition> clearedPositions)
        {
            if (clearedPositions == null)
                throw new ArgumentNullException(nameof(clearedPositions));

            var cleared = new HashSet<BoardPosition>(clearedPositions);
            var moves = new List<BoardMove>();

            foreach (BoardPosition position in cleared)
                SetCell(position.Column, position.Row, CellColor.None);

            for (int column = 0; column < Columns; column++)
            {
                int targetRow = Rows - 1;

                for (int sourceRow = Rows - 1; sourceRow >= 0; sourceRow--)
                {
                    CellColor color = GetCell(column, sourceRow);
                    if (color == CellColor.None)
                        continue;

                    if (targetRow != sourceRow)
                    {
                        SetCell(column, targetRow, color);
                        SetCell(column, sourceRow, CellColor.None);
                        moves.Add(new BoardMove(
                            new BoardPosition(column, sourceRow),
                            new BoardPosition(column, targetRow)));
                    }

                    targetRow--;
                }

                for (int row = targetRow; row >= 0; row--)
                    SetCell(column, row, CellColor.None);
            }

            return moves;
        }

        private void ValidatePosition(int column, int row)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column));

            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row));
        }
    }
}
