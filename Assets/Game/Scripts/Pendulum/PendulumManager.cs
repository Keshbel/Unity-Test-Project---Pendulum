using UnityEngine;

public class PendulumManager : MonoBehaviour
{
    private CircleObject CurrentCircleObject { get; set; }
    
    private SpawnCircleController SpawnCircleController { get; set; }
    
    private Rigidbody2D Rigidbody2D { get; set; }

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        SpawnCircleController = GetComponent<SpawnCircleController>();
    }

    private void OnEnable()
    {
        SpawnCircleObject();
    }

    private void Update()
    {
        if (!Input.anyKeyDown || !CurrentCircleObject) return;
        
        GameSingleton.Instance.TriggerGridChecker.StopChecking();
        
        DropTheCircle();
        Invoke(nameof(SpawnCircleObject), 1);
        
        GameSingleton.Instance.TriggerGridChecker.StartChecking();
    }

    public void SpawnCircleObject()
    {
        if (!GameSingleton.Instance.GameplayController.IsGame) return;
        
        CurrentCircleObject = SpawnCircleController.SpawnCircle(Rigidbody2D);
    }
    
    private void DropTheCircle()
    {
        if (!CurrentCircleObject) return;
        
        CurrentCircleObject.DeactivateHingeJoint();
        CurrentCircleObject = null;
    }
}
