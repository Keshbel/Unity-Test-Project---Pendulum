using UnityEngine;

public class ScreenObject : MonoBehaviour
{
    [field: SerializeField]
    public GameScreen GameScreen { get; private set; }
}

public enum GameScreen
{
    Menu,
    Game,
    Stats
}
