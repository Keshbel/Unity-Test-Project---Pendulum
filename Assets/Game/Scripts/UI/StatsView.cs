using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsView : MonoBehaviour
{
    [field: Header("Text")]
    
    [field: SerializeField] 
    private TMP_Text ScoreText { get; set; }
    
    
    [field: Header("Buttons")]
    
    [field: SerializeField]
    private Button RestartButton { get; set; }
    
    [field: SerializeField]
    private Button ReturnToMenuButton { get; set; }
    private GameplayController GameplayController { get; set; }

    public void Construct(GameplayController gameplayController)
    {
        GameplayController = gameplayController;
        Bind();
    }

    private void Awake()
    {
        Bind();
    }

    private void OnEnable()
    {
        if (ScoreText && GameplayController) ScoreText.text = "Score = " + GameplayController.Score;
    }

    private void Bind()
    {
        if (!GameplayController) return;

        if (RestartButton)
        {
            RestartButton.onClick.RemoveListener(GameplayController.StartGame);
            RestartButton.onClick.AddListener(GameplayController.StartGame);
        }

        if (ReturnToMenuButton)
        {
            ReturnToMenuButton.onClick.RemoveListener(GameplayController.ReturnToMainMenu);
            ReturnToMenuButton.onClick.AddListener(GameplayController.ReturnToMainMenu);
        }
    }
}
