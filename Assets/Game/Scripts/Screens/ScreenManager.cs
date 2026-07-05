using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameScreen GameScreen { get; private set; }

    [Header("Screens")]
    
    [SerializeField, Tooltip("All game screens in display order.")]
    private List<ScreenObject> _screens = new List<ScreenObject>();

    public IReadOnlyList<ScreenObject> Screens => _screens;

    public void SetGameScreen(GameScreen gameScreen)
    {
        if (GameScreen == gameScreen && IsOnlyScreenActive(gameScreen))
            return;

        if (!TryGetScreen(gameScreen, out ScreenObject targetScreen))
        {
            Debug.LogError($"Screen '{gameScreen}' is not assigned in ScreenManager.", this);
            return;
        }

        foreach (ScreenObject screen in _screens)
        {
            if (screen)
                screen.gameObject.SetActive(false);
        }

        targetScreen.gameObject.SetActive(true);

        GameScreen = gameScreen;
    }

    private bool IsOnlyScreenActive(GameScreen gameScreen)
    {
        if (!TryGetScreen(gameScreen, out ScreenObject targetScreen))
            return false;

        foreach (ScreenObject screen in _screens)
        {
            if (!screen)
                continue;

            bool shouldBeActive = screen == targetScreen;
            if (screen.gameObject.activeSelf != shouldBeActive)
                return false;
        }

        return true;
    }

    private bool TryGetScreen(GameScreen gameScreen, out ScreenObject screenObject)
    {
        foreach (ScreenObject screen in _screens)
        {
            if (!screen)
                continue;

            if (screen.GameScreen == gameScreen)
            {
                screenObject = screen;
                return true;
            }
        }

        screenObject = null;
        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_screens.Count == 0 || Application.isPlaying)
            return;

        _screens = _screens.Where(screen => screen).ToList();
        _screens = _screens.OrderBy(screen => (int)screen.GameScreen).ToList();

        foreach (GameScreen gameScreen in Enum.GetValues(typeof(GameScreen)))
        {
            if (_screens.Any(screen => screen.GameScreen == gameScreen))
                continue;

            Debug.LogWarning($"Screen '{gameScreen}' is not assigned in ScreenManager.", this);
        }
    }
#endif
}
