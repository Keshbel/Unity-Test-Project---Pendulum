using System;
using System.Collections.Generic;

namespace Pendulum.Domain
{
    public sealed class ScoreCalculator
    {
        private readonly IReadOnlyDictionary<CellColor, int> _scoreByColor;

        public ScoreCalculator(IReadOnlyDictionary<CellColor, int> scoreByColor)
        {
            _scoreByColor = scoreByColor ?? throw new ArgumentNullException(nameof(scoreByColor));
        }

        public int Calculate(CellColor color)
        {
            if (color == CellColor.None)
                return 0;

            if (!_scoreByColor.TryGetValue(color, out var score))
            {
                throw new InvalidOperationException($"Missing score config for color '{color}'.");
            }

            return score;
        }

        public int Calculate(IEnumerable<MatchLine> matches)
        {
            if (matches == null)
                throw new ArgumentNullException(nameof(matches));

            var total = 0;
            foreach (var match in matches)
            {
                total += Calculate(match.Color);
            }

            return total;
        }
    }
}
