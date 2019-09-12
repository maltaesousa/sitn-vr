//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This script controls what needs to be done once button in menu is clicked
//
//=====================================================================================================================

using UnityEngine;
using UnityEngine.SceneManagement;


namespace SITN
{
    public class MenuActions : MonoBehaviour
    {
        public VRInputManager inputModule; // The input manager where all controllers are
        public GameObject spawnPoint;      // Where the new buildings will appear

        //-------------------------------------------------------------
        // Action that reloads the scene
        //-------------------------------------------------------------
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //-------------------------------------------------------------
        // Clones the building in parameter making it spawn in scene
        // Activates moving mode alowing to move buildings
        //-------------------------------------------------------------
        public void SpawnBuilding(GameObject building)
        {     
            GameObject newObject = Instantiate(building);
            newObject.GetComponent<Rigidbody>().useGravity = true;
            Vector3 buildingPosition = spawnPoint.transform.position;
            newObject.transform.localScale = new Vector3(1f, 1f, 1f);
            newObject.transform.position = buildingPosition;
            inputModule.ToggleMenu(true, "default");
            inputModule.ActivateActionSetByMode("moving");
        }

        //-------------------------------------------------------------
        // Activates info mode alowing to query on buildings
        //-------------------------------------------------------------
        public void ActivateInfo()
        {
            inputModule.ToggleMenu(true, "info");
        }

        //-------------------------------------------------------------
        // Activates moving mode alowing to move buildings
        //-------------------------------------------------------------
        public void MoveBuilding()
        {
            inputModule.ToggleMenu(true, "default");
            inputModule.ActivateActionSetByMode("moving");
        }

        //-------------------------------------------------------------
        // Activates delete mode
        //-------------------------------------------------------------
        public void DeleteBuilding()
        {
            inputModule.ToggleMenu(true, "delete");
        }
    }
}
