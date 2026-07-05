using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsView : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text _scoreText;
    
    [Header("Buttons")]
    [SerializeField] private Button _restartButton;

    [SerializeField] private TMP_Text _restartButtonText;

    [SerializeField] private Button _returnToMenuButton;

    [SerializeField] private TMP_Text _returnToMenuButtonText;

    [Header("Localization")]
    [SerializeField] private GameLocalization _localization;

    private GameplayController _gameplayController;

    public void Construct(GameplayController gameplayController)
    {
        _gameplayController = gameplayController;
        Bind();
    }

    public void SetLocalization(GameLocalization gameLocalization)
    {
        if (_localization)
            _localization.OnLanguageChanged -= RefreshLocalizedText;

        _localization = gameLocalization;

        if (_localization)
            _localization.OnLanguageChanged += RefreshLocalizedText;

        RefreshLocalizedText();
    }

    private void Awake()
    {
        if (_localization)
            _localization.OnLanguageChanged += RefreshLocalizedText;

        ValidateReferences();
        Bind();
        RefreshLocalizedText();
    }

    private void OnDestroy()
    {
        if (_localization)
            _localization.OnLanguageChanged -= RefreshLocalizedText;
    }

    private void OnEnable()
    {
        RefreshLocalizedText();
    }

    private void Bind()
    {
        if (!_gameplayController)
            return;

        if (_restartButton)
        {
            _restartButton.onClick.RemoveListener(_gameplayController.StartGame);
            _restartButton.onClick.AddListener(_gameplayController.StartGame);
        }

        if (_returnToMenuButton)
        {
            _returnToMenuButton.onClick.RemoveListener(_gameplayController.ReturnToMainMenu);
            _returnToMenuButton.onClick.AddListener(_gameplayController.ReturnToMainMenu);
        }
    }

    private void RefreshScoreText()
    {
        if (!_scoreText || !_gameplayController)
            return;

        string scorePrefix = _localization ? _localization.Get(LocalizationKey.ScorePrefix) : "Score = ";
        _scoreText.text = scorePrefix + _gameplayController.Score;
    }

    private void RefreshLocalizedText()
    {
        RefreshScoreText();

        if (!_localization)
            return;

        if (_restartButtonText)
            _restartButtonText.text = _localization.Get(LocalizationKey.RestartGame);

        if (_returnToMenuButtonText)
            _returnToMenuButtonText.text = _localization.Get(LocalizationKey.ReturnToMenu);
    }

    private void ValidateReferences()
    {
        LogMissing(_scoreText, nameof(_scoreText));
        LogMissing(_restartButton, nameof(_restartButton));
        LogMissing(_restartButtonText, nameof(_restartButtonText));
        LogMissing(_returnToMenuButton, nameof(_returnToMenuButton));
        LogMissing(_returnToMenuButtonText, nameof(_returnToMenuButtonText));
    }

    private void LogMissing(Object reference, string referenceName)
    {
        if (reference)
            return;

        Debug.LogError($"StatsView is missing required UI reference: {referenceName}.", this);
    }
}
