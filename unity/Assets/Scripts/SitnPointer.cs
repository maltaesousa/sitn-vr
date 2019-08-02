using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;

public class SitnPointer : MonoBehaviour
{
    public float defaultLength = 0.5f;
    public GameObject endDot;
    public Hand hand;
    public VRInputManager inputModule;

    private LineRenderer lineRenderer = null;
    private bool autoLength = true;
    private float lastLength = 0.5f;

    private float offset = 0.0f;

    public void SetOffset(float value)
    {
        offset = value;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lastLength = defaultLength;
    }

    private void Update()
    {
        UpdateLineState();
    }

    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetAutoLength(bool value)
    {
        autoLength = value;
    }

    public GameObject GetAttachedObject()
    {
        return hand.currentAttachedObject;
    }
    
    private void UpdateLineState()
    {
        PointerEventData data = inputModule.GetData();
        float targetLength;
        float inputRayLength = data.pointerCurrentRaycast.distance;
        if (inputRayLength == 0)
        {
            targetLength = defaultLength;
        } else
        {
            targetLength = inputRayLength;
        }

        // Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPosition;

        // Don't update en position until the attached object is released
        if (GetAttachedObject() == null)
        {
            endPosition = transform.position + (transform.forward * targetLength);
        }
        else
        {
            endPosition = transform.position + (transform.forward * lastLength);
        }

        if (autoLength && hit.collider != null)
        {   
            endPosition = hit.point;
            lastLength = hit.distance;
        }
        else if (!autoLength)
        {
            // if there's an offset update lenght otherwise continue using a fixed length
            targetLength = offset == 0.0f ? lastLength : lastLength + offset;
            lastLength = targetLength;
            endPosition = transform.position + (transform.forward * targetLength);
            print("FIXED" + lastLength);
        }

        // Update position of the dot
        endDot.transform.position = endPosition;

        // Update the line renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
        offset = 0.0f;
    }

    private RaycastHit CreateRaycast(float lenght)
    {
        // collide against everything except layer 8.
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;
        // This creates a ray with two Vector3.
        Ray ray = new Ray(transform.position, transform.forward);
        // This creates the Raycast. the variable hit will contain information about what is hit
        Physics.Raycast(ray, out hit, lenght, layerMask);

        return hit;
    }
}
