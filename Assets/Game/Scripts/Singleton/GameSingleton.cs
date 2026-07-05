using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GameSingleton : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameplayController _gameplayController;
    [SerializeField] private TriggerGridChecker _triggerGridChecker;
    [SerializeField] private TriggerGridBuilder _triggerGridBuilder;
    [SerializeField] private ScreenManager _screenManager;
    [SerializeField] private EffectsController _effectsController;
    [SerializeField] private ColorPoints _colorPoints;
    [SerializeField] private SpawnCircleController _spawnCircleController;
    [SerializeField] private PendulumManager _pendulumManager;
    [SerializeField] private MenuView _menuView;
    [SerializeField] private StatsView _statsView;
    [SerializeField] private AddScoreAnimation _addScoreAnimation;
    [SerializeField] private GameLocalization _localization;

    private static GameSingleton _instance;

    public static GameSingleton Instance
    {
        get
        {
            if (!_instance)
                _instance = FindFirstObjectByType<GameSingleton>();

            return _instance;
        }
    }

    public GameplayController GameplayController => _gameplayController;
    public TriggerGridChecker TriggerGridChecker => _triggerGridChecker;
    public ScreenManager ScreenManager => _screenManager;
    public EffectsController EffectsController => _effectsController;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        WireScene();
    }

    private void WireScene()
    {
        ResolveReferences();
        ValidateReferences();

        _gameplayController?.Construct(_screenManager, _triggerGridChecker, _colorPoints, _spawnCircleController);
        _triggerGridChecker?.Construct(_gameplayController, _effectsController, _colorPoints);
        _triggerGridBuilder?.Construct(_gameplayController, _triggerGridChecker);
        _pendulumManager?.Construct(_gameplayController, _triggerGridChecker);
        _menuView?.Construct(_gameplayController);
        _menuView?.SetLocalization(_localization);
        _statsView?.Construct(_gameplayController);
        _statsView?.SetLocalization(_localization);
        _addScoreAnimation?.Construct(_gameplayController);
    }

    private void ResolveReferences()
    {
        _gameplayController = ResolveSceneReference(_gameplayController);
        _triggerGridChecker = ResolveSceneReference(_triggerGridChecker);
        _triggerGridBuilder = ResolveSceneReference(_triggerGridBuilder);
        _screenManager = ResolveSceneReference(_screenManager);
        _effectsController = ResolveSceneReference(_effectsController);
        _spawnCircleController = ResolveSceneReference(_spawnCircleController);
        _pendulumManager = ResolveSceneReference(_pendulumManager);
        _menuView = ResolveSceneReference(_menuView);
        _statsView = ResolveSceneReference(_statsView);
        _addScoreAnimation = ResolveSceneReference(_addScoreAnimation);

        if (!_colorPoints && _gameplayController)
            _colorPoints = _gameplayController.ColorPoints;

        if (!_colorPoints)
            _colorPoints = Resources.Load<ColorPoints>("Color Points/Color Points Data");

        if (!_localization)
            _localization = Resources.Load<GameLocalization>("Localization/Game Localization");
    }

    private void ValidateReferences()
    {
        LogMissing(_gameplayController, nameof(_gameplayController));
        LogMissing(_triggerGridChecker, nameof(_triggerGridChecker));
        LogMissing(_triggerGridBuilder, nameof(_triggerGridBuilder));
        LogMissing(_screenManager, nameof(_screenManager));
        LogMissing(_effectsController, nameof(_effectsController));
        LogMissing(_colorPoints, nameof(_colorPoints));
        LogMissing(_spawnCircleController, nameof(_spawnCircleController));
        LogMissing(_pendulumManager, nameof(_pendulumManager));
        LogMissing(_menuView, nameof(_menuView));
        LogMissing(_statsView, nameof(_statsView));
        LogMissing(_addScoreAnimation, nameof(_addScoreAnimation));
        LogMissing(_localization, nameof(_localization));
    }

    private static T ResolveSceneReference<T>(T reference) where T : Object
    {
        return reference ? reference : FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }

    private void LogMissing(Object reference, string referenceName)
    {
        if (reference)
            return;

        Debug.LogError($"Game bootstrap is missing required scene reference: {referenceName}.", this);
    }
}
