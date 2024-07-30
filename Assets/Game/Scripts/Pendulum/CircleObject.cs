using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer), typeof(HingeJoint2D))]
public class CircleObject : MonoBehaviour
{
    public CircleColor CircleColor { get; private set; }
    
    #region Privates
    private Rigidbody2D Rigidbody2D { get; set; }
    private SpriteRenderer SpriteRenderer { get; set; }
    private HingeJoint2D HingeJoint2D { get; set; }
    
    #endregion

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        HingeJoint2D = GetComponent<HingeJoint2D>();
        
        ChangeColor();
    }

    public void SetConnectedRigidbody2D(Rigidbody2D connectedRigidbody2D)
    {
        if (!HingeJoint2D) return;
        
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

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    
    private void ChangeColor()
    {
        var random = Random.Range(1, Enum.GetValues(typeof(CircleColor)).Length);
        CircleColor = (CircleColor)random;
        var stringColor = CircleColor.ToString();
        var isColor = ColorUtility.TryParseHtmlString(stringColor, out var color);
        
        if (isColor) SpriteRenderer.color = color;
    }
}

public enum CircleColor
{
    None,
    Red,
    Green,
    Blue
}
