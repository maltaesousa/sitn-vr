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


/// <summary>
/// This manages the inputs from Vive Controller:
///  - the physicall buttons
///  - The Laser Pointer is also considered as an input inside the game
/// </summary>
public class VRInputManager : BaseInputModule
{
    [Header("Actions")]
    [Tooltip("The action set when menu is open")]
    public SteamVR_ActionSet menuSet;
    [Tooltip("The action set when buildings are being placed")]
    public SteamVR_ActionSet movingBuildingsSet;
    [Tooltip("The action to open the menu")]
    public SteamVR_Action_Boolean openMenu = null;
    [Tooltip("")]
    public SteamVR_Action_Boolean selectInMenu;
    public SteamVR_Action_Boolean moveBuilding;
    public SteamVR_Action_Vector2 fingerPosition;
    [Tooltip("")]
    public SteamVR_Input_Sources touchButtonSource;

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

    protected override void Awake()
    {
        openMenu.onStateDown += PressRelease;
        moveBuilding.onStateDown += MoveBuilding;
        moveBuilding.onStateUp += StopMoveBuilding;
        data = new PointerEventData(eventSystem);
        menuPointerCamera = menuPointerWithCamera.GetComponent<Camera>();
    }

    protected override void OnDestroy()
    {
        openMenu.onStateDown -= PressRelease;
        moveBuilding.onStateDown -= MoveBuilding;
        moveBuilding.onStateUp -= StopMoveBuilding;
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

    public void ActivateActionSet(SteamVR_ActionSet newActionSet, int priority)
    {
        newActionSet.Activate(SteamVR_Input_Sources.Any, 1);
    }   

    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        ToggleMenu(!active);
    }

    private void ProcessTouchPress(PointerEventData data)
    {
        // Set Raycast
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        // Check for object hit, get the down handler call
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObject, data, ExecuteEvents.pointerDownHandler);

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

    private void MoveBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        print("START MOVING");
    }

    private void StopMoveBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        print("STOP MOVING");
    }
}
