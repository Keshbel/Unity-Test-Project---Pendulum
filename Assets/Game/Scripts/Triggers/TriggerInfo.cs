using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class TriggerInfo : MonoBehaviour
{
    [SerializeField, Tooltip("Speed below which a circle can be pulled into the cell center.")]
    private float _magnetSpeedThreshold = 0.35f;

    [SerializeField, Tooltip("Time a circle should remain slow inside the cell before magnetizing.")]
    private float _magnetDelay = 0.2f;

    [SerializeField, Range(0.01f, 1f), Tooltip("How strongly the cell pulls a settled circle to its center.")]
    private float _magnetStrength = 0.35f;

    private CircleObject CircleObject { get; set; }
    private TriggerGridChecker TriggerGridChecker { get; set; }
    private float TimeBelowSpeedThreshold { get; set; }

    public void Construct(TriggerGridChecker triggerGridChecker)
    {
        TriggerGridChecker = triggerGridChecker;
    }
    
    private void Awake()
    {
        CircleCollider2D trigger = gameObject.GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius = 0.2f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out CircleObject circleObject))
            return;

        if (CircleObject && CircleObject.Speed <= circleObject.Speed)
            return;

        CircleObject = circleObject;
        TimeBelowSpeedThreshold = 0f;
        TriggerGridChecker?.ScheduleCheck();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CircleObject || !other.TryGetComponent(out CircleObject circleObject) || circleObject != CircleObject)
            return;

        if (CircleObject.IsAttachedToPendulum)
        {
            TimeBelowSpeedThreshold = 0f;
            return;
        }

        if (CircleObject.Speed > _magnetSpeedThreshold)
        {
            TimeBelowSpeedThreshold = 0f;
            return;
        }

        TimeBelowSpeedThreshold += Time.deltaTime;

        if (TimeBelowSpeedThreshold < _magnetDelay)
            return;

        CircleObject.MoveToCellCenter(transform.position, _magnetStrength);
        TriggerGridChecker?.ScheduleCheck();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out CircleObject circleObject) || circleObject != CircleObject)
            return;

        CircleObject = null;
        TimeBelowSpeedThreshold = 0f;
        TriggerGridChecker?.ScheduleCheck();
    }

    public CircleColor GetColor()
    {
        return CircleObject ? CircleObject.CircleColor : CircleColor.None;
    }

    public CircleObject GetCircle()
    {
        return CircleObject;
    }

    public CircleObject ReleaseCircle()
    {
        CircleObject releasedCircle = CircleObject;
        CircleObject = null;
        TimeBelowSpeedThreshold = 0f;
        return releasedCircle;
    }

    public void PlaceCircle(CircleObject circleObject)
    {
        CircleObject = circleObject;
        TimeBelowSpeedThreshold = 0f;

        if (CircleObject)
            CircleObject.SettleAtCellCenter(transform.position);
    }

    public void WakeUpRigidbody2D()
    {
        if (!CircleObject)
            return;
        
        CircleObject.WakeUpRigidbody2D();
    }

    public void DestroyCircle()
    {
        if (!CircleObject)
        {
            CircleObject = null;
            return;
        }
        
        CircleObject.DestroyObject();
        CircleObject = null;
    }
}
