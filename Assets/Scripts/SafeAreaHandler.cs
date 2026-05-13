using UnityEngine;

public class SafeAreaHandler : MonoBehaviour
{
    RectTransform rectTransform;
    Rect lastSafeArea = Rect.zero;
    Vector2 lastScreenSize = Vector2.zero;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Refresh();
    }

    void Update()
    {
        if (lastSafeArea != Screen.safeArea || lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
        {
            Refresh();
        }
    }

    void Refresh()
    {
        lastScreenSize.x = Screen.width;
        lastScreenSize.y = Screen.height;
        lastSafeArea = Screen.safeArea;

        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}