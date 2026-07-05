using System.Collections.Generic;
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

    private readonly List<CircleObject> _circleObjects = new();

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

        if (!_circleObjects.Contains(circleObject))
            _circleObjects.Add(circleObject);

        TimeBelowSpeedThreshold = 0f;
        TriggerGridChecker?.ScheduleCheck();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out CircleObject circleObject) || !_circleObjects.Contains(circleObject))
            return;

        CircleObject selectedCircle = SelectCircle();
        if (!selectedCircle || circleObject != selectedCircle)
            return;

        if (selectedCircle.IsAttachedToPendulum)
        {
            TimeBelowSpeedThreshold = 0f;
            return;
        }

        if (selectedCircle.Speed > _magnetSpeedThreshold)
        {
            TimeBelowSpeedThreshold = 0f;
            return;
        }

        TimeBelowSpeedThreshold += Time.deltaTime;

        if (TimeBelowSpeedThreshold < _magnetDelay)
            return;

        selectedCircle.MoveToCellCenter(transform.position, _magnetStrength);
        TriggerGridChecker?.ScheduleCheck();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out CircleObject circleObject))
            return;

        _circleObjects.Remove(circleObject);
        TimeBelowSpeedThreshold = 0f;
        TriggerGridChecker?.ScheduleCheck();
    }

    public CircleColor GetColor()
    {
        CircleObject selectedCircle = SelectCircle();
        return selectedCircle ? selectedCircle.CircleColor : CircleColor.None;
    }

    public CircleObject GetCircle()
    {
        return SelectCircle();
    }

    public CircleObject ReleaseCircle()
    {
        CircleObject releasedCircle = SelectCircle();
        if (releasedCircle)
            _circleObjects.Remove(releasedCircle);

        TimeBelowSpeedThreshold = 0f;
        return releasedCircle;
    }

    public void PlaceCircle(CircleObject circleObject)
    {
        if (circleObject && !_circleObjects.Contains(circleObject))
            _circleObjects.Add(circleObject);

        TimeBelowSpeedThreshold = 0f;

        if (circleObject)
            circleObject.SettleAtCellCenter(transform.position);
    }

    public void WakeUpRigidbody2D()
    {
        CircleObject selectedCircle = SelectCircle();
        if (!selectedCircle)
            return;
        
        selectedCircle.WakeUpRigidbody2D();
    }

    public void DestroyCircle()
    {
        CircleObject selectedCircle = SelectCircle();
        if (!selectedCircle)
            return;

        _circleObjects.Remove(selectedCircle);
        selectedCircle.DestroyObject();
    }

    private CircleObject SelectCircle()
    {
        RemoveMissingCircles();

        CircleObject bestCircle = null;
        float bestDistance = float.MaxValue;

        foreach (CircleObject circleObject in _circleObjects)
        {
            if (!circleObject || circleObject.IsAttachedToPendulum)
                continue;

            Vector2 circlePosition = circleObject.transform.position;
            Vector2 cellPosition = transform.position;
            float distance = Vector2.SqrMagnitude(circlePosition - cellPosition);
            if (distance >= bestDistance)
                continue;

            bestDistance = distance;
            bestCircle = circleObject;
        }

        return bestCircle;
    }

    private void RemoveMissingCircles()
    {
        for (int i = _circleObjects.Count - 1; i >= 0; i--)
        {
            if (!_circleObjects[i])
                _circleObjects.RemoveAt(i);
        }
    }
}
