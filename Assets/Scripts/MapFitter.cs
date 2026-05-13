using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapFitter : MonoBehaviour
{
    public float defaultOrthoSize = 5f;
    private float targetAspect = 16f / 9f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float currentAspect = (float)Screen.width / (float)Screen.height;

        if (currentAspect < targetAspect)
        {
            cam.orthographicSize = defaultOrthoSize * (targetAspect / currentAspect);
        }
        else
        {
            cam.orthographicSize = defaultOrthoSize;
        }
    }
}