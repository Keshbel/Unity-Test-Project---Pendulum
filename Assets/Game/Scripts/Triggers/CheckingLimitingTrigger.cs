using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CheckingLimitingTrigger : MonoBehaviour
{
    public float DefaultTimeToDefeat { get; private set; } = 3f;
    private float TimeToDefeat { get; set; }
    private List<Collider2D> CircleColliders { get; set; } = new();
    private GameplayController GameplayController { get; set; }

    public void Construct(GameplayController gameplayController)
    {
        GameplayController = gameplayController;
    }

    public void SetDefaultTimeToDefeat(float seconds)
    {
        DefaultTimeToDefeat = seconds;
    }

    private void Awake()
    {
        Collider2D trigger = gameObject.GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<CircleObject>())
            CircleColliders.Add(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CircleColliders.Contains(other))
            return;

        TimeToDefeat += Time.deltaTime;
        if (TimeToDefeat >= DefaultTimeToDefeat && GameplayController && GameplayController.State == GameState.Playing)
            GameplayController.EndGame();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CircleColliders.Remove(other);

        if (CircleColliders.Count == 0)
            TimeToDefeat = 0;
    }
}
