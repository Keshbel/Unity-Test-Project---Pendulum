using DG.Tweening;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [field: Header("Options")]
    
    [field: SerializeField, Tooltip("Время между изменение от минимального до максимального значения Scale.")]
    private float Duration { get; set; } = 0.5f;
    
    [field: SerializeField]
    private float ScaleMin { get; set; } = 0.75f;
    
    [field: SerializeField]
    private float ScaleMax { get; set; } = 1.5f;
    
    private Tweener _shakeTweener;

    private void OnEnable()
    {
        transform.localScale = new Vector3(ScaleMin, ScaleMin, ScaleMin);
        
        StartAnimation();
    }

    private void OnDisable()
    {
        _shakeTweener?.Kill();
    }

    private void StartAnimation()
    {
        _shakeTweener?.Kill();
        _shakeTweener = transform.DOScale(ScaleMax, Duration).SetLoops(-1, LoopType.Yoyo);
    }
}
