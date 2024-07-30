using UnityEngine;

public class SpawnCircleController : MonoBehaviour
{
    [field: Header("Spawn Options")]
    
    [field: SerializeField] 
    private Transform SpawnTransform { get; set; }
    
    [field: SerializeField] 
    private CircleObject CirclePrefab { get; set; }

    public CircleObject SpawnCircle(Rigidbody2D connectedRigidbody)
    {
        var circleObject = Instantiate(CirclePrefab, SpawnTransform);
        circleObject.SetConnectedRigidbody2D(connectedRigidbody);
        return circleObject;
    }
}
