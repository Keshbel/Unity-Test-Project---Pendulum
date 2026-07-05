using System.Collections.Generic;
using Pendulum.Domain;
using UnityEngine;

public class TriggerGridChecker : MonoBehaviour
{
    [Header("Delay")]
    
    [SerializeField, Tooltip("Delay between grid checks in seconds.")]
    private float _checkerDelay = 2f;
    
    private TriggerGridBuilder TriggerGridBuilder { get; set; }
    private GameSession GameSession { get; set; }
    private GameplayController GameplayController { get; set; }
    private EffectsController EffectsController { get; set; }
    private ColorPoints ColorPoints { get; set; }

    public void Construct(GameplayController gameplayController, EffectsController effectsController, ColorPoints colorPoints)
    {
        GameplayController = gameplayController;
        EffectsController = effectsController;
        ColorPoints = colorPoints;
    }

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
        ScheduleCheck();
    }

    public void StopChecking()
    {
        CancelInvoke(nameof(CheckGrid));
    }

    public void ScheduleCheck()
    {
        if (IsInvoking(nameof(CheckGrid)))
            return;

        Invoke(nameof(CheckGrid), _checkerDelay);
    }

    public void ResetBoard()
    {
        if (TriggerGridBuilder?.TriggerInfos == null)
            return;

        GameSession = CreateGameSession(CreateBoardSnapshot());
    }

    public void CheckGrid()
    {
        if (TriggerGridBuilder.TriggerInfos == null)
            return;

        if (!GameplayController || GameplayController.State != GameState.Playing)
            return;

        var board = CreateBoardSnapshot();
        GameSession ??= CreateGameSession(board);

        if (GameSession == null)
            return;

        GameSession.ReplaceBoard(board);

        var result = GameSession.ResolveBoard();
        if (result.HasMatches)
        {
            GameplayController.SetResolving();
            ApplyMatches(result);
            GameplayController.SetPlaying();
            return;
        }

        if (result.IsGameOver)
            GameplayController.EndGame();
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
        var colorPoints = ColorPoints ? ColorPoints : GameplayController?.ColorPoints;
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
        GameplayController.AddScore(result.ScoreAwarded);

        EffectsController?.PlayExplosionEffect();
            
        // Wake sleeping bodies before removing the matched line so physics contacts refresh.
        foreach (TriggerInfo triggerInfo in TriggerGridBuilder.TriggerInfos)
        {
            triggerInfo.WakeUpRigidbody2D();
        }

        HashSet<BoardPosition> matchedPositions = GetMatchedPositions(result.Matches);

        foreach (BoardPosition position in matchedPositions)
        {
            TriggerGridBuilder.TriggerInfos[position.Column, position.Row].DestroyCircle();
        }

        ApplyCollapse(result.CollapseMoves);
        ScheduleCheck();
    }

    private void ApplyCollapse(IEnumerable<BoardMove> moves)
    {
        foreach (BoardMove move in moves)
        {
            TriggerInfo sourceCell = TriggerGridBuilder.TriggerInfos[move.Source.Column, move.Source.Row];
            CircleObject circle = sourceCell.ReleaseCircle();

            if (!circle)
                continue;

            TriggerGridBuilder.TriggerInfos[move.Target.Column, move.Target.Row].PlaceCircle(circle);
        }
    }

    private static HashSet<BoardPosition> GetMatchedPositions(IEnumerable<MatchLine> matches)
    {
        HashSet<BoardPosition> positions = new HashSet<BoardPosition>();

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
