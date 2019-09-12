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
using Valve.VR.InteractionSystem;

namespace SITN
{
    //--------------------------------------------------------------------------------------------------
    // This manages the inputs from Vive Controller:
    // - the physicall buttons
    // - The Laser Pointer is also considered as an input inside the game
    //--------------------------------------------------------------------------------------------------
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
        [Tooltip("GameObject containing a Main Menu script")]
        public MainMenu mainMenu = null;
        [Tooltip("Sitn Laser Pointer")]
        public SitnPointer menuPointerWithCamera = null;

        private bool menuIsActive = false;                   // stores the state of the menu
        private GameObject currentObject = null;             // object being raycasted
        private PointerEventData data = null;                // the data related to the pointer event
        private Camera menuPointerCamera = null;             // the camera at the end of laser
        private bool trackpadIsPristine = true;              // state of physical trackpad
        private Dictionary<string, SteamVR_ActionSet> modes; // map modes to actionSets
        private Player player;                               // the only current player
        private bool isFirstTimeBuildingMode = true;         // controls if hints should be displayed
        private GameObject currentAttachedObject;            // Active GameObject attached to this Hand

        //--------------------------------------------------------------------------------------------------
        // Register all callbacks for all actions, initialise data and modes
        // This is called only once before the game starts
        //--------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------
        // Unregister all callbacks for all actions, initialise data and modes
        // This is called after Scene is destroyed
        //--------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------
        // Register Player, show Menu Hint, this is called before Awake
        //--------------------------------------------------------------------------------------------------
        protected override void Start()
        {
            base.Start();
            player = Player.instance;
            Invoke("ShowMenuHint", 2.0f);
        }

        //--------------------------------------------------------------------------------------------------
        // Called every frame, makes camera at the end of Sitn Laser Pointer acts like a mouse pointer
        //--------------------------------------------------------------------------------------------------
        public override void Process()
        {
            data.Reset();
            // mimics position of mouse pointer
            data.position = new Vector2(menuPointerCamera.pixelWidth / 2, menuPointerCamera.pixelHeight / 2);
            // then raycasts as mouse pointer
            eventSystem.RaycastAll(data, m_RaycastResultCache);
            data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            currentObject = data.pointerCurrentRaycast.gameObject;

            m_RaycastResultCache.Clear();

            // Controls the hover
            HandlePointerExitAndEnter(data, currentObject);

            // Click
            if (selectInMenu[touchButtonSource].stateDown)
                ProcessTouchPress(data);

            if (selectInMenu[touchButtonSource].stateUp)
                ProcessTouchRelease(data);
        }

        //--------------------------------------------------------------------------------------------------
        // Getter for pointer data
        //--------------------------------------------------------------------------------------------------
        public PointerEventData GetData()
        {
            return data;
        }

        //--------------------------------------------------------------------------------------------------
        // Controls the menu state base on a boolean
        //--------------------------------------------------------------------------------------------------
        private void ToggleMenu(bool pointerIsActive)
        {
            menuIsActive = !menuIsActive;
            mainMenu.Show(menuIsActive);
            menuPointerWithCamera.Show(pointerIsActive);
            if (menuIsActive)
            {
                ActivateActionSetByMode("menu");
            }
            else
            {
                ActivateActionSetByMode("default");
            }
            CancelMenuHint();
        }

        //--------------------------------------------------------------------------------------------------
        // Manages which mode should be enabled before controlling menu state
        //--------------------------------------------------------------------------------------------------
        public void ToggleMenu(bool pointerIsActive, string mode)
        {
            ToggleMenu(pointerIsActive);
            menuPointerWithCamera.SetMode(mode);
            ActivateActionSetByMode(mode);
        }

        //--------------------------------------------------------------------------------------------------
        // Handles physical menu button action
        //--------------------------------------------------------------------------------------------------
        private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            ToggleMenu(!menuIsActive);
        }

        //--------------------------------------------------------------------------------------------------
        // Handles trackpad press action while menu is activated
        //--------------------------------------------------------------------------------------------------
        private void ProcessTouchPress(PointerEventData data)
        {
            // Set Raycast
            data.pointerPressRaycast = data.pointerCurrentRaycast;

            // Check for object hit, get the down handler call
            GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(
                currentObject, data, ExecuteEvents.pointerDownHandler);

            // If no down handler, try to get the click handler
            if (newPointerPress == null)
            {
                newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);
            }

            // set pointer data
            data.pressPosition = data.position;
            data.pointerPress = newPointerPress;
            data.rawPointerPress = currentObject;
        }

        //--------------------------------------------------------------------------------------------------
        // Handles trackpad release action while menu is activated
        //--------------------------------------------------------------------------------------------------
        private void ProcessTouchRelease(PointerEventData data)
        {
            // Execute pointer up
            ExecuteEvents.Execute(currentObject, data, ExecuteEvents.pointerUpHandler);

            // Check for click handler
            GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);

            // Check if actual
            if (data.pointerPress == pointerUpHandler)
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

        //--------------------------------------------------------------------------------------------------
        // Handles trackpad press action while moving buildings
        //--------------------------------------------------------------------------------------------------
        private void BuildingMove(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // the farther from center the User touches the trackpad, the faster the building will move
            // Actually nobody noticed this awesome feature :°(
            float speed = fingerPosition[fromSource].axis.y * 2.0f;
            menuPointerWithCamera.ChangeLaserLength(speed);
        }

        //--------------------------------------------------------------------------------------------------
        // Handles trackpad release action while moving buildings
        //--------------------------------------------------------------------------------------------------
        private void BuildingStopMoving(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            menuPointerWithCamera.ChangeLaserLength(0.0f);
        }

        //--------------------------------------------------------------------------------------------------
        // Handles trackpad untouch and sets its state to pristine
        //--------------------------------------------------------------------------------------------------
        private void TrackpadTouchOut(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            trackpadIsPristine = true;
        }

        //--------------------------------------------------------------------------------------------------
        // Handles finger position and rotates building
        // Needs an PlayableBuilding and collider wrapping in order to work
        //--------------------------------------------------------------------------------------------------
        private void BuildingRotate(
            SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            float minimumAngle = 2; // minimum angle the finger needs to move before building rotates
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

            // wait until minimum angle is reached before starting rotation
            if (Mathf.Abs(angleDiff) > minimumAngle && menuPointerWithCamera.GetAttachedObject() != null)
            {
                // get the building object that is child of a wrapper collider
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

        //--------------------------------------------------------------------------------------------------
        // Handles trigger hold action to grab a building
        //--------------------------------------------------------------------------------------------------
        private void GrabBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            // Stop updating laser length when building is hold
            menuPointerWithCamera.SetAutoLength(false);
        }

        //--------------------------------------------------------------------------------------------------
        // Handles trigger release action to drop the building
        //--------------------------------------------------------------------------------------------------
        private void StopGrabBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            menuPointerWithCamera.SetAutoLength(true);
        }

        //--------------------------------------------------------------------------------------------------
        // Deletes whatever is attached to the laser pointer
        //--------------------------------------------------------------------------------------------------
        private void DeleteBuilding(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (menuPointerWithCamera.GetHoverObject() != null)
            {
                Destroy(menuPointerWithCamera.GetHoverObject());
            }
            else
            {
                Debug.Log("DeleteBuilding was called but nothing has been found!");
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Handles the state of pointer and the actives ActionSets
        //--------------------------------------------------------------------------------------------------
        public void ActivateActionSetByMode(string mode)
        {
            // Clone modes Dictionnary
            Dictionary<string, SteamVR_ActionSet> deactivateModes = new Dictionary<string, SteamVR_ActionSet>(modes);
            // This contains the modes that should be deactivated. Only one active mode is supported
            deactivateModes.Remove(mode);
            Debug.Log("ActivateActionSets called with mode: " + mode);
            // Ensure all modes are deactivated and activates the new one
            for (int actionSetIndex = 0; actionSetIndex < SteamVR_Input.actionSets.Length; actionSetIndex++)
            {
                if (modes[mode] != null && SteamVR_Input.actionSets[actionSetIndex].Equals(modes[mode]))
                {
                    Debug.Log("ACTIVATE " + SteamVR_Input.actionSets[actionSetIndex].fullPath);
                    SteamVR_Input.actionSets[actionSetIndex].Activate(SteamVR_Input_Sources.Any, 2);
                    SteamVR_Input.actionSets[actionSetIndex].Activate(SteamVR_Input_Sources.RightHand, 2);
                }
                else if (deactivateModes.ContainsValue(SteamVR_Input.actionSets[actionSetIndex]))
                {
                    Debug.Log("DEACTIVATE " + SteamVR_Input.actionSets[actionSetIndex].fullPath);
                    SteamVR_Input.actionSets[actionSetIndex].Deactivate(SteamVR_Input_Sources.Any);
                    SteamVR_Input.actionSets[actionSetIndex].Deactivate(SteamVR_Input_Sources.RightHand);
                }
            }

            // Show hints if it's first time to use the moving mode
            if (isFirstTimeBuildingMode && mode == "moving")
            {
                Invoke("ShowMovingHint", 0.5f);
                isFirstTimeBuildingMode = false;
            }

            // TODO: This is dirty, call this after User uses the buttons
            else
            {
                CancelMovingHint();
            }
        }

        //--------------------------------------------------------------------------------------------------
        // Shows help hints for menu
        //--------------------------------------------------------------------------------------------------
        public void ShowMenuHint()
        {
            CancelMenuHint();
            ControllerButtonHints.ShowTextHint(player.rightHand, openMenu, "Menu");
        }

        //--------------------------------------------------------------------------------------------------
        // Hides help hints for menu
        //--------------------------------------------------------------------------------------------------
        public void CancelMenuHint()
        {
            ControllerButtonHints.HideTextHint(player.rightHand, openMenu);
            CancelInvoke("ShowMenuHint");
        }

        //--------------------------------------------------------------------------------------------------
        // Shows help hints for moving buildings actions
        //--------------------------------------------------------------------------------------------------
        public void ShowMovingHint()
        {
            CancelMovingHint();
            ControllerButtonHints.ShowTextHint(
                player.rightHand, grabBuilding, "Maintenir la gachette pour déplacer un bâtiment");
            // TODO: NOT WORKING
            ControllerButtonHints.ShowTextHint(
                player.rightHand, moveBuilding, "Éloigner, Approcher et pivoter un bâtiment");
        }

        //--------------------------------------------------------------------------------------------------
        // Hides help hints for moving buildings actions
        //--------------------------------------------------------------------------------------------------
        public void CancelMovingHint()
        {
            ControllerButtonHints.HideTextHint(player.rightHand, grabBuilding);
            CancelInvoke("ShowMovingHint");
        }
    }
}
