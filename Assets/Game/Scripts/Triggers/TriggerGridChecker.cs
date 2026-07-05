using System.Collections.Generic;
using Pendulum.Domain;
using UnityEngine;

public class TriggerGridChecker : MonoBehaviour
{
    [field: Header("Delay")]
    
    [field: SerializeField, Tooltip("Delay between grid checks in seconds.")]
    private float CheckerDelay { get; set; } = 2f;
    
    private TriggerGridBuilder TriggerGridBuilder { get; set; }
    private GameSession GameSession { get; set; }

    private void Awake()
    {
        TriggerGridBuilder = GetComponent<TriggerGridBuilder>();
    }

    private void OnEnable()
    {
        StartChecking();
    }

    private void OnDisable()
    {
        StopChecking();
    }

    public void StartChecking()
    {
        StopChecking();
        Invoke(nameof(CheckGrid), CheckerDelay);
    }

    public void StopChecking()
    {
        CancelInvoke(nameof(CheckGrid));
    }

    public void ResetBoard()
    {
        if (TriggerGridBuilder?.TriggerInfos == null) return;

        GameSession = CreateGameSession(CreateBoardSnapshot());
    }

    public void CheckGrid()
    {
        if (TriggerGridBuilder.TriggerInfos == null) return;

        var board = CreateBoardSnapshot();
        GameSession ??= CreateGameSession(board);
        if (GameSession == null) return;

        GameSession.ReplaceBoard(board);

        var result = GameSession.ResolveBoard();
        if (result.HasMatches)
        {
            ApplyMatches(result);
            return;
        }

        if (result.IsGameOver)
        {
            GameSingleton.Instance.GameplayController.EndGame();
        }
    }

    private BoardModel CreateBoardSnapshot()
    {
        var board = new BoardModel(TriggerGridBuilder.Column, TriggerGridBuilder.Row);

        for (var column = 0; column < TriggerGridBuilder.Column; column++)
        {
            for (var row = 0; row < TriggerGridBuilder.Row; row++)
            {
                board.SetCell(column, row, CircleColorMapper.ToCellColor(TriggerGridBuilder.TriggerInfos[column, row].GetColor()));
            }
        }

        return board;
    }

    private GameSession CreateGameSession(BoardModel board)
    {
        var colorPoints = GameSingleton.Instance.GameplayController.ColorPoints;
        if (!colorPoints)
        {
            Debug.LogError("ColorPoints config is missing. Grid rules cannot resolve scores.", this);
            return null;
        }

        return new GameSession(
            board,
            new MatchDetector(),
            colorPoints.CreateScoreCalculator());
    }

    private void ApplyMatches(BoardResolutionResult result)
    {
        GameSingleton.Instance.GameplayController.AddScore(result.ScoreAwarded);

        GameSingleton.Instance.EffectsController.PlayExplosionEffect();
            
        // Wake sleeping bodies before removing the matched line so physics contacts refresh.
        foreach (var triggerInfo in TriggerGridBuilder.TriggerInfos)
        {
            triggerInfo.WakeUpRigidbody2D();
        }

        foreach (var position in GetMatchedPositions(result.Matches))
        {
            TriggerGridBuilder.TriggerInfos[position.Column, position.Row].DestroyCircle();
        }
    }

    private static HashSet<BoardPosition> GetMatchedPositions(IEnumerable<MatchLine> matches)
    {
        var positions = new HashSet<BoardPosition>();

        foreach (var match in matches)
        {
            foreach (var position in match.Positions)
            {
                positions.Add(position);
            }
        }

        return positions;
    }

}
