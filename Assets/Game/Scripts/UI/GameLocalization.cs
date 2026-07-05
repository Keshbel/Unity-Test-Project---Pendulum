using System;
using UnityEngine;

public enum GameLanguage
{
    English,
    Russian
}

public enum LocalizationKey
{
    MenuHint,
    StartGame,
    ScorePrefix,
    LanguageToggle,
    MenuTitle,
    RestartGame,
    ReturnToMenu
}

[CreateAssetMenu(menuName = "Pendulum/Localization Table")]
public class GameLocalization : ScriptableObject
{
    [SerializeField] private GameLanguage _defaultLanguage = GameLanguage.English;

    [SerializeField] private LocalizedText[] _texts = Array.Empty<LocalizedText>();

    public event Action OnLanguageChanged;

    public GameLanguage CurrentLanguage { get; private set; }

    private void OnEnable()
    {
        CurrentLanguage = _defaultLanguage;
    }

    public string Get(LocalizationKey key)
    {
        foreach (LocalizedText localizedText in _texts)
        {
            if (localizedText.Key == key)
                return localizedText.Get(CurrentLanguage);
        }

        return key.ToString();
    }

    public void ToggleLanguage()
    {
        CurrentLanguage = CurrentLanguage == GameLanguage.English
            ? GameLanguage.Russian
            : GameLanguage.English;

        OnLanguageChanged?.Invoke();
    }
}

[Serializable]
public struct LocalizedText
{
    [SerializeField] private LocalizationKey _key;

    [SerializeField] private string _english;

    [SerializeField] private string _russian;

    public LocalizedText(LocalizationKey key, string english, string russian)
    {
        _key = key;
        _english = english;
        _russian = russian;
    }

    public LocalizationKey Key => _key;

    public string Get(GameLanguage language)
    {
        string localizedValue = language == GameLanguage.Russian ? _russian : _english;
        return string.IsNullOrEmpty(localizedValue) ? _english : localizedValue;
    }
}
