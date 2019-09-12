//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This script is intended to wrap playable buildings with a box collider and PlayableBuilding Script
//
//=====================================================================================================================

using UnityEditor;
using UnityEngine;


namespace SITN
{
    public class CreateBoundingCollider: ScriptableWizard
    {
        public GameObject building;


        public CreateBoundingCollider()
        {
            building = null;
        }

        //-------------------------------------------------
        // Make it visible as a menu in Editor
        //-------------------------------------------------
        [MenuItem("SITN/Create bounding collider")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<CreateBoundingCollider>("Create bounding collider", "Create");
        }

        //--------------------------------------------------------------------------------------------------
        // Wraps a GameObject with a collider allowing the GameObject to be manipulated inside the wrapper
        // Adds PlayableBuilding script
        //--------------------------------------------------------------------------------------------------
        void OnWizardCreate()
        {
            try
            {
                Bounds bounds = building.GetComponent<Renderer>().bounds;
                GameObject newParent = new GameObject(building.name + "_wrapper");
                newParent.transform.position = building.transform.position;
                building.transform.SetParent(newParent.transform);
                newParent.AddComponent<PlayableBuilding>();
                BoxCollider bc = newParent.AddComponent<BoxCollider>();
                bc.center = bounds.center;
                bc.size = bounds.size;
            }
            catch (UnityException)
            {
                EditorUtility.DisplayDialog("Error", "Something went terribly wrong!", "Cancel");
                return;
            }

        }

        void OnWizardUpdate()
        {
            isValid = (building != null);
        }
    }
}