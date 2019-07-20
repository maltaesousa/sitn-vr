﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRInputManager : Valve.VR.InteractionSystem.InputModule
{
    [Header("Actions")]
    public SteamVR_Action_Boolean press = null;

    [Header("Scene Objects")]
    public MainMenu mainMenu = null;
    public MenuPointer menuPointer = null;

    private bool active = false;

    protected override void Awake()
    {
        base.Awake();
        press.onStateDown += PressRelease;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        press.onStateDown -= PressRelease;
    }

    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        active = !active;
        mainMenu.Show(active);
        menuPointer.Show(active);
        print("MENU :" + active);
    }
}