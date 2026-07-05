using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameScreen GameScreen { get; private set; }

    [field: Header("Screens")]
    
    [field: SerializeField, Tooltip("All game screens in display order.")]
    public List<ScreenObject> Screens { get; private set; } = new();

    public void SetGameScreen(GameScreen gameScreen)
    {
        if (GameScreen == gameScreen && IsOnlyScreenActive(gameScreen)) return;

        Screens.ForEach(screen => screen.gameObject.SetActive(false));
        Screens[(int)gameScreen].gameObject.SetActive(true);

        GameScreen = gameScreen;
    }

    private bool IsOnlyScreenActive(GameScreen gameScreen)
    {
        for (var i = 0; i < Screens.Count; i++)
        {
            var shouldBeActive = i == (int)gameScreen;
            if (Screens[i].gameObject.activeSelf != shouldBeActive)
            {
                return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Screens.Count == 0 || Application.isPlaying) return;
        
        Screens = Screens.OrderBy(screen => (int)screen.GameScreen).ToList();
    }
#endif
}
