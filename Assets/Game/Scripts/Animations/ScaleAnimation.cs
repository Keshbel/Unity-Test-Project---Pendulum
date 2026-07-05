using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [Header("Options")]
    
    [SerializeField, Tooltip("Duration of one scale animation step.")]
    private float _duration = 0.5f;
    
    [SerializeField] private float _scaleMin = 0.75f;
    
    [SerializeField] private float _scaleMax = 1.5f;
    
    private Tweener _shakeTweener;

    private void OnEnable()
    {
        transform.localScale = new Vector3(_scaleMin, _scaleMin, _scaleMin);
        
        StartAnimation();
    }

    private void OnDisable()
    {
        _shakeTweener?.Kill();
    }

    private void StartAnimation()
    {
        _shakeTweener?.Kill();
        _shakeTweener = transform.DOScale(_scaleMax, _duration).SetLoops(-1, LoopType.Yoyo);
    }
}
