using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class BoardResolutionResult
    {
        public BoardResolutionResult(
            IReadOnlyList<MatchLine> matches,
            IReadOnlyList<BoardMove> collapseMoves,
            int scoreAwarded,
            bool isGameOver)
        {
            Matches = matches;
            CollapseMoves = collapseMoves;
            ScoreAwarded = scoreAwarded;
            IsGameOver = isGameOver;
        }

        public IReadOnlyList<MatchLine> Matches { get; }
        public IReadOnlyList<BoardMove> CollapseMoves { get; }
        public int ScoreAwarded { get; }
        public bool IsGameOver { get; }
        public bool HasMatches => Matches.Count > 0;
    }
}
