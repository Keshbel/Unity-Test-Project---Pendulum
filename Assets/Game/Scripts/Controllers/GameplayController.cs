using System;
using System.Linq;
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

    private void Awake()
    {
        IsGame = false;
        ReturnToMainMenu();
        
        if (ColorPoints) return;

        var load = Resources.Load<ColorPoints>("Color Points");
        if (load) ColorPoints = load;
    }

    public void AddScore(CircleColor circleColor)
    {
        var score = ColorPoints.GetScoreForColor(circleColor);
        Score += score;
        
        OnAddScore?.Invoke(score);
    }
    
    public void StartGame()
    {
        IsGame = true;
        Score = 0;
        
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Game);
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
}
