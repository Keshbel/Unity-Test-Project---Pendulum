using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GameSingleton : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameplayController gameplayController;
    [SerializeField] private TriggerGridChecker triggerGridChecker;
    [SerializeField] private TriggerGridBuilder triggerGridBuilder;
    [SerializeField] private ScreenManager screenManager;
    [SerializeField] private EffectsController effectsController;
    [SerializeField] private ColorPoints colorPoints;
    [SerializeField] private SpawnCircleController spawnCircleController;
    [SerializeField] private PendulumManager pendulumManager;
    [SerializeField] private MenuView menuView;
    [SerializeField] private StatsView statsView;
    [SerializeField] private AddScoreAnimation addScoreAnimation;

    private GameplayController _gameplayController;
    public GameplayController GameplayController => _gameplayController;

    private TriggerGridChecker _triggerGridChecker;
    public TriggerGridChecker TriggerGridChecker => _triggerGridChecker;

    private ScreenManager _screenManager;
    public ScreenManager ScreenManager => _screenManager;

    private EffectsController _effectsController;
    public EffectsController EffectsController => _effectsController;
    
    private static GameSingleton _instance;
    public static GameSingleton Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameSingleton>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (Instance && Instance != this) Destroy(gameObject);
        else
        {
            _instance = this;
            WireScene();
        }
    }

    private void WireScene()
    {
        ResolveReferences();
        ValidateReferences();

        _gameplayController = gameplayController;
        _triggerGridChecker = triggerGridChecker;
        _screenManager = screenManager;
        _effectsController = effectsController;

        gameplayController?.Construct(screenManager, triggerGridChecker, colorPoints, spawnCircleController);
        triggerGridChecker?.Construct(gameplayController, effectsController, colorPoints);
        triggerGridBuilder?.Construct(gameplayController);
        pendulumManager?.Construct(gameplayController, triggerGridChecker);
        menuView?.Construct(gameplayController);
        statsView?.Construct(gameplayController);
        addScoreAnimation?.Construct(gameplayController);
    }

    private void ResolveReferences()
    {
        gameplayController = gameplayController ? gameplayController : FindObjectOfType<GameplayController>(true);
        triggerGridChecker = triggerGridChecker ? triggerGridChecker : FindObjectOfType<TriggerGridChecker>(true);
        triggerGridBuilder = triggerGridBuilder ? triggerGridBuilder : FindObjectOfType<TriggerGridBuilder>(true);
        screenManager = screenManager ? screenManager : FindObjectOfType<ScreenManager>(true);
        effectsController = effectsController ? effectsController : FindObjectOfType<EffectsController>(true);
        spawnCircleController = spawnCircleController ? spawnCircleController : FindObjectOfType<SpawnCircleController>(true);
        pendulumManager = pendulumManager ? pendulumManager : FindObjectOfType<PendulumManager>(true);
        menuView = menuView ? menuView : FindObjectOfType<MenuView>(true);
        statsView = statsView ? statsView : FindObjectOfType<StatsView>(true);
        addScoreAnimation = addScoreAnimation ? addScoreAnimation : FindObjectOfType<AddScoreAnimation>(true);

        if (!colorPoints && gameplayController)
        {
            colorPoints = gameplayController.ColorPoints;
        }

        if (!colorPoints)
        {
            colorPoints = Resources.Load<ColorPoints>("Color Points");
        }
    }

    private void ValidateReferences()
    {
        LogMissing(gameplayController, nameof(gameplayController));
        LogMissing(triggerGridChecker, nameof(triggerGridChecker));
        LogMissing(triggerGridBuilder, nameof(triggerGridBuilder));
        LogMissing(screenManager, nameof(screenManager));
        LogMissing(effectsController, nameof(effectsController));
        LogMissing(colorPoints, nameof(colorPoints));
        LogMissing(spawnCircleController, nameof(spawnCircleController));
        LogMissing(pendulumManager, nameof(pendulumManager));
        LogMissing(menuView, nameof(menuView));
        LogMissing(statsView, nameof(statsView));
        LogMissing(addScoreAnimation, nameof(addScoreAnimation));
    }

    private void LogMissing(Object reference, string referenceName)
    {
        if (reference) return;

        Debug.LogError($"Game bootstrap is missing required scene reference: {referenceName}.", this);
    }
}
