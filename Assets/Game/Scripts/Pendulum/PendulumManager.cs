using UnityEngine;

public class PendulumManager : MonoBehaviour
{
    private CircleObject CurrentCircleObject { get; set; }
    
    private SpawnCircleController SpawnCircleController { get; set; }
    
    private Rigidbody2D Rigidbody2D { get; set; }
    private GameplayController GameplayController { get; set; }
    private TriggerGridChecker TriggerGridChecker { get; set; }

    public void Construct(GameplayController gameplayController, TriggerGridChecker triggerGridChecker)
    {
        GameplayController = gameplayController;
        TriggerGridChecker = triggerGridChecker;
    }

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        SpawnCircleController = GetComponent<SpawnCircleController>();
    }

    private void OnEnable()
    {
        SpawnCircleObject();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(SpawnCircleObject));
    }

    private void Update()
    {
        if (!GameplayController || GameplayController.State != GameState.Playing)
            return;

        if (!Input.anyKeyDown || !CurrentCircleObject)
            return;
        
        TriggerGridChecker?.StopChecking();
        
        DropTheCircle();
        Invoke(nameof(SpawnCircleObject), 1);
        
        TriggerGridChecker?.StartChecking();
    }

    public void SpawnCircleObject()
    {
        if (!GameplayController || GameplayController.State != GameState.Playing)
            return;

        if (CurrentCircleObject)
            return;
        
        CurrentCircleObject = SpawnCircleController.SpawnCircle(Rigidbody2D);
    }
    
    private void DropTheCircle()
    {
        if (!CurrentCircleObject)
            return;
        
        CurrentCircleObject.DeactivateHingeJoint();
        CurrentCircleObject = null;
    }
}
