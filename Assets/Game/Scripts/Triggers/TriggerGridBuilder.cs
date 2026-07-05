using UnityEngine;

public class TriggerGridBuilder : MonoBehaviour
{
    private static Sprite CellSprite { get; set; }

    [Header("Grid")]
    
    [SerializeField, Tooltip("Number of grid rows.")]
    private int _row = 3;
    
    [SerializeField, Tooltip("Number of grid columns.")]
    private int _column = 3;
    
    [Header("Is Limiting Triggers")]

    [SerializeField, Tooltip("Creates height limit triggers above the grid.")]
    private bool _isLimitingTrigger = true;

    [SerializeField, Tooltip("Allowed time above the grid before ending the game.")]
    private float _limitingSeconds = 3f;
    
    [Header("Options")]
    
    [SerializeField, Tooltip("Horizontal spacing between grid columns.")]
    private float _rowOffset = 1.88f;
    
    [SerializeField, Tooltip("Vertical spacing between grid rows.")]
    private float _columnOffset = 1.6f;

    [Header("Cell Visuals")]

    [SerializeField, Tooltip("Draws visual cells behind the trigger grid.")]
    private bool _showCellVisuals = true;

    [SerializeField]
    private Color _cellColor = new Color(1f, 1f, 1f, 0.28f);

    [SerializeField]
    private Vector2 _cellVisualScale = new Vector2(1.55f, 1.3f);

    public int Row => _row;
    public int Column => _column;
    public bool IsLimitingTrigger => _isLimitingTrigger;
    public float LimitingSeconds => _limitingSeconds;
    public float RowOffset => _rowOffset;
    public float ColumnOffset => _columnOffset;

    public TriggerInfo[,] TriggerInfos { get; private set; }
    private GameplayController GameplayController { get; set; }
    private TriggerGridChecker TriggerGridChecker { get; set; }

    public void Construct(GameplayController gameplayController, TriggerGridChecker triggerGridChecker)
    {
        GameplayController = gameplayController;
        TriggerGridChecker = triggerGridChecker;
        ApplyDependenciesToGeneratedTriggers();
    }

    private void ApplyDependenciesToGeneratedTriggers()
    {
        if (TriggerInfos != null)
        {
            foreach (TriggerInfo triggerInfo in TriggerInfos)
            {
                triggerInfo.Construct(TriggerGridChecker);
            }
        }

        foreach (CheckingLimitingTrigger limitingTrigger in GetComponentsInChildren<CheckingLimitingTrigger>(true))
        {
            limitingTrigger.Construct(GameplayController);
        }
    }

    private void Awake()
    {
        BuildGrid();
        ApplyDependenciesToGeneratedTriggers();
    }

    private void BuildGrid()
    {
        TriggerInfos = new TriggerInfo[Row, Column];
        
        float rowPosition = 0f;
        
        for (int c = 0; c < Column; c++)
        {
            float columnPosition = 0f;

            GameObject rowGo = CreateGameObject("Column " + c, transform, new Vector3(rowPosition, columnPosition));
            
            for (int r = 0; r < Row; r++)
            {
                GameObject triggerGo = CreateGameObject("Trigger " + r, rowGo.transform, new Vector3(0, columnPosition));
                
                TriggerInfo triggerInfo = triggerGo.AddComponent<TriggerInfo>();

                if (TriggerGridChecker)
                    triggerInfo.Construct(TriggerGridChecker);

                TriggerInfos[c, r] = triggerInfo;

                if (_showCellVisuals)
                    AddCellVisual(triggerGo);
                
                columnPosition -= ColumnOffset;
            }

            rowPosition += RowOffset;
        }
        
        if (IsLimitingTrigger)
            BuildLimitingTrigger();
    }

    private GameObject CreateGameObject(string goName, Transform parent, Vector3 localPosition)
    {
        GameObject triggerGo = new GameObject(goName);
        triggerGo.transform.SetParent(parent, false);
        triggerGo.transform.localPosition = localPosition;

        return triggerGo;
    }

    private void BuildLimitingTrigger()
    {
        GameObject limitingGo = CreateGameObject("Limiting Triggers", transform, new Vector3(0, 0 + ColumnOffset));

        float rowPosition = 0f;
        for (int r = 0; r < Row; r++)
        {
            GameObject triggerGo = CreateGameObject("Trigger " + r, limitingGo.transform, new Vector3(rowPosition, 0));
            CheckingLimitingTrigger limitingTrigger = triggerGo.AddComponent<CheckingLimitingTrigger>();
            limitingTrigger.SetDefaultTimeToDefeat(LimitingSeconds);

            if (GameplayController)
                limitingTrigger.Construct(GameplayController);

            rowPosition += RowOffset;
        }
    }

    private void AddCellVisual(GameObject triggerGo)
    {
        GameObject visual = new GameObject("Cell Visual");
        visual.transform.SetParent(triggerGo.transform, false);
        visual.transform.localPosition = Vector3.zero;

        SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GetCellSprite();
        spriteRenderer.color = _cellColor;
        spriteRenderer.sortingOrder = -10;
        visual.transform.localScale = new Vector3(_cellVisualScale.x, _cellVisualScale.y, 1f);
    }

    private static Sprite GetCellSprite()
    {
        if (CellSprite)
            return CellSprite;

        const int size = 32;
        const int border = 2;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isBorder = x < border || x >= size - border || y < border || y >= size - border;
                texture.SetPixel(x, y, isBorder ? Color.white : new Color(1f, 1f, 1f, 0.08f));
            }
        }

        texture.Apply();
        CellSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return CellSprite;
    }
}
