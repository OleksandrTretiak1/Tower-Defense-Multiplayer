using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapFitter : MonoBehaviour
{
    public float defaultOrthoSize = 5f;
    private float _targetAspect = 16f / 9f;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();
        float currentAspect = (float)Screen.width / (float)Screen.height;

        if (currentAspect < _targetAspect)
        {
            cam.orthographicSize = defaultOrthoSize * (_targetAspect / currentAspect);
        }
        else
        {
            cam.orthographicSize = defaultOrthoSize;
        }
    }
}