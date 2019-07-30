using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SitnPointer : MonoBehaviour
{
    public float defaultLength = 0.5f;
    public GameObject endDot;
    public VRInputManager inputModule;

    private LineRenderer lineRenderer = null;
    private bool autoLength = true;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        UpdateLine();
    }

    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetAutoLength(bool value)
    {
        autoLength = value;
    }
    
    private void UpdateLine()
    {
        // Use default or provided distance
        PointerEventData data = inputModule.GetData();
        float targetLength = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;

        // Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        // Default dot

        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // Get end position if collider is hit
        if (hit.collider != null && autoLength)
            endPosition = hit.point;

        // Update position of the dot
        endDot.transform.position = endPosition;

        // Update the line renderer
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
