using UnityEngine;

public class CanvasUtils
{
    public static Vector2 WorldPositionToCanvasPosition(Vector3 position, Canvas canvas, Camera camera)
    {
        Vector2 result = Vector2.zero;
        Vector2 screenViewPort = camera.WorldToViewportPoint(position);
        Rect canvasRect = canvas.GetComponent<RectTransform>().rect;

        Vector2 canvasHalfSize = canvasRect.size / 2f;
        result.x = MathUtils.Remap(0f, 1f, -canvasHalfSize.x, canvasHalfSize.x, screenViewPort.x);
        result.y = MathUtils.Remap(0f, 1f, -canvasHalfSize.y, canvasHalfSize.y, screenViewPort.y);

        return result;
    }
}
