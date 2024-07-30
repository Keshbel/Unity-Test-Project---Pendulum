using DG.Tweening;
using TMPro;
using UnityEngine;

public class AddScoreAnimation : MonoBehaviour
{
    [field: Header("UI")]
    
    [field: SerializeField] 
    private TMP_Text ScoreText { get; set; }


    [field: Header("Options")]
    
    [field: SerializeField]
    private float Duration { get; set; } = 2f;

    [field: SerializeField] 
    private float MaxScale { get; set; } = 1.5f;
    
    private Tweener _tweener;

    private void Awake()
    {
        if (!ScoreText) ScoreText = GetComponent<TMP_Text>();
        
        GameSingleton.Instance.GameplayController.OnAddScore += SetAdditionalScore;
    }

    private void StartAnimation()
    {
        _tweener?.Kill();
        
        _tweener = ScoreText.transform.DOScale(MaxScale, Duration).OnComplete(() =>
        {
            _tweener = ScoreText.transform.DOScale(0, Duration);
        });
    }

    private void SetAdditionalScore(int score)
    {
        ScoreText.text = "+" + score;
        StartAnimation();
    }
}
