using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button _startGameButton;

    [SerializeField] private TMP_Text _startGameText;

    [SerializeField] private TMP_Text _titleText;

    [SerializeField] private TMP_Text _hintText;

    [SerializeField] private Button _languageButton;

    [SerializeField] private TMP_Text _languageButtonText;

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
        Bind();
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

    private void Bind()
    {
        if (_startGameButton && _gameplayController)
        {
            _startGameButton.onClick.RemoveListener(_gameplayController.StartGame);
            _startGameButton.onClick.AddListener(_gameplayController.StartGame);
        }

        if (_languageButton && _localization)
        {
            _languageButton.onClick.RemoveListener(_localization.ToggleLanguage);
            _languageButton.onClick.AddListener(_localization.ToggleLanguage);
        }
    }

    private void ValidateReferences()
    {
        LogMissing(_startGameButton, nameof(_startGameButton));
        LogMissing(_startGameText, nameof(_startGameText));
        LogMissing(_titleText, nameof(_titleText));
        LogMissing(_hintText, nameof(_hintText));
    }

    private void RefreshLocalizedText()
    {
        if (!_localization)
            return;

        if (_titleText)
            _titleText.text = _localization.Get(LocalizationKey.MenuTitle);

        if (_hintText)
            _hintText.text = _localization.Get(LocalizationKey.MenuHint);

        if (_languageButtonText)
            _languageButtonText.text = _localization.Get(LocalizationKey.LanguageToggle);

        if (_startGameText)
            _startGameText.text = _localization.Get(LocalizationKey.StartGame);
    }

    private void LogMissing(Object reference, string referenceName)
    {
        if (reference)
            return;

        Debug.LogError($"MenuView is missing required UI reference: {referenceName}.", this);
    }
}
