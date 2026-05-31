using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea = Rect.zero;
    private Vector2 _lastScreenSize = Vector2.zero;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    private void Update()
    {
        if (_lastSafeArea != Screen.safeArea || _lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        _lastScreenSize.x = Screen.width;
        _lastScreenSize.y = Screen.height;
        _lastSafeArea = Screen.safeArea;

        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}