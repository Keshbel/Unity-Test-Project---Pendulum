using UnityEngine;
using UnityEngine.UI;

public class MenuView : MonoBehaviour
{
    [field: Header("UI")]
    
    [field: SerializeField] 
    private Button StartGameButton { get; set; }
    private GameplayController GameplayController { get; set; }

    public void Construct(GameplayController gameplayController)
    {
        GameplayController = gameplayController;
        Bind();
    }

    private void Awake()
    {
        if (!StartGameButton) StartGameButton = GetComponentInChildren<Button>();
        Bind();
    }

    private void Bind()
    {
        if (!StartGameButton || !GameplayController) return;

        StartGameButton.onClick.RemoveListener(GameplayController.StartGame);
        StartGameButton.onClick.AddListener(GameplayController.StartGame);
    }
}
