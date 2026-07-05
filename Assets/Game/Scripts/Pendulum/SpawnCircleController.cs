using UnityEngine;
using System.Collections.Generic;

public class SpawnCircleController : MonoBehaviour
{
    [Header("Spawn Options")]
    
    [SerializeField] private Transform _spawnTransform;
    
    [SerializeField] private CircleObject _circlePrefab;

    private List<CircleObject> SpawnedCircles { get; } = new();

    public CircleObject SpawnCircle(Rigidbody2D connectedRigidbody)
    {
        CircleObject circleObject = Instantiate(_circlePrefab, _spawnTransform);
        circleObject.SetConnectedRigidbody2D(connectedRigidbody);
        SpawnedCircles.Add(circleObject);
        return circleObject;
    }

    public void ClearSpawnedCircles()
    {
        for (int i = SpawnedCircles.Count - 1; i >= 0; i--)
        {
            CircleObject circleObject = SpawnedCircles[i];

            if (circleObject)
                Destroy(circleObject.gameObject);
        }

        SpawnedCircles.Clear();
    }
}
