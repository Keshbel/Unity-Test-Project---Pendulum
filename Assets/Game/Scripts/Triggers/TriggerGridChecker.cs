using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TriggerGridChecker : MonoBehaviour
{
    [field: Header("Delay")]
    
    [field: SerializeField, Tooltip("Delay between grid checks in seconds.")]
    private float CheckerDelay { get; set; } = 2f;
    
    private TriggerGridBuilder TriggerGridBuilder { get; set; }
    private CancellationTokenSource CancellationTokenSource { get; set; }

    private void Awake()
    {
        TriggerGridBuilder = GetComponent<TriggerGridBuilder>();
    }

    private void OnEnable()
    {
        StartChecking();
    }

    private void OnDisable()
    {
        StopChecking();
    }

    public void StartChecking()
    {
        CancellationTokenSource = new CancellationTokenSource();
        
        CheckTriggers();
    }

    public void StopChecking()
    {
        CancellationTokenSource.Cancel();
    }

    private async void CheckTriggers()
    {
        var delay = TimeSpan.FromSeconds(CheckerDelay);
        
        while (!CancellationTokenSource.IsCancellationRequested)
        {
            if (TriggerGridBuilder.TriggerInfos != null)
            {
                CheckingDiagonally(true);
                CheckingDiagonally(false);

                CheckAlongStraightLines(true);
                CheckAlongStraightLines(false);

                CheckForCompleteness();
            }

            try
            {
                await Task.Delay(delay, CancellationTokenSource.Token);
                
            }
            catch { /* ignored */ }
        }
    }

    private void CheckForCompleteness()
    {
        foreach (var triggerInfo in TriggerGridBuilder.TriggerInfos)
        {
            if (triggerInfo.GetColor() == CircleColor.None) return; 
        }
        
        GameSingleton.Instance.GameplayController.EndGame();
    }

    private void CheckAlongStraightLines(bool isHorizontal)
    {
        for (var r = 0; r < TriggerGridBuilder.Row; r++)
        {
            var colorCollection = new ColorCollection();

            for (var c = 0; c < TriggerGridBuilder.Column; c++)
            {
                var color = isHorizontal ? TriggerGridBuilder.TriggerInfos[c, r].GetColor() :TriggerGridBuilder.TriggerInfos[r, c].GetColor();
                if (color != CircleColor.None) colorCollection.ColorDictionary[color] += 1;
            }

            var isCombination = isHorizontal ? colorCollection.ColorDictionary.Any(key => key.Value >= TriggerGridBuilder.Row) : colorCollection.ColorDictionary.Any(key => key.Value >= TriggerGridBuilder.Column);
            if (!isCombination) continue;

            GameSingleton.Instance.GameplayController.AddScore(isHorizontal ? TriggerGridBuilder.TriggerInfos[0, r].GetColor() : TriggerGridBuilder.TriggerInfos[r, 0].GetColor());

            GameSingleton.Instance.EffectsController.PlayExplosionEffect();

            // Wake sleeping bodies before removing the matched line so physics contacts refresh.
            foreach (var triggerInfo in TriggerGridBuilder.TriggerInfos)
            {
                triggerInfo.WakeUpRigidbody2D();
            }
            
            for (int c = 0; c < TriggerGridBuilder.Column; c++)
            {
                if (isHorizontal) TriggerGridBuilder.TriggerInfos[c, r]?.DestroyCircle();
                else TriggerGridBuilder.TriggerInfos[r, c]?.DestroyCircle();
            }
        }
    }

    private void CheckingDiagonally(bool isFromLeftTop)
    {
        var columnIndex = isFromLeftTop ? TriggerGridBuilder.Column-1 : 0;

        var colorCollection = new ColorCollection();
        
        for (int r = 0; r < TriggerGridBuilder.Row; r++)
        {
            var color = TriggerGridBuilder.TriggerInfos[r, columnIndex].GetColor();
            if (color != CircleColor.None) colorCollection.ColorDictionary[color] += 1;
            
            columnIndex = isFromLeftTop ? columnIndex - 1 : columnIndex + 1;
        }

        var lineCount = TriggerGridBuilder.Row == TriggerGridBuilder.Column ? TriggerGridBuilder.Row : 0;
        var isCombination = lineCount > 0 && colorCollection.ColorDictionary.Any(key => key.Value >= lineCount);
        if (!isCombination) return;
        
        GameSingleton.Instance.GameplayController.AddScore(isFromLeftTop ? TriggerGridBuilder.TriggerInfos[0, TriggerGridBuilder.Column-1].GetColor() : TriggerGridBuilder.TriggerInfos[0, 0].GetColor());

        GameSingleton.Instance.EffectsController.PlayExplosionEffect();
            
        // Wake sleeping bodies before removing the matched line so physics contacts refresh.
        foreach (var triggerInfo in TriggerGridBuilder.TriggerInfos)
        {
            triggerInfo.WakeUpRigidbody2D();
        }
        
        columnIndex = isFromLeftTop ? TriggerGridBuilder.Column-1 : 0;
        
        for (int r = 0; r < TriggerGridBuilder.Row; r++)
        {
            TriggerGridBuilder.TriggerInfos[r, columnIndex].DestroyCircle();
            
            columnIndex = isFromLeftTop ? columnIndex - 1 : columnIndex + 1;
        }
    }
}
