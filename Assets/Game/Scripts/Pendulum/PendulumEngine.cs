using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PendulumEngine : MonoBehaviour
{
    [field: Header("Options")]
    
    [field: SerializeField] 
    public float MovementSpeed { get; private set; }
    
    [field: SerializeField] 
    public float Angle { get; private set; }

    #region Privates
    
    /// <summary>
    /// Движение по часовой стрелке?
    /// </summary>
    private bool MovingClockwise { get; set; }
    
    private Rigidbody2D _rigidbody2D;
    
    #endregion
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void TryChangeMoveDirection()
    {
        if (transform.rotation.z > Angle) MovingClockwise = true;
        else if (transform.rotation.z < -Angle) MovingClockwise = false;
    }

    private void Move()
    {
        TryChangeMoveDirection();
        
        _rigidbody2D.angularVelocity = MovingClockwise ? -MovementSpeed : MovementSpeed;
    }
}
