using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class MenuActions : MonoBehaviour
{
    public float defaultLength = 5f;
    public VRInputManager inputModule;
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnBuilding(GameObject building)
    {     
        GameObject newObject = Instantiate(building);
        newObject.GetComponent<Rigidbody>().useGravity = true;
        Vector3 buildingPosition = Camera.main.transform.TransformPoint(Vector3.forward * defaultLength);
        buildingPosition.y = Camera.main.transform.position.y;
        newObject.transform.position = buildingPosition;
        newObject.transform.localScale = new Vector3(1f, 1f, 1f);
        inputModule.ToggleMenu(true);
        inputModule.ActivateActionSet(inputModule.movingBuildingsSet, 1000);
    }
}
