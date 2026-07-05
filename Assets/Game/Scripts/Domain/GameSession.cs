#nullable enable

using System;
using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class GameSession
    {
        private readonly MatchDetector _matchDetector;
        private readonly ScoreCalculator _scoreCalculator;

        public GameSession(BoardModel board, MatchDetector matchDetector, ScoreCalculator scoreCalculator)
        {
            Board = board ?? throw new ArgumentNullException(nameof(board));
            _matchDetector = matchDetector ?? throw new ArgumentNullException(nameof(matchDetector));
            _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        }

        public BoardModel Board { get; private set; }
        public int Score { get; private set; }

        public void ReplaceBoard(BoardModel board)
        {
            Board = board ?? throw new ArgumentNullException(nameof(board));
        }

        public void Reset(BoardModel? board = null)
        {
            Board = board ?? new BoardModel(Board.Columns, Board.Rows);
            Score = 0;
        }

        public BoardResolutionResult ResolveBoard()
        {
            var matches = _matchDetector.FindMatches(Board);
            var scoreAwarded = _scoreCalculator.Calculate(matches);
            IReadOnlyList<BoardMove> collapseMoves = Array.Empty<BoardMove>();
            var isGameOver = matches.Count == 0 && Board.IsFull();

            if (matches.Count > 0)
            {
                collapseMoves = Board.RemoveAndCollapse(GetMatchedPositions(matches));
                isGameOver = false;
            }

            Score += scoreAwarded;

            return new BoardResolutionResult(matches, collapseMoves, scoreAwarded, isGameOver);
        }

        private static IEnumerable<BoardPosition> GetMatchedPositions(IEnumerable<MatchLine> matches)
        {
            var positions = new HashSet<BoardPosition>();

            foreach (MatchLine match in matches)
            {
                foreach (BoardPosition position in match.Positions)
                {
                    positions.Add(position);
                }
            }

            return positions;
        }
    }
}
