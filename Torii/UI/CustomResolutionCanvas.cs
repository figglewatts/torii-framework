using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class CustomResolutionCanvas : MonoBehaviour
{
    public Vector2 Resolution;
    public Camera TargetCamera;

    private Canvas _canvas;
    private CanvasScaler _scaler;
    private RectTransform _rect;

    public void OnValidate()
    {
        if (TargetCamera == null)
        {
            Debug.LogWarning("CustomResolutionCanvas needs to have a target camera");
            return;
        }

        if (Resolution.x <= 0 || Resolution.y <= 0)
        {
            Debug.LogWarning("CustomResolutionCanvas needs to have a resolution of at least 1x1");
            return;
        }

        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.worldCamera = TargetCamera;

        _scaler = GetComponent<CanvasScaler>();
        _scaler.dynamicPixelsPerUnit = 1;
        _scaler.referencePixelsPerUnit = 1;

        _rect = GetComponent<RectTransform>();
        _rect.sizeDelta = Resolution;
        transform.position = new Vector3(0, 0, getFrustumDistance());
    }

    private float getFrustumDistance()
    {
        return Resolution.y * 0.5f / Mathf.Tan(TargetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }
}
