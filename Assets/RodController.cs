using UnityEngine;

public class RodController : MonoBehaviour
{
    [Header("References")]
    public Transform rodVisual;

    [Header("Movement")]
    public float minX = -4f;
    public float maxX = 4f;
    public float moveSensitivity = 0.01f;

    [Header("Rotation")]
    public float rotationSensitivity = 2f;

    private int controllingFingerId = -1;

    private Vector2 lastTouchPosition;

    private float currentRotation;
    private float previousRotation;

    private Vector3 initialEuler;

    public float RotationSpeed { get; private set; }

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (rodVisual != null)
        {
            initialEuler = rodVisual.localEulerAngles;
        }
    }

    void Update()
    {
        HandleTouches();
    }

    void HandleTouches()
    {
        // Find a finger if we don't currently own one
        if (controllingFingerId == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase != TouchPhase.Began)
                    continue;

                Ray ray =
                    mainCamera.ScreenPointToRay(
                        touch.position
                    );

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    RodController rod =
                        hit.collider.GetComponentInParent<RodController>();

                    if (rod == this)
                    {
                        controllingFingerId = touch.fingerId;
                        lastTouchPosition = touch.position;

                        Debug.Log(
                            $"{name} claimed finger {controllingFingerId}"
                        );

                        break;
                    }
                }
            }

            return;
        }

        // Update the finger currently controlling this rod
        bool fingerFound = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.fingerId != controllingFingerId)
                continue;

            fingerFound = true;

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
            {
                Debug.Log(
                    $"{name} released finger {controllingFingerId}"
                );

                controllingFingerId = -1;
                RotationSpeed = 0f;

                return;
            }

            Vector2 delta =
                touch.position -
                lastTouchPosition;

            // Horizontal movement
            Vector3 pos = transform.position;

            pos.x += delta.x * moveSensitivity;

            pos.x = Mathf.Clamp(
                pos.x,
                minX,
                maxX
            );

            transform.position = pos;

            // Vertical rotation
            currentRotation +=
                delta.y * rotationSensitivity;

            if (rodVisual != null)
            {
                Vector3 euler = initialEuler;

                // Change axis if needed
                euler.x =
                    initialEuler.x +
                    currentRotation;

                rodVisual.localEulerAngles =
                    euler;
            }

            RotationSpeed =
                Mathf.Abs(
                    Mathf.DeltaAngle(
                        previousRotation,
                        currentRotation
                    )
                ) /
                Mathf.Max(
                    Time.deltaTime,
                    0.0001f
                );

            previousRotation =
                currentRotation;

            lastTouchPosition =
                touch.position;

            break;
        }

        if (!fingerFound)
        {
            controllingFingerId = -1;
            RotationSpeed = 0f;
        }
    }
}