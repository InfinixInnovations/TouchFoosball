using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [Header("Hide On Touch")]
    [Tooltip("GameObject to hide (disable) when touched. Often this object itself, e.g. a start screen.")]
    public GameObject objectToHide;

    [Header("Enable On Touch")]
    [Tooltip("Components to enable when touched (scripts, Colliders, Renderers, AudioSources, etc.).")]
    public Behaviour[] componentsToEnable;

    [Tooltip("GameObjects to activate when touched (alternative to enabling individual components).")]
    public GameObject[] objectsToActivate;

    [Header("Options")]
    [Tooltip("If true, only triggers the first time it's touched.")]
    public bool triggerOnce = true;

    [Tooltip("Raycast must hit this object's collider. Requires a Collider on this GameObject.")]
    public bool requireRaycastHit = true;

    private bool hasTriggered;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (triggerOnce && hasTriggered)
            return;

        if (TouchBegan(out Vector2 screenPos))
        {
            if (requireRaycastHit)
            {
                if (HitsThisObject(screenPos))
                    Activate();
            }
            else
            {
                Activate();
            }
        }
    }

    private bool TouchBegan(out Vector2 screenPos)
    {
        screenPos = default;

        // Touch input
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                screenPos = t.position;
                return true;
            }
            return false;
        }

        // Mouse fallback (editor / desktop testing)
        if (Input.GetMouseButtonDown(0))
        {
            screenPos = Input.mousePosition;
            return true;
        }

        return false;
    }

    private bool HitsThisObject(Vector2 screenPos)
    {
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return false;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hit this object or one of its children.
            return hit.collider.transform == transform
                || hit.collider.transform.IsChildOf(transform);
        }
        return false;
    }

    private void Activate()
    {
        hasTriggered = true;

        foreach (Behaviour b in componentsToEnable)
        {
            if (b != null)
                b.enabled = true;
        }

        foreach (GameObject go in objectsToActivate)
        {
            if (go != null)
                go.SetActive(true);
        }

        // Hide last so disabling this object (if it's the target) doesn't
        // stop the loops above from running.
        if (objectToHide != null)
            objectToHide.SetActive(false);
    }
}