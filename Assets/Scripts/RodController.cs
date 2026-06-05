using UnityEngine;
using DG.Tweening; // Required for DOTween

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
    public float minRotationAngle = -90f;
    public float maxRotationAngle = 90f;
    [Tooltip("Enable for rods whose players face the opposite direction (e.g. the blue team) so their controls aren't reversed.")]
    public bool invertRotation = false;

    [Header("Gravity (DOTween)")]
    [Tooltip("How many seconds it takes for the heavy players to swing back straight down")]
    public float returnDuration = 1.0f;

    private bool isDragging;
    private Vector2 lastPointerPos;
    private float currentRotation;
    private Vector3 initialEuler;
    private float previousRotation;

    // Tracks the current active animation
    private Tweener returnTween;

    public float SpinVelocity { get; private set; }
    public bool IsDragging => isDragging;

    void Start()
    {
        if (rodVisual != null)
        {
            initialEuler = rodVisual.localEulerAngles;
        }
    }

    // Called by InputManager when this rod is selected (pointer pressed on it).
    public void BeginDrag(Vector2 pointerPos)
    {
        isDragging = true;
        lastPointerPos = pointerPos;

        // If the rod is currently swinging back to center and the player grabs it,
        // instantly kill the animation so they regain total control.
        if (returnTween != null && returnTween.IsActive())
        {
            returnTween.Kill();
        }
    }

    // Called by InputManager when the pointer is released.
    public void EndDrag()
    {
        if (!isDragging)
            return;

        isDragging = false;

        // Swing the rod back to 0 (straight down) with a bottom-heavy wobble.
        returnTween = DOTween.To(() => currentRotation, x => currentRotation = x, 0f, returnDuration)
            .OnUpdate(() =>
            {
                if (rodVisual != null)
                {
                    Vector3 euler = initialEuler;
                    euler.x = initialEuler.x + currentRotation;
                    rodVisual.localEulerAngles = euler;
                }
            })
            .SetEase(Ease.OutBounce);
    }

    // Called every frame by InputManager while this rod is the active drag target.
    public void UpdateDrag(Vector2 pointerPos)
    {
        if (!isDragging)
            return;

        Vector2 delta = pointerPos - lastPointerPos;

        // Position: driven by VERTICAL finger movement (Y) along the rod's travel axis (X)
        Vector3 pos = transform.position;
        pos.x += delta.y * moveSensitivity;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // Rotation: driven by HORIZONTAL finger movement (X).
        // Direction depends on which way the rod's players face.
        float rotationDir = invertRotation ? -1f : 1f;
        currentRotation += delta.x * rotationSensitivity * rotationDir;
        currentRotation = Mathf.Clamp(currentRotation, minRotationAngle, maxRotationAngle);

        if (rodVisual != null)
        {
            Vector3 euler = initialEuler;
            euler.x = initialEuler.x + currentRotation;
            rodVisual.localEulerAngles = euler;
        }

        // Spin velocity
        SpinVelocity =
            (currentRotation - previousRotation) /
            Mathf.Max(Time.deltaTime, 0.0001f);

        previousRotation = currentRotation;
        lastPointerPos = pointerPos;
    }

    private void LateUpdate()
    {
        if (!isDragging)
        {
            SpinVelocity = 0f;
        }
    }
}