using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer), typeof(HingeJoint2D))]
public class CircleObject : MonoBehaviour
{
    public CircleColor CircleColor { get; private set; }
    public bool IsAttachedToPendulum => HingeJoint2D && HingeJoint2D.enabled;
    public float Speed => Rigidbody2D ? Rigidbody2D.linearVelocity.magnitude : 0f;
    
    private Rigidbody2D Rigidbody2D { get; set; }
    private SpriteRenderer SpriteRenderer { get; set; }
    private HingeJoint2D HingeJoint2D { get; set; }

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        HingeJoint2D = GetComponent<HingeJoint2D>();
        
        ChangeColor();
    }

    public void SetConnectedRigidbody2D(Rigidbody2D connectedRigidbody2D)
    {
        if (!HingeJoint2D)
            return;
        
        HingeJoint2D.connectedBody = connectedRigidbody2D;
    }
    
    public void DeactivateHingeJoint()
    {
        transform.SetParent(null, true);
        HingeJoint2D.enabled = false;
    }

    public void WakeUpRigidbody2D()
    {
        Rigidbody2D.WakeUp();
    }

    public void MoveToCellCenter(Vector3 worldPosition, float strength)
    {
        if (!Rigidbody2D || IsAttachedToPendulum)
            return;

        Vector2 nextPosition = Vector2.Lerp(Rigidbody2D.position, worldPosition, strength);
        Rigidbody2D.MovePosition(nextPosition);
        Rigidbody2D.linearVelocity *= 0.5f;
        Rigidbody2D.angularVelocity *= 0.5f;
    }

    public void SettleAtCellCenter(Vector3 worldPosition)
    {
        if (!Rigidbody2D || IsAttachedToPendulum)
            return;

        Vector2 targetPosition = worldPosition;
        Rigidbody2D.position = targetPosition;
        Rigidbody2D.linearVelocity = Vector2.zero;
        Rigidbody2D.angularVelocity = 0f;
        transform.position = worldPosition;
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    
    private void ChangeColor()
    {
        int random = Random.Range(1, Enum.GetValues(typeof(CircleColor)).Length);
        CircleColor = (CircleColor)random;
        string stringColor = CircleColor.ToString();
        bool isColor = ColorUtility.TryParseHtmlString(stringColor, out Color color);
        
        if (isColor)
            SpriteRenderer.color = color;
    }
}

public enum CircleColor
{
    None,
    Red,
    Green,
    Blue
}
