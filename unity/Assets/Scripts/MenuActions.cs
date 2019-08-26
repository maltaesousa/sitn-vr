using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class MenuActions : MonoBehaviour
{
    public VRInputManager inputModule;
    public GameObject spawnPoint;
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnBuilding(GameObject building)
    {     
        GameObject newObject = Instantiate(building);
        newObject.GetComponent<Rigidbody>().useGravity = true;
        Vector3 buildingPosition = spawnPoint.transform.position;
        newObject.transform.localScale = new Vector3(1f, 1f, 1f);
        newObject.transform.position = buildingPosition;
        inputModule.ToggleMenu(true, "default");
        inputModule.ActivateActionSet(inputModule.movingBuildingsSet, 1000);
    }

    public void ActivateInfo()
    {
        inputModule.ToggleMenu(true, "info");
    }

    public void MoveBuilding()
    {
        inputModule.ToggleMenu(true, "default");
        inputModule.ActivateActionSet(inputModule.movingBuildingsSet, 1000);
    }

    public void DeleteBuilding()
    {
        inputModule.ToggleMenu(true, "delete");
    }
}
