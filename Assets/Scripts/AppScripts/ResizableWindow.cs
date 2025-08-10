using UnityEngine;
using UnityEngine.UI;

public class ResizableWindow : MonoBehaviour
{
    public RectTransform windowRectTransform; // Reference to the panel's RectTransform
    public float resizeBorderThickness = 10f; // How close you need to be to the edge to start resizing
    private Vector2 originalMousePosition;
    private Vector2 originalPanelSize;
    private bool isResizing = false;

    void Update()
    {
        // Check if the mouse is over the border and initiate resizing
        if (IsMouseOverResizeArea())
        {
            Cursor.SetCursor(Texture2D.whiteTexture, Vector2.zero, CursorMode.ForceSoftware);
            if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
            {
                isResizing = true;
                originalMousePosition = Input.mousePosition;
                originalPanelSize = windowRectTransform.sizeDelta;
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // Reset cursor to default
        }

        // If we're resizing, update the panel size based on mouse movement
        if (isResizing && Input.GetMouseButton(0))
        {
            Vector2 deltaMousePosition = (Vector2)Input.mousePosition - originalMousePosition;
            windowRectTransform.sizeDelta = originalPanelSize + new Vector2(deltaMousePosition.x, -deltaMousePosition.y);
        }

        // Stop resizing when mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isResizing = false;
        }
    }

    // Check if the mouse is near the edges of the panel for resizing
    bool IsMouseOverResizeArea()
    {
        Vector2 mousePosition = Input.mousePosition;
        Rect panelRect = windowRectTransform.rect;

        return mousePosition.x >= panelRect.xMin && mousePosition.x <= panelRect.xMax &&
               mousePosition.y >= panelRect.yMin && mousePosition.y <= panelRect.yMax;
    }
}
