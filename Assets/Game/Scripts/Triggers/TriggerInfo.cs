using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TriggerInfo : MonoBehaviour
{
    private CircleObject CircleObject { get; set; }
    
    private void Awake()
    {
        var trigger = gameObject.GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.2f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out CircleObject circleObject)) return;
        
        CircleObject = circleObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.GetComponent<CircleObject>()) return;

        CircleObject = null;
    }

    public CircleColor GetColor()
    {
        return CircleObject ? CircleObject.CircleColor : CircleColor.None;
    }

    public void WakeUpRigidbody2D()
    {
        if (!CircleObject) return;
        
        CircleObject.WakeUpRigidbody2D();
    }

    public void DestroyCircle()
    {
        if (!CircleObject) return;
        
        CircleObject.DestroyObject();
        CircleObject = null;
    }
}
