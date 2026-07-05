using System;
using Pendulum.Domain;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public event Action<int> OnAddScore;
    public event Action<GameState> OnStateChanged;
    
    [field: Header("States")]
    
    [field: SerializeField]
    public bool IsGame { get; private set; }
    
    
    [field: Header("Options")]
    
    [field: SerializeField, Tooltip("Score value for each circle color.")]
    public ColorPoints ColorPoints { get; private set; }
    
    public int Score { get; private set; }
    public GameState State { get; private set; } = GameState.Menu;

    private ScoreCalculator ScoreCalculator { get; set; }
    private ScreenManager ScreenManager { get; set; }
    private TriggerGridChecker TriggerGridChecker { get; set; }
    private SpawnCircleController SpawnCircleController { get; set; }

    public void Construct(
        ScreenManager screenManager,
        TriggerGridChecker triggerGridChecker,
        ColorPoints colorPoints,
        SpawnCircleController spawnCircleController)
    {
        ScreenManager = screenManager;
        TriggerGridChecker = triggerGridChecker;
        SpawnCircleController = spawnCircleController;
        if (colorPoints) ColorPoints = colorPoints;

        BuildScoreCalculator();
        SetState(GameState.Menu);
    }

    private void Awake()
    {
        if (!ColorPoints)
        {
            var load = Resources.Load<ColorPoints>("Color Points");
            if (load) ColorPoints = load;
        }

        if (ColorPoints) BuildScoreCalculator();
        SetState(GameState.Menu);
    }

    public void AddScore(CircleColor circleColor)
    {
        EnsureScoreCalculator();
        AddScore(ScoreCalculator.Calculate(CircleColorMapper.ToCellColor(circleColor)));
    }

    public void AddScore(int score)
    {
        Score += score;
        OnAddScore?.Invoke(score);
    }
    
    public void StartGame()
    {
        Score = 0;
        SpawnCircleController?.ClearSpawnedCircles();
        TriggerGridChecker?.ResetBoard();
        SetState(GameState.Playing);
    }
    
    public void EndGame()
    {
        SetState(GameState.GameOver);
        SpawnCircleController?.ClearSpawnedCircles();
    }

    public void SetResolving()
    {
        SetState(GameState.Resolving);
    }

    public void SetPlaying()
    {
        SetState(GameState.Playing);
    }

    public void ReturnToMainMenu()
    {
        SetState(GameState.Menu);
    }

    private void BuildScoreCalculator()
    {
        if (!ColorPoints)
        {
            Debug.LogError("ColorPoints config is missing. Score calculation cannot be initialized.", this);
            return;
        }

        ScoreCalculator = ColorPoints.CreateScoreCalculator();
    }

    private void EnsureScoreCalculator()
    {
        if (ScoreCalculator != null) return;

        BuildScoreCalculator();
        if (ScoreCalculator == null)
        {
            throw new InvalidOperationException("ScoreCalculator is not initialized.");
        }
    }

    private void SetState(GameState state)
    {
        State = state;
        IsGame = state == GameState.Playing;

        if (ScreenManager)
        {
            ScreenManager.SetGameScreen(ToGameScreen(state));
        }

        OnStateChanged?.Invoke(state);
    }

    private static GameScreen ToGameScreen(GameState state)
    {
        return state switch
        {
            GameState.Playing => GameScreen.Game,
            GameState.Resolving => GameScreen.Game,
            GameState.GameOver => GameScreen.Stats,
            _ => GameScreen.Menu
        };
    }
}
