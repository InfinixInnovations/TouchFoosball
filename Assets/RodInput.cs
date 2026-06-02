using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static RodController ActiveRod;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(
                ray.origin,
                ray.direction * 100f,
                Color.red,
                2f
            );

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Raycast Hit: " + hit.collider.name);

                RodController rod =
                    hit.collider.GetComponentInParent<RodController>();

                if (rod != null)
                {
                    ActiveRod = rod;
                    Debug.Log("Selected Rod: " + rod.name);
                }
                else
                {
                    Debug.Log("Hit object has no RodController");
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ActiveRod = null;
            Debug.Log("Rod Deselected");
        }
    }
}