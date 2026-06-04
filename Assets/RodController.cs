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
    private float previousRotation;

    private Vector3 initialEuler;

    public float RotationSpeed { get; private set; }

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
        RotationSpeed = 0f;
    }

    void Update()
    {
        if (!isDragging)
            return;

        Vector3 currentMousePos = Input.mousePosition;
        Vector3 delta = currentMousePos - lastMousePos;

        // Horizontal rod movement
        Vector3 pos = transform.position;
        pos.x += delta.x * moveSensitivity;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Rod rotation
        currentRotation += delta.y * rotationSensitivity;

        if (rodVisual != null)
        {
            Vector3 euler = initialEuler;

            // Change axis if required
            euler.x = initialEuler.x + currentRotation;

            rodVisual.localEulerAngles = euler;
        }

        // Rotation speed calculation
        RotationSpeed =
            Mathf.Abs(
                Mathf.DeltaAngle(
                    previousRotation,
                    currentRotation
                )
            ) / Mathf.Max(Time.deltaTime, 0.0001f);

        previousRotation = currentRotation;

        lastMousePos = currentMousePos;
    }
}