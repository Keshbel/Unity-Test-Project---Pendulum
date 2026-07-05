using DG.Tweening;
using TMPro;
using UnityEngine;

public class AddScoreAnimation : MonoBehaviour
{
    [Header("UI")]
    
    [SerializeField] private TMP_Text _scoreText;


    [Header("Options")]
    
    [SerializeField] private float _duration = 2f;

    [SerializeField] private float _maxScale = 1.5f;
    
    private Tweener _tweener;
    private GameplayController GameplayController { get; set; }

    public void Construct(GameplayController gameplayController)
    {
        if (GameplayController != null)
            GameplayController.OnAddScore -= SetAdditionalScore;

        GameplayController = gameplayController;
        if (GameplayController != null)
            GameplayController.OnAddScore += SetAdditionalScore;
    }

    private void Awake()
    {
        if (!_scoreText)
            _scoreText = GetComponent<TMP_Text>();
    }

    private void OnDestroy()
    {
        if (GameplayController != null)
            GameplayController.OnAddScore -= SetAdditionalScore;
    }

    private void StartAnimation()
    {
        _tweener?.Kill();
        
        _tweener = _scoreText.transform.DOScale(_maxScale, _duration).OnComplete(() =>
        {
            _tweener = _scoreText.transform.DOScale(0, _duration);
        });
    }

    private void SetAdditionalScore(int score)
    {
        _scoreText.text = "+" + score;
        StartAnimation();
    }
}
