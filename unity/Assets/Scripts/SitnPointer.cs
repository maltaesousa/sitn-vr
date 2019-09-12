//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// Controls color and length of laser
//
//=====================================================================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using TMPro;

namespace SITN
{
    public class SitnPointer : MonoBehaviour
    {
        public float defaultLength = 0.5f;
        public GameObject endDot;                   // the end of pointer
        public Hand hand;                           // player hand at the end of pointer
        public VRInputManager inputModule;          // the main input manager
        public Material deleteMaterial;             // material for delete mode
        public Material infoMaterial;               // material for info mode
        public Canvas attributeTextCanvas;          // canvas to show information
        public TextMeshProUGUI attributeTextArea;   // text area wher to put the information

        private LineRenderer lineRenderer = null;
        private bool autoLength = true;             // true if Laser adapts length to colliders
        private float lastLength = 0.5f;            // length of previous frame
        private float offset = 0.0f;                // grow or reduces laser length
        private float timer = 0.0f;                 // internal timer to control speed of laser length
        private float waitTime = 0.0f;              // time for speed of laser length

        private Dictionary<string, Material> modes; // map modes to materials
        private string enabledMode = "default";     // default mode


        //--------------------------------------------------------------------------------------------------
        // Continuously changes the pointer laser length given an offset.
        // To stop it, call this method with a value of 0.0f
        // Param value:
        //     The speed value that will be add to the laser
        //--------------------------------------------------------------------------------------------------
        public void ChangeLaserLength(float value)
        {
            waitTime = 0.01f;
            offset = value/10;
        }

        //--------------------------------------------------------------------------------------------------
        // Before game starts initialize modes
        //--------------------------------------------------------------------------------------------------
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lastLength = defaultLength;
            modes = new Dictionary<string, Material>();
            modes.Add("default", lineRenderer.material);
            modes.Add("delete", deleteMaterial);
            modes.Add("info", infoMaterial);
        }

        //--------------------------------------------------------------------------------------------------
        // Called every frame
        //--------------------------------------------------------------------------------------------------
        private void Update()
        {
            UpdateLineState();
        }

        //--------------------------------------------------------------------------------------------------
        // Controls the active state of the laser given the incoming boolean
        //--------------------------------------------------------------------------------------------------
        public void Show(bool value)
        {
            gameObject.SetActive(value);
        }

        //--------------------------------------------------------------------------------------------------
        // Autolength is enabled by default. It means the laser reacts with colliders and adapt its lenght.
        // If param value is true, laser adapt its lenght with collides, if false, length will be fixed
        //--------------------------------------------------------------------------------------------------
        public void SetAutoLength(bool value)
        {
            autoLength = value;
        }

        //--------------------------------------------------------------------------------------------------
        // This returns the Gameobject attached to the hand
        //--------------------------------------------------------------------------------------------------
        public GameObject GetAttachedObject()
        {
            return hand.currentAttachedObject;
        }

        //--------------------------------------------------------------------------------------------------
        // This returns the Gameobject attached to the hand
        //--------------------------------------------------------------------------------------------------
        public GameObject GetHoverObject()
        {
            return hand.hoveringInteractable.gameObject;
        }

        //--------------------------------------------------------------------------------------------------
        // This sets mode and changes color of laser and end dot
        //--------------------------------------------------------------------------------------------------
        public void SetMode(string mode)
        {
            attributeTextCanvas.gameObject.SetActive(false);
            enabledMode = mode;
            Material newMaterial = modes[mode];
            lineRenderer.material = newMaterial;
            endDot.gameObject.GetComponent<Renderer>().material = newMaterial;
        }

        //--------------------------------------------------------------------------------------------------
        // Called every frame, controls line length
        //--------------------------------------------------------------------------------------------------
        private void UpdateLineState()
        {

            // Get first lenght if camera at the end of laser raycasts something
            PointerEventData data = inputModule.GetData();
            float targetLength = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;

            // Creates a RaycastHit
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

            // if a collider is found and length is auto
            if (autoLength && hit.collider != null && !hit.collider.isTrigger)
            {
                endPosition = hit.point;
                lastLength = hit.distance;
                InteractWithCollider(hit.collider);
            }
            else if (!autoLength) // if lenght is controlled by user
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
            } else
            {
                // TODO: Dirty, this should be called only once
                attributeTextCanvas.gameObject.SetActive(false);
            }

            // Update position of the dot
            endDot.transform.position = endPosition;

            // Update the line renderer
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPosition);
        }


        //--------------------------------------------------------------------------------------------------
        // Raycasts from begginning of hand to end of pointer and returns whats have been hit
        //--------------------------------------------------------------------------------------------------
        private RaycastHit CreateRaycast(float lenght)
        {
            RaycastHit hit;
            // This creates a ray with two Vector3.
            Ray ray = new Ray(transform.position, transform.forward);
            // This creates the Raycast. the variable hit will contain information about what is hit
            Physics.Raycast(ray, out hit, lenght);

            return hit;
        }

        //--------------------------------------------------------------------------------------------------
        // Activates information canvas and gets attributes from buildings
        //--------------------------------------------------------------------------------------------------
        private void InteractWithCollider(Collider collider)
        {
            if (enabledMode == "info")
            {
                QueryableBuilding qo = collider.GetComponent<QueryableBuilding>();
                if (qo != null)
                {
                    string attributesContent = qo.ToString();
                    attributeTextCanvas.gameObject.SetActive(true);
                    attributeTextArea.text = attributesContent;
                } else
                {
                    attributeTextCanvas.gameObject.SetActive(false);
                }
            }
        }
    }
}
