using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInputManager : Valve.VR.InteractionSystem.InputModule
{
    [Header("Actions")]
    public SteamVR_Action_Boolean pressMenu = null;
    public SteamVR_Action_Boolean pressTouchAction;
    public SteamVR_Input_Sources touchButtonSource;

    [Header("Scene Objects")]
    public MainMenu mainMenu = null;
    public MenuPointer menuPointerWithCamera = null;

    // stores the state of the menu
    private bool active = false;
    private GameObject currentObject = null;
    private PointerEventData data = null;
    private Camera menuPointerCamera = null;

    protected override void Awake()
    {
        base.Awake();
        pressMenu.onStateDown += PressRelease;
        data = new PointerEventData(eventSystem);
        menuPointerCamera = menuPointerWithCamera.GetComponent<Camera>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        pressMenu.onStateDown -= PressRelease;
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
        if (pressTouchAction.GetStateDown(touchButtonSource))
            ProcessTouchPress(data);

        if (pressTouchAction.GetStateUp(touchButtonSource))
            ProcessTouchRelease(data);
    }

    public PointerEventData GetData()
    {
        return data;
    }

    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        active = !active;
        mainMenu.Show(active);
        menuPointerWithCamera.Show(active);
        print("MENU :" + active);
    }

    private void ProcessTouchPress(PointerEventData data)
    {
        print("TOUCH PRESS: " + data);
    }

    private void ProcessTouchRelease(PointerEventData data)
    {
        print(data);
    }

}
