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
    
    [field: SerializeField, Tooltip("Ценность цветов.")]
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
        // стартовые параметры
        IsGame = true;
        Score = 0;
        
        // перейти ко второму экрану
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Game);
    }
    
    public void EndGame()
    {
        IsGame = false;
        
        // вызвать третий экран c количеством очков и кнопками
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Stats);
        
        // очищаем поле
        var circles = FindObjectsByType<CircleObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        circles.ForEach(circle => Destroy(circle.gameObject));
    }

    public static void ReturnToMainMenu()
    {
        GameSingleton.Instance.ScreenManager.SetGameScreen(GameScreen.Menu);
    }
}
