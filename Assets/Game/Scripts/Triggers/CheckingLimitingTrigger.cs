using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CheckingLimitingTrigger : MonoBehaviour
{
    [field: Header("Time to Defeat")]
    
    public float DefaultTimeToDefeat { get; set; } = 3f;
    private float TimeToDefeat { get; set; }
    
    [field: Header("Circle Colliders")]
    
    private List<Collider2D> CircleColliders { get; set; } = new();

    private void Awake()
    {
        var trigger = gameObject.GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<CircleObject>()) CircleColliders.Add(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CircleColliders.Contains(other)) return;

        TimeToDefeat += Time.deltaTime;
        if (TimeToDefeat >= DefaultTimeToDefeat & GameSingleton.Instance.GameplayController.IsGame) GameSingleton.Instance.GameplayController.EndGame();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CircleColliders.Remove(other);

        if (CircleColliders.Count == 0) TimeToDefeat = 0;
    }
}
