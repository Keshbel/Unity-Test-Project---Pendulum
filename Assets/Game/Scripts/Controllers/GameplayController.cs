using System;
using System.Linq;
using Pendulum.Domain;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public Action<int> OnAddScore;
    
    [field: Header("States")]
    
    [field: SerializeField]
    public bool IsGame { get; set; }
    
    
    [field: Header("Options")]
    
    [field: SerializeField, Tooltip("Score value for each circle color.")]
    public ColorPoints ColorPoints { get; private set; }
    
    public int Score { get; private set; }

    private ScoreCalculator ScoreCalculator { get; set; }

    private void Awake()
    {
        IsGame = false;
        ReturnToMainMenu();
        
        if (!ColorPoints)
        {
            var load = Resources.Load<ColorPoints>("Color Points");
            if (load) ColorPoints = load;
        }

        BuildScoreCalculator();
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
        IsGame = true;
        Score = 0;
        
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Game);
        GameSingleton.Instance.TriggerGridChecker.ResetBoard();
    }
    
    public void EndGame()
    {
        IsGame = false;
        
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Stats);
        
        var circles = FindObjectsByType<CircleObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        circles.ForEach(circle => Destroy(circle.gameObject));
    }

    public static void ReturnToMainMenu()
    {
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Menu);
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

}
