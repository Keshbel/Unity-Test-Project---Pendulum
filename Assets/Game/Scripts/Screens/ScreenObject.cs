using UnityEngine;

public class ScreenObject : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    public GameScreen GameScreen => _gameScreen;
}

public enum GameScreen
{
    Menu,
    Game,
    Stats
}
