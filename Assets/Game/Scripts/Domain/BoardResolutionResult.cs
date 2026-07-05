using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class BoardResolutionResult
    {
        public BoardResolutionResult(IReadOnlyList<MatchLine> matches, int scoreAwarded, bool isGameOver)
        {
            Matches = matches;
            ScoreAwarded = scoreAwarded;
            IsGameOver = isGameOver;
        }

        public IReadOnlyList<MatchLine> Matches { get; }
        public int ScoreAwarded { get; }
        public bool IsGameOver { get; }
        public bool HasMatches => Matches.Count > 0;
    }
}
