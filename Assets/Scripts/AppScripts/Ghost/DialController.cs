using UnityEngine;
using UnityEngine.EventSystems;

public class DialController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Rotation Settings")]
    public float[] rotationStrenghts = new float[4]; // Hoeveel de meter daalt bij draaien
    private bool isDragging = false;
    private float lastAngle;

    private float rotationStrenght;
    private Ghost ghost;

    void Start()
    {
        ghost = GetComponentInParent<Ghost>();
        rotationStrenght = rotationStrenghts[(int)GameManager.Instance.difficulty];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastAngle = GetAngle(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        float currentAngle = GetAngle(eventData.position);
        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle); // Hoeveel draai sinds vorige frame
        lastAngle = currentAngle;

        // Draai de dial visueel
        transform.Rotate(Vector3.forward, deltaAngle);

        // Laat de Ghost-meter reageren
        if (ghost != null)
        {
            ghost.DialRotated(Mathf.Abs(deltaAngle) * rotationStrenght);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private float GetAngle(Vector2 pointerPos)
    {
        Vector2 dialPos = RectTransformUtility.WorldToScreenPoint(null, transform.position);
        Vector2 dir = pointerPos - dialPos;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }
}
