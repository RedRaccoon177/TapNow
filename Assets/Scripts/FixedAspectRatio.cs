using UnityEngine;

/// <summary>
/// 모든 디바이스에서 지정된 비율을 유지하고, 여백을 추가하여 화면 왜곡 방지
/// </summary>
[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    public float targetAspect = 9f / 16f; // 1080:1920 = 9:16

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}
