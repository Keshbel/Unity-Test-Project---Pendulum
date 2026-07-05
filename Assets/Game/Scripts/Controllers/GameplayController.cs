using System;
using Pendulum.Domain;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public event Action<int> OnAddScore;
    public event Action<GameState> OnStateChanged;
    
    [Header("States")]
    
    [SerializeField] private bool _isGame;
    
    
    [Header("Options")]
    
    [SerializeField, Tooltip("Score value for each circle color.")]
    private ColorPoints _colorPoints;
    
    public bool IsGame => _isGame;
    public ColorPoints ColorPoints => _colorPoints;
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

        if (colorPoints)
            _colorPoints = colorPoints;

        BuildScoreCalculator();
        SetState(GameState.Menu);
    }

    private void Awake()
    {
        if (!_colorPoints)
        {
            ColorPoints loadedColorPoints = Resources.Load<ColorPoints>("Color Points/Color Points Data");

            if (loadedColorPoints)
                _colorPoints = loadedColorPoints;
        }

        if (_colorPoints)
            BuildScoreCalculator();

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
        if (!_colorPoints)
        {
            Debug.LogError("ColorPoints config is missing. Score calculation cannot be initialized.", this);
            return;
        }

        ScoreCalculator = _colorPoints.CreateScoreCalculator();
    }

    private void EnsureScoreCalculator()
    {
        if (ScoreCalculator != null)
            return;

        BuildScoreCalculator();
        if (ScoreCalculator == null)
            throw new InvalidOperationException("ScoreCalculator is not initialized.");
    }

    private void SetState(GameState state)
    {
        State = state;
        _isGame = state == GameState.Playing;

        if (ScreenManager)
            ScreenManager.SetGameScreen(ToGameScreen(state));

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
