using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [field: Header("UI")]
    
    [field: SerializeField] 
    private Button StartGameButton { get; set; }

    private void Awake()
    {
        if (!StartGameButton) StartGameButton = GetComponentInChildren<Button>();
        
        StartGameButton.onClick.AddListener(GameSingleton.Instance.GameplayController.StartGame);
    }
}
