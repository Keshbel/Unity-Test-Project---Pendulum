using UnityEngine;
using System.Collections.Generic;

public class SpawnCircleController : MonoBehaviour
{
    [field: Header("Spawn Options")]
    
    [field: SerializeField] 
    private Transform SpawnTransform { get; set; }
    
    [field: SerializeField] 
    private CircleObject CirclePrefab { get; set; }

    private List<CircleObject> SpawnedCircles { get; } = new();

    public CircleObject SpawnCircle(Rigidbody2D connectedRigidbody)
    {
        var circleObject = Instantiate(CirclePrefab, SpawnTransform);
        circleObject.SetConnectedRigidbody2D(connectedRigidbody);
        SpawnedCircles.Add(circleObject);
        return circleObject;
    }

    public void ClearSpawnedCircles()
    {
        for (var i = SpawnedCircles.Count - 1; i >= 0; i--)
        {
            var circleObject = SpawnedCircles[i];
            if (circleObject) Destroy(circleObject.gameObject);
        }

        SpawnedCircles.Clear();
    }
}
