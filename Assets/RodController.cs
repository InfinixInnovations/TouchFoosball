using UnityEngine;

public class RodController : MonoBehaviour
{
    [Header("References")]
    public Transform rodVisual;

    [Header("Movement")]
    public float minX = -4f;
    public float maxX = 4f;
    public float moveSensitivity = 0.02f;

    [Header("Rotation")]
    public float rotationSensitivity = 3f;

    private bool isDragging;
    private Vector3 lastMousePos;
    private float currentRotation;

    private Vector3 initialEuler;

    public float CurrentSpinSpeed { get; private set; }

    private float previousRotation;

    void Start()
    {
        if (rodVisual != null)
        {
            initialEuler = rodVisual.localEulerAngles;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        lastMousePos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        if (!isDragging)
            return;

        Vector3 currentMousePos = Input.mousePosition;
        Vector3 delta = currentMousePos - lastMousePos;

        // Horizontal movement
        Vector3 pos = transform.position;
        pos.x += delta.x * moveSensitivity;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Rotation
        currentRotation += delta.y * rotationSensitivity;

        if (rodVisual != null)
        {
            Vector3 euler = initialEuler;

            // CHANGE THIS AXIS IF NEEDED
            euler.x = initialEuler.x + currentRotation;

            rodVisual.localEulerAngles = euler;
        }

        // Track spin speed
        CurrentSpinSpeed =
            Mathf.Abs(currentRotation - previousRotation) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        previousRotation = currentRotation;

        lastMousePos = currentMousePos;
    }
}