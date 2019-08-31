using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using SITN;

public class SitnPointer : MonoBehaviour
{
    public float defaultLength = 0.5f;
    public GameObject endDot;
    public Hand hand;
    public VRInputManager inputModule;
    public Material deleteMaterial;
    public Material infoMaterial;
    public Text attributeTextArea;

    private LineRenderer lineRenderer = null;
    private bool autoLength = true;
    private float lastLength = 0.5f;

    private float offset = 0.0f;
    private float timer = 0.0f;
    private float waitTime = 0.0f;

    private Dictionary<string, Material> modes;
    private string enabledMode = "default";


    //-------------------------------------------------
    // Summary :
    //     Continuously changes the pointer laser length given an offset.
    //     To stop it, call this method with a value of 0.0f
    //
    // Param :
    //   value:
    //     The speed value that will be add to the laser
    //-------------------------------------------------
    public void ChangeLaserLength(float value)
    {
        waitTime = 0.01f;
        offset = value/10;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lastLength = defaultLength;
        modes = new Dictionary<string, Material>();
        modes.Add("default", lineRenderer.material);
        modes.Add("delete", deleteMaterial);
        modes.Add("info", infoMaterial);
    }

    private void Update()
    {
        UpdateLineState();
    }

    //-------------------------------------------------
    // Summary :
    //     Controls the active state of the laser
    //
    // Param :
    //   value:
    //     If set to true, laser is active.
    //-------------------------------------------------
    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }

    //-------------------------------------------------
    // Summary :
    //     Autolength is enabled by default. It means the laser reacts with colliders
    //     and adapt its lenght.
    //
    // Param :
    //   value:
    //     If true, laser adapt its lenght with collides, if false, length will be fixed
    //-------------------------------------------------
    public void SetAutoLength(bool value)
    {
        autoLength = value;
    }

    //-------------------------------------------------
    // Summary :
    //     This returns the Gameobject attached to the hand
    //-------------------------------------------------
    public GameObject GetAttachedObject()
    {
        return hand.currentAttachedObject;
    }

    //-------------------------------------------------
    // Summary :
    //     This returns the Gameobject attached to the hand
    //-------------------------------------------------
    public GameObject GetHoverObject()
    {
        return hand.hoveringInteractable.gameObject;
    }

    public void SetMode(string mode)
    {
        enabledMode = mode;
        Material newMaterial = modes[mode];
        lineRenderer.material = newMaterial;
        endDot.gameObject.GetComponent<Renderer>().material = newMaterial;
    }

    private void UpdateLineState()
    {
        PointerEventData data = inputModule.GetData();
        float targetLength = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;

        // Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPosition;

        // Don't update end position until the attached object is released
        if (GetAttachedObject() == null)
        {
            endPosition = transform.position + (transform.forward * targetLength);
        }
        else
        {
            endPosition = transform.position + (transform.forward * lastLength);
        }

        if (autoLength && hit.collider != null && !hit.collider.isTrigger)
        {   
            endPosition = hit.point;
            lastLength = hit.distance;
            InteractWithCollider(hit.collider);
        }
        else if (!autoLength)
        {
            // if there's an offset update lenght otherwise continue using a fixed length
            timer += Time.deltaTime;
            if (offset != 0.0f && timer > waitTime)
            {
                targetLength = lastLength + offset;
                timer = 0.0f;
            }
            else
            {
                targetLength = lastLength;
            }
            lastLength = targetLength;
            endPosition = transform.position + (transform.forward * targetLength);
        }

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
        Physics.Raycast(ray, out hit, lenght);

        return hit;
    }

    private void InteractWithCollider(Collider collider)
    {
        if (enabledMode == "info")
        {
            QueryableObject qo = collider.GetComponent<QueryableObject>();
            Debug.Log("INFO ENABLED");
            if (qo != null)
            {
                string attributesContent = qo.ToString();
                attributeTextArea.GetComponentInParent<Canvas>().enabled = true;
                attributeTextArea.text = attributesContent;
            }
        }
    }
}
