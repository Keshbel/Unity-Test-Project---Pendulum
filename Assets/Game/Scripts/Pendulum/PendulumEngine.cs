using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PendulumEngine : MonoBehaviour
{
    [Header("Options")]
    
    [SerializeField] private float _movementSpeed;
    
    [SerializeField] private float _angle;

    public float MovementSpeed => _movementSpeed;
    public float Angle => _angle;
    
    private bool MovingClockwise { get; set; }
    
    private Rigidbody2D _rigidbody2D;
    
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
        if (transform.rotation.z > Angle)
            MovingClockwise = true;
        else if (transform.rotation.z < -Angle)
            MovingClockwise = false;
    }

    private void Move()
    {
        TryChangeMoveDirection();
        
        _rigidbody2D.angularVelocity = MovingClockwise ? -MovementSpeed : MovementSpeed;
    }
}
