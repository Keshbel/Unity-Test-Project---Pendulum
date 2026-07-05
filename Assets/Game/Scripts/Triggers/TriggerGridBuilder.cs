using UnityEngine;

public class TriggerGridBuilder : MonoBehaviour
{
    [field: Header("Grid")]
    
    [field: SerializeField, Tooltip("Number of grid rows.")]
    public int Row { get; private set; } = 3;
    
    [field: SerializeField, Tooltip("Number of grid columns.")]
    public int Column { get; private set; } = 3;
    
    [field: Header("Is Limiting Triggers")]

    [field: SerializeField, Tooltip("Creates height limit triggers above the grid.")]
    public bool IsLimitingTrigger { get; private set; } = true;

    [field: SerializeField, Tooltip("Allowed time above the grid before ending the game.")]
    public float LimitingSeconds { get; private set; } = 3f;
    
    [field: Header("Options")]
    
    [field: SerializeField, Tooltip("Horizontal spacing between grid columns.")]
    public float RowOffset { get; private set; } = 1.88f;
    
    [field: SerializeField, Tooltip("Vertical spacing between grid rows.")]
    public float ColumnOffset { get; private set; } = 1.6f;

    public TriggerInfo[,] TriggerInfos { get; private set; }

    private void Awake()
    {
        BuildGrid();
    }

    private void BuildGrid()
    {
        TriggerInfos = new TriggerInfo[Row, Column];
        
        var rowPosition = 0f;
        
        for (int c = 0; c < Column; c++)
        {
            var columnPosition = 0f;

            var rowGo = CreateGameObject("Column " + c, transform, new Vector3(rowPosition, columnPosition));
            
            for (int r = 0; r < Row; r++)
            {
                var triggerGo = CreateGameObject("Trigger " + r, rowGo.transform, new Vector3(0, columnPosition));
                
                var triggerInfo = triggerGo.AddComponent<TriggerInfo>();
                TriggerInfos[c, r] = triggerInfo;
                
                columnPosition -= ColumnOffset;
            }

            rowPosition += RowOffset;
        }
        
        if (IsLimitingTrigger) BuildLimitingTrigger();
    }

    private GameObject CreateGameObject(string goName, Transform parent, Vector3 localPosition)
    {
        var triggerGo = new GameObject(goName);
        triggerGo.transform.SetParent(parent, false);
        triggerGo.transform.localPosition = localPosition;

        return triggerGo;
    }

    private void BuildLimitingTrigger()
    {
        var limitingGo = CreateGameObject("Limiting Triggers", transform, new Vector3(0, 0 + ColumnOffset));

        var rowPosition = 0f;
        for (int r = 0; r < Row; r++)
        {
            var triggerGo = CreateGameObject("Trigger " + r, limitingGo.transform, new Vector3(rowPosition, 0));
            triggerGo.AddComponent<CheckingLimitingTrigger>().DefaultTimeToDefeat = LimitingSeconds;

            rowPosition += RowOffset;
        }
    }
}
