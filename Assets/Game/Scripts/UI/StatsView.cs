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

    private void Awake()
    {
        RestartButton.onClick.AddListener(GameSingleton.Instance.GameplayController.StartGame);
        ReturnToMenuButton.onClick.AddListener(GameplayController.ReturnToMainMenu);
    }

    private void OnEnable()
    {
        ScoreText.text = "Score = " + GameSingleton.Instance.GameplayController.Score;
    }
}
