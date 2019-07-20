using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPointer : MonoBehaviour
{
    public float defaultLength = 0.5f;
    public GameObject endDot;
    public VRInputModule inputModule;

    private LineRenderer lineRenderer = null;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // Use default or provided distance
        float targetLength = defaultLength;

        // Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        // Default dot

        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // Get end position if collider is hit
        if (hit.collider != null)
            endPosition = hit.point;

        // Update position of the dot
        endDot.transform.position = endPosition;

        // Update the line rendere
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRaycast(float lenght)
    {
        RaycastHit hit;
        // This creates a ray with two Vector3.
        Ray ray = new Ray(transform.position, transform.forward);
        // This creates the Raycast. the variable hit will contain information about what is hit
        Physics.Raycast(ray, out hit, defaultLength);

        return hit;
    }
}
