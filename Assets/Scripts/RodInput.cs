using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Tooltip("Optional. Falls back to Camera.main if left empty.")]
    public Camera inputCamera;

    [Tooltip("Limit raycasts to the rod collider layer(s) if you want (default = everything).")]
    public LayerMask raycastMask = ~0;

    [Tooltip("If true, a rod already controlled by one finger can't be grabbed by another until released.")]
    public bool lockRodToFinger = true;

    // Maps an active finger (by fingerId) to the rod it's dragging.
    private readonly Dictionary<int, RodController> fingerToRod = new Dictionary<int, RodController>();

    // Reverse lookup so we can tell if a rod is already claimed.
    private readonly HashSet<RodController> claimedRods = new HashSet<RodController>();

    // Sentinel fingerId used for the mouse so it shares the same code path.
    private const int MouseFingerId = -1;

    void Awake()
    {
        if (inputCamera == null)
            inputCamera = Camera.main;
    }

    void Update()
    {
        if (inputCamera == null)
        {
            inputCamera = Camera.main;
            if (inputCamera == null)
                return;
        }

        if (Input.touchCount > 0)
            HandleTouches();
        else
            HandleMouse();
    }

    private void HandleTouches()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    TryBeginDrag(t.fingerId, t.position);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    UpdateDrag(t.fingerId, t.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    EndDrag(t.fingerId);
                    break;
            }
        }
    }

    private void HandleMouse()
    {
        Vector2 pos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
            TryBeginDrag(MouseFingerId, pos);
        else if (Input.GetMouseButton(0))
            UpdateDrag(MouseFingerId, pos);
        else if (Input.GetMouseButtonUp(0))
            EndDrag(MouseFingerId);
    }

    private void TryBeginDrag(int fingerId, Vector2 screenPos)
    {
        // This finger is already dragging something (shouldn't normally happen on Began).
        if (fingerToRod.ContainsKey(fingerId))
            return;

        RodController rod = RaycastForRod(screenPos);
        if (rod == null)
            return;

        // If the rod is already held by another finger, optionally refuse.
        if (lockRodToFinger && claimedRods.Contains(rod))
            return;

        fingerToRod[fingerId] = rod;
        claimedRods.Add(rod);
        rod.BeginDrag(screenPos);
    }

    private void UpdateDrag(int fingerId, Vector2 screenPos)
    {
        if (fingerToRod.TryGetValue(fingerId, out RodController rod) && rod != null)
            rod.UpdateDrag(screenPos);
    }

    private void EndDrag(int fingerId)
    {
        if (fingerToRod.TryGetValue(fingerId, out RodController rod))
        {
            if (rod != null)
                rod.EndDrag();

            claimedRods.Remove(rod);
            fingerToRod.Remove(fingerId);
        }
    }

    private RodController RaycastForRod(Vector2 screenPos)
    {
        Ray ray = inputCamera.ScreenPointToRay(screenPos);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastMask))
        {
            return hit.collider.GetComponentInParent<RodController>();
        }
        return null;
    }
}