using UnityEngine;
using UnityEngine.EventSystems;

public class PendulumManager : MonoBehaviour
{
    private CircleObject CurrentCircleObject { get; set; }
    
    private SpawnCircleController SpawnCircleController { get; set; }
    
    private Rigidbody2D Rigidbody2D { get; set; }
    private GameplayController GameplayController { get; set; }
    private TriggerGridChecker TriggerGridChecker { get; set; }

    public void Construct(GameplayController gameplayController, TriggerGridChecker triggerGridChecker)
    {
        if (GameplayController != null)
            GameplayController.OnStateChanged -= OnGameStateChanged;

        GameplayController = gameplayController;
        TriggerGridChecker = triggerGridChecker;

        if (GameplayController != null)
            GameplayController.OnStateChanged += OnGameStateChanged;
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

    private void OnDestroy()
    {
        if (GameplayController != null)
            GameplayController.OnStateChanged -= OnGameStateChanged;
    }

    private void Update()
    {
        if (!GameplayController || GameplayController.State != GameState.Playing)
            return;

        if (!IsDropInputPressed() || !CurrentCircleObject)
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

    private void OnGameStateChanged(GameState state)
    {
        if (state != GameState.Playing)
            return;

        if (!CurrentCircleObject && !IsInvoking(nameof(SpawnCircleObject)))
            Invoke(nameof(SpawnCircleObject), 0.1f);
    }

    private static bool IsDropInputPressed()
    {
        if (!Input.anyKeyDown)
            return false;

        if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return false;

        return true;
    }
}
