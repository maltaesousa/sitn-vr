using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SitnControllerButtonHints : ControllerButtonHints
{
    private static SitnControllerButtonHints GetControllerButtonHints(SitnGrabber grabber)
    {
        if (grabber != null)
        {
            SitnControllerButtonHints hints = grabber.GetComponentInChildren<SitnControllerButtonHints>();
            if (hints != null && hints.initialized)
            {
                return hints;
            }
        }

        return null;
    }

    public static void ShowButtonHint(SitnGrabber grabber, params ISteamVR_Action_In_Source[] actions)
    {
        SitnControllerButtonHints hints = GetControllerButtonHints(grabber);
        if (hints != null)
        {
            hints.ShowButtonHint( actions );
        }
    }

    //-------------------------------------------------
    public static void HideButtonHint(SitnGrabber grabber, params ISteamVR_Action_In_Source[] actions)
    {
        SitnControllerButtonHints hints = GetControllerButtonHints(grabber);
        if (hints != null)
        {
            hints.HideButtonHint( actions );
        }
    }

    //-------------------------------------------------
    public static void ShowTextHint(SitnGrabber grabber, ISteamVR_Action_In_Source action, string text, bool highlightButton = true)
    {
        SitnControllerButtonHints hints = GetControllerButtonHints(grabber);
        if (hints != null)
        {
            hints.ShowText(action, text, highlightButton);
        }
    }
}

