using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCrop : MonoBehaviour 
{
    // Set this to your target aspect ratio, eg. (16, 9) or (4, 3).
    public Vector2 targetAspect = new Vector2(16, 9);
    [SerializeField] private Color _outsideFrameColor = Color.black;
    
    private Camera _camera;
    private Camera _clearCamera;
    private int _lastScreenWidth;
    private int _lastScreenHeight;

    private void Start () 
    {
        _camera = GetComponent<Camera>();
        CreateClearCamera();
        
        UpdateCrop();
    }

    private void LateUpdate()
    {
        if (_lastScreenWidth == Screen.width && _lastScreenHeight == Screen.height)
            return;

        UpdateCrop();
    }

    private void OnDestroy()
    {
        if (!_clearCamera)
            return;

        Destroy(_clearCamera.gameObject);
    }

    private void CreateClearCamera()
    {
        if (_clearCamera)
            return;

        GameObject clearCameraObject = new GameObject($"{nameof(CameraCrop)} Clear Camera");
        clearCameraObject.transform.SetParent(transform, false);

        _clearCamera = clearCameraObject.AddComponent<Camera>();
        _clearCamera.clearFlags = CameraClearFlags.SolidColor;
        _clearCamera.backgroundColor = _outsideFrameColor;
        _clearCamera.cullingMask = 0;
        _clearCamera.depth = _camera.depth - 1f;
        _clearCamera.rect = new Rect(0, 0, 1, 1);
        _clearCamera.allowHDR = false;
        _clearCamera.allowMSAA = false;
        _clearCamera.orthographic = _camera.orthographic;
    }

    private void UpdateCrop() 
    {
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;

        // Determine ratios of screen/window & target, respectively.
        float screenRatio = Screen.width / (float)Screen.height;
        float targetRatio = targetAspect.x / targetAspect.y;

        if (Mathf.Approximately(screenRatio, targetRatio)) 
        {
            // Screen or window is the target aspect ratio: use the whole area.
            _camera.rect = new Rect(0, 0, 1, 1);
        }
        else if (screenRatio > targetRatio) 
        {
            // Screen or window is wider than the target: pillarbox.
            float normalizedWidth = targetRatio / screenRatio;
            float barThickness = (1f - normalizedWidth)/2f;
            _camera.rect = new Rect(barThickness, 0, normalizedWidth, 1);
        }
        else 
        {
            // Screen or window is narrower than the target: letterbox.
            float normalizedHeight = screenRatio / targetRatio;
            float barThickness = (1f - normalizedHeight) / 2f;
            _camera.rect = new Rect(0, barThickness, 1, normalizedHeight);
        }
    }
}
