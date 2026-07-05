using System.Collections.Generic;
using NUnit.Framework;
using Pendulum.Domain;

namespace Pendulum.Tests.EditMode
{
    public sealed class DomainRulesTests
    {
        [Test]
        public void DetectsHorizontalMatch()
        {
            var board = CreateBoard();
            board.SetCell(0, 1, CellColor.Blue);
            board.SetCell(1, 1, CellColor.Blue);
            board.SetCell(2, 1, CellColor.Blue);

            var matches = new MatchDetector().FindMatches(board);

            Assert.That(matches, Has.Count.EqualTo(1));
            Assert.That(matches[0].Type, Is.EqualTo(MatchLineType.Horizontal));
            Assert.That(matches[0].Color, Is.EqualTo(CellColor.Blue));
        }

        [Test]
        public void DetectsVerticalMatch()
        {
            var board = CreateBoard();
            board.SetCell(2, 0, CellColor.Green);
            board.SetCell(2, 1, CellColor.Green);
            board.SetCell(2, 2, CellColor.Green);

            var matches = new MatchDetector().FindMatches(board);

            Assert.That(matches, Has.Count.EqualTo(1));
            Assert.That(matches[0].Type, Is.EqualTo(MatchLineType.Vertical));
            Assert.That(matches[0].Color, Is.EqualTo(CellColor.Green));
        }

        [Test]
        public void DetectsMainDiagonalMatch()
        {
            var board = CreateBoard();
            board.SetCell(0, 0, CellColor.Red);
            board.SetCell(1, 1, CellColor.Red);
            board.SetCell(2, 2, CellColor.Red);

            var matches = new MatchDetector().FindMatches(board);

            Assert.That(matches, Has.Count.EqualTo(1));
            Assert.That(matches[0].Type, Is.EqualTo(MatchLineType.MainDiagonal));
            Assert.That(matches[0].Color, Is.EqualTo(CellColor.Red));
        }

        [Test]
        public void DetectsAntiDiagonalMatch()
        {
            var board = CreateBoard();
            board.SetCell(0, 2, CellColor.Green);
            board.SetCell(1, 1, CellColor.Green);
            board.SetCell(2, 0, CellColor.Green);

            var matches = new MatchDetector().FindMatches(board);

            Assert.That(matches, Has.Count.EqualTo(1));
            Assert.That(matches[0].Type, Is.EqualTo(MatchLineType.AntiDiagonal));
            Assert.That(matches[0].Color, Is.EqualTo(CellColor.Green));
        }

        [Test]
        public void DoesNotDetectMatchesWithEmptyCells()
        {
            var board = CreateBoard();
            board.SetCell(0, 0, CellColor.Red);
            board.SetCell(2, 0, CellColor.Red);

            var matches = new MatchDetector().FindMatches(board);

            Assert.That(matches, Is.Empty);
        }

        [Test]
        public void FullBoardWithoutMatchesResolvesAsGameOver()
        {
            var board = CreateBoard();
            SetRow(board, 0, CellColor.Red, CellColor.Green, CellColor.Blue);
            SetRow(board, 1, CellColor.Blue, CellColor.Green, CellColor.Red);
            SetRow(board, 2, CellColor.Green, CellColor.Red, CellColor.Blue);

            var session = CreateSession(board);
            var result = session.ResolveBoard();

            Assert.That(result.HasMatches, Is.False);
            Assert.That(result.IsGameOver, Is.True);
            Assert.That(result.ScoreAwarded, Is.Zero);
        }

        [Test]
        public void CalculatesScoreByColor()
        {
            var calculator = new ScoreCalculator(CreateScoreValues());

            Assert.That(calculator.Calculate(CellColor.Red), Is.EqualTo(10));
            Assert.That(calculator.Calculate(CellColor.Green), Is.EqualTo(20));
            Assert.That(calculator.Calculate(CellColor.Blue), Is.EqualTo(30));
            Assert.That(calculator.Calculate(CellColor.None), Is.Zero);
        }

        [Test]
        public void ResolvesMultipleMatches()
        {
            var board = CreateBoard();
            for (var column = 0; column < board.Columns; column++)
            {
                for (var row = 0; row < board.Rows; row++)
                {
                    board.SetCell(column, row, CellColor.Red);
                }
            }

            var session = CreateSession(board);
            var result = session.ResolveBoard();

            Assert.That(result.Matches.Count, Is.GreaterThan(1));
            Assert.That(result.IsGameOver, Is.False);
            Assert.That(result.ScoreAwarded, Is.EqualTo(result.Matches.Count * 10));
            Assert.That(session.Score, Is.EqualTo(result.ScoreAwarded));
        }

        private static BoardModel CreateBoard()
        {
            return new BoardModel(3, 3);
        }

        private static GameSession CreateSession(BoardModel board)
        {
            return new GameSession(board, new MatchDetector(), new ScoreCalculator(CreateScoreValues()));
        }

        private static Dictionary<CellColor, int> CreateScoreValues()
        {
            return new Dictionary<CellColor, int>
            {
                [CellColor.Red] = 10,
                [CellColor.Green] = 20,
                [CellColor.Blue] = 30
            };
        }

        private static void SetRow(BoardModel board, int row, params CellColor[] colors)
        {
            for (var column = 0; column < colors.Length; column++)
            {
                board.SetCell(column, row, colors[column]);
            }
        }
    }
}
