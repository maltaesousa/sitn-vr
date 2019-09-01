//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This extends the SteamVR InputModule allowing to add custom actions.
// Some parts are based on "VR with Andrew" YouTube videos.
//
//=====================================================================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;


/// <summary>
/// This manages the inputs from Vive Controller:
///  - the physicall buttons
///  - The Laser Pointer is also considered as an input inside the game
/// </summary>
public class VRInputManager : BaseInputModule
{
    [Header("Actions Sets")]
    [Tooltip("The default action set")]
    public SteamVR_ActionSet defaultSet;
    [Tooltip("The action set when menu is open")]
    public SteamVR_ActionSet menuSet;
    [Tooltip("The action set when buildings are being placed")]
    public SteamVR_ActionSet movingBuildingsSet;
    [Tooltip("The action set when buildings are being deleted")]
    public SteamVR_ActionSet deletingBuildingsSet;

    [Header("Menu actions")]
    [Tooltip("The action to open the menu")]
    public SteamVR_Action_Boolean openMenu = null;
    [Tooltip("The action to press a button in the menu")]
    public SteamVR_Action_Boolean selectInMenu;
    public SteamVR_Input_Sources touchButtonSource;

    [Header("Building actions")]
    [Tooltip("The action to grab a building")]
    public SteamVR_Action_Boolean grabBuilding;
    [Tooltip("The action to move a building")]
    public SteamVR_Action_Boolean moveBuilding;
    [Tooltip("The action when finger touches trackpad")]
    public SteamVR_Action_Boolean touchTrackpad;
    [Tooltip("Tracking finger on trackpad action")]
    public SteamVR_Action_Vector2 fingerPosition;
    [Tooltip("The action to delete a building")]
    public SteamVR_Action_Boolean touchDeleteTrackpad;

    [Header("Scene Objects")]
    [Tooltip("")]
    public MainMenu mainMenu = null;
    [Tooltip("")]
    public SitnPointer menuPointerWithCamera = null;

    // stores the state of the menu
    private bool menuIsActive = false;
    private GameObject currentObject = null;
    private PointerEventData data = null;
    private Camera menuPointerCamera = null;
    private bool trackpadIsPristine = true;
    private Dictionary<string, SteamVR_ActionSet> modes;

    //-------------------------------------------------
    // Active GameObject attached to this Hand
    //-------------------------------------------------
    private GameObject currentAttachedObject;

    protected override void Awake()
    {
        openMenu.onStateDown += PressRelease;
        grabBuilding.onStateDown += GrabBuilding;
        grabBuilding.onStateUp += StopGrabBuilding;
        moveBuilding.onStateDown += BuildingMove;
        moveBuilding.onStateUp += BuildingStopMoving;
        touchTrackpad.onStateUp += TrackpadTouchOut;
        fingerPosition.onAxis += BuildingRotate;
        touchDeleteTrackpad.onStateDown += DeleteBuilding;
        data = new PointerEventData(eventSystem);
        menuPointerCamera = menuPointerWithCamera.GetComponent<Camera>();

        modes = new Dictionary<string, SteamVR_ActionSet>
        {
            {"default", null },
            {"info", null },
            {"delete", deletingBuildingsSet },
            {"moving", movingBuildingsSet },
            {"menu", menuSet}
        };
    }

    protected override void OnDestroy()
    {
        openMenu.onStateDown -= PressRelease;
        grabBuilding.onStateDown -= GrabBuilding;
        grabBuilding.onStateUp -= StopGrabBuilding;
        moveBuilding.onStateUp -= BuildingStopMoving;
        moveBuilding.onStateDown -= BuildingMove;
        touchTrackpad.onStateUp -= TrackpadTouchOut;
        fingerPosition.onAxis -= BuildingRotate;
        touchDeleteTrackpad.onStateDown -= DeleteBuilding;

    }

    public override void Process()
    {
        data.Reset();
        data.position = new Vector2(menuPointerCamera.pixelWidth / 2, menuPointerCamera.pixelHeight / 2);
        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        currentObject = data.pointerCurrentRaycast.gameObject;

        m_RaycastResultCache.Clear();

        // Controls the hover
        HandlePointerExitAndEnter(data, currentObject);

        // Click
        if(selectInMenu[touchButtonSource].stateDown)
            ProcessTouchPress(data);

        if(selectInMenu[touchButtonSource].stateUp)
            ProcessTouchRelease(data);
    }

    public PointerEventData GetData()
    {
        return data;
    }

    //-------------------------------------------------
    // Controls the menu state
    //-------------------------------------------------
    private void ToggleMenu(bool pointerIsActive)
    {
        menuIsActive = !menuIsActive;
        mainMenu.Show(menuIsActive);
        menuPointerWithCamera.Show(pointerIsActive);
        if (menuIsActive)
        {
            Debug.Log("ACTIVATE MENU");
            ActivateActionSetByMode("menu");
        }
        else
        {
            Debug.Log("DEACTIVATE MENU");
            ActivateActionSetByMode("default");
        }
    }

    //-------------------------------------------------
    // Controls the menu state
    //-------------------------------------------------
    public void ToggleMenu(bool pointerIsActive, string mode)
    {
        Debug.Log("ToggleMenu called");
        ToggleMenu(pointerIsActive);
        menuPointerWithCamera.SetMode(mode);
        ActivateActionSetByMode(mode);
        
    }

    //-------------------------------------------------
    // Helper to activate an Action Set
    //-------------------------------------------------
    public void ActivateActionSet(SteamVR_ActionSet newActionSet, int priority)
    {
        newActionSet.Activate(SteamVR_Input_Sources.Any, 1);
    }

    //-------------------------------------------------
    // Handles menu button action
    //-------------------------------------------------
    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        ToggleMenu(!menuIsActive);
    }

    //-------------------------------------------------
    // Handles trackpad press action while menu is activated
    //-------------------------------------------------
    private void ProcessTouchPress(PointerEventData data)
    {
        // Set Raycast
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        // Check for object hit, get the down handler call
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(
            currentObject, data, ExecuteEvents.pointerDownHandler);

        // If no down handler, try to get the click handler
        if(newPointerPress == null)
        {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);
        }

        // data
        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = currentObject;
    }

    //-------------------------------------------------
    // Handles trackpad release action while menu is activated
    //-------------------------------------------------
    private void ProcessTouchRelease(PointerEventData data)
    {
        // Execute pointer up
        ExecuteEvents.Execute(currentObject, data, ExecuteEvents.pointerUpHandler);

        // Check for click handler
        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

        // Check if actual
        if(data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        // Clear selected object
        eventSystem.SetSelectedGameObject(null);

        // Reset data
        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;
    }

    //-------------------------------------------------
    // Handles trackpad press action while moving buildings
    //-------------------------------------------------
    private void BuildingMove(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        float speed = fingerPosition[fromSource].axis.y * 2.0f;
        menuPointerWithCamera.ChangeLaserLength(speed);
    }

    //-------------------------------------------------
    // Handles trackpad release action while moving buildings
    //-------------------------------------------------
    private void BuildingStopMoving(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        menuPointerWithCamera.ChangeLaserLength(0.0f);
    }

    //-------------------------------------------------
    // Handles trackpad untouch
    //-------------------------------------------------
    private void TrackpadTouchOut(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        trackpadIsPristine = true;
    }

    //-------------------------------------------------
    // Handles finger position and rotates building
    // Needs an BuildingWrapper prefab
    //-------------------------------------------------
    private void BuildingRotate(
        SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        float minimumAngle = 2;
        float currentAngle = Mathf.Atan2(axis.x, axis.y) * Mathf.Rad2Deg;
        float previousAngle;
        if (trackpadIsPristine)
        {
            previousAngle = currentAngle;
            trackpadIsPristine = false;
        }
        else
        {
            previousAngle = Mathf.Atan2(
                fromAction[fromSource].lastAxis.x, fromAction[fromSource].lastAxis.y) * Mathf.Rad2Deg;
        }
        float angleDiff = currentAngle - previousAngle;

        if (Mathf.Abs(angleDiff) > minimumAngle && menuPointerWithCamera.GetAttachedObject() != null)
        {
            GameObject building = menuPointerWithCamera.GetAttachedObject().transform.GetChild(0).gameObject;
            if (angleDiff > 0)
            {
                building.transform.Rotate(0.0f, 5.0f, 0.0f);
            }
            else
            {
                building.transform.Rotate(0.0f, -5.0f, 0.0f);
            }
        }
    }

    //-------------------------------------------------
    // Handles trigger hold action to grab a building
    //-------------------------------------------------
    private void GrabBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        menuPointerWithCamera.SetAutoLength(false);
    }

    //-------------------------------------------------
    // Handles trigger release action to drop the building
    //-------------------------------------------------
    private void StopGrabBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        menuPointerWithCamera.SetAutoLength(true);
    }


    private void DeleteBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if(menuPointerWithCamera.GetHoverObject() != null)
        {
            Destroy(menuPointerWithCamera.GetHoverObject());
        } else
        {
            Debug.Log("DeleteBuilding was called but nothing has been found!");
        }
    }


    //-------------------------------------------------
    // Handles trigger release action to drop the building
    //-------------------------------------------------
    private void ActivateActionSetByMode(string mode)
    {
        Dictionary<string, SteamVR_ActionSet> deactivateModes = new Dictionary<string, SteamVR_ActionSet>(modes);
        deactivateModes.Remove(mode);
        Debug.Log("ActivateActionSets called with mode: " + mode);
        for (int actionSetIndex = 0; actionSetIndex < SteamVR_Input.actionSets.Length; actionSetIndex++)
        {
            if (modes[mode] != null && SteamVR_Input.actionSets[actionSetIndex].Equals(modes[mode]))
            {
                Debug.Log("ACTIVATE " + SteamVR_Input.actionSets[actionSetIndex].fullPath);
                SteamVR_Input.actionSets[actionSetIndex].Activate(SteamVR_Input_Sources.Any, 2);
                SteamVR_Input.actionSets[actionSetIndex].Activate(SteamVR_Input_Sources.RightHand, 2);
            } else if (deactivateModes.ContainsValue(SteamVR_Input.actionSets[actionSetIndex]))
            {
                Debug.Log("DEACTIVATE " + SteamVR_Input.actionSets[actionSetIndex].fullPath);
                SteamVR_Input.actionSets[actionSetIndex].Deactivate(SteamVR_Input_Sources.Any);
                SteamVR_Input.actionSets[actionSetIndex].Deactivate(SteamVR_Input_Sources.RightHand);
            }
        }

    }
}
