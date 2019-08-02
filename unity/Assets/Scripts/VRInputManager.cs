//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This extends the SteamVR InputModule allowing to add custom actions.
// Some parts are based on "VR with Andrew" YouTube videos.
//
//=====================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;


/// <summary>
/// This manages the inputs from Vive Controller:
///  - the physicall buttons
///  - The Laser Pointer is also considered as an input inside the game
/// </summary>
public class VRInputManager : BaseInputModule
{
    [Header("Actions Sets")]
    [Tooltip("The action set when menu is open")]
    public SteamVR_ActionSet menuSet;
    [Tooltip("The action set when buildings are being placed")]
    public SteamVR_ActionSet movingBuildingsSet;

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
    [Tooltip("The trackpad touch action")]
    public SteamVR_Action_Boolean touchTrackpad;
    [Tooltip("Tracking finger on trackpad action")]
    public SteamVR_Action_Vector2 fingerPosition;

    [Header("Scene Objects")]
    [Tooltip("")]
    public MainMenu mainMenu = null;
    [Tooltip("")]
    public SitnPointer menuPointerWithCamera = null;

    // stores the state of the menu
    private bool active = false;
    private GameObject currentObject = null;
    private PointerEventData data = null;
    private Camera menuPointerCamera = null;

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
        touchTrackpad.onChange += BuildingTouch;
        fingerPosition.onAxis += BuildingRotate;
        data = new PointerEventData(eventSystem);
        menuPointerCamera = menuPointerWithCamera.GetComponent<Camera>();
    }

    protected override void OnDestroy()
    {
        openMenu.onStateDown -= PressRelease;
        grabBuilding.onStateDown -= GrabBuilding;
        grabBuilding.onStateUp -= StopGrabBuilding;
        moveBuilding.onStateDown -= BuildingMove;
        moveBuilding.onStateUp -= BuildingStopMoving;
        touchTrackpad.onChange -= BuildingTouch;
        fingerPosition.onAxis -= BuildingRotate;
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
    public void ToggleMenu(bool pointerIsActive)
    {
        active = !active;
        mainMenu.Show(active);
        menuPointerWithCamera.Show(pointerIsActive);
        if (active)
        {
            menuSet.Activate(touchButtonSource, 2);
        }
        else
        {
            menuSet.Deactivate(touchButtonSource);
        }
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
        ToggleMenu(!active);
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
        currentAttachedObject = menuPointerWithCamera.GetAttachedObject();
        if (currentAttachedObject)
        {
            //print("START MOVING");
            menuPointerWithCamera.SetOffset(0.5f);
        }
        // if finger is 
    }

    //-------------------------------------------------
    // Handles trackpad release action while moving buildings
    //-------------------------------------------------
    private void BuildingStopMoving(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
       // print("STOP MOVING");
    }

    //-------------------------------------------------
    // Handles finger position and rotates building
    //-------------------------------------------------
    private void BuildingTouch(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        //print("Touching");
    }

    //-------------------------------------------------
    // Handles finger position and rotates building
    //-------------------------------------------------
    private void BuildingRotate(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
       // print("ROTATING");
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

}
