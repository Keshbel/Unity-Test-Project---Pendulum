using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameScreen GameScreen { get; private set; }

    [field: Header("Screens")]
    
    [field: SerializeField, Tooltip("Все экраны игры.")]
    public List<ScreenObject> Screens { get; private set; } = new();

    public void SetGameScreen(GameScreen gameScreen)
    {
        Screens.ForEach(screen => screen.gameObject.SetActive(false));
        Screens[(int)gameScreen].gameObject.SetActive(true);

        GameScreen = gameScreen;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Screens.Count == 0 || Application.isPlaying) return;
        
        Screens = Screens.OrderBy(screen => (int)screen.GameScreen).ToList();
    }
#endif
}
