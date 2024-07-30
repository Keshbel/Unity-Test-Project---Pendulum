using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GameSingleton : MonoBehaviour
{
    [Header("Gameplay Controller")]
    
    private GameplayController _gameplayController;
    public GameplayController GameplayController => ReturnObject(ref _gameplayController);

    
    [Header("Grid Checker")] 
    
    private TriggerGridChecker _triggerGridChecker;
    public TriggerGridChecker TriggerGridChecker => ReturnObject(ref _triggerGridChecker);

    
    [Header("Screen Manager")] 
    
    private ScreenManager _screenManager;
    public ScreenManager ScreenManager => ReturnObject(ref _screenManager);
    
    
    [Header("Effects Controller")]
    
    private EffectsController _effectsController;
    public EffectsController EffectsController => ReturnObject(ref _effectsController);
    
    #region Singleton
    
    private static GameSingleton _instance;
    public static GameSingleton Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameSingleton>();
                if (!_instance) _instance = new GameObject("Game Singleton").AddComponent<GameSingleton>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance && Instance != this) Destroy(gameObject);
        else _instance = this;
    }
    
    #endregion

    #region Utility

    private static T ReturnObject<T>(ref T component) where T : Component
    {
        if (!component) component = FindObjectOfType<T>(true);
        return component;
    }

    #endregion
}
