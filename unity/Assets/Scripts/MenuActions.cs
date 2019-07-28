using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class MenuActions : MonoBehaviour
{
    public Camera vrCamera;
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnBuilding(GameObject building)
    {

        GameObject newObject = Instantiate(building);
        Vector3 buildingPosition = vrCamera.transform.position + (transform.forward * 5);
        //Destroy(newObject.GetComponent<RectTransform>());
        newObject.transform.position = buildingPosition;
        newObject.transform.localScale = new Vector3(1f, 1f, 1f);
        //Rigidbody gameObjectsRigidBody = newObject.AddComponent<Rigidbody>(); // Add the rigidbody.
        //Throwable gameObjectsThrowable = newObject.AddComponent<Throwable>(); // Add Throwable
        //Interactable gameObjectsInteractable = newObject.GetComponent<Interactable>(); // Add Interactable
        //gameObjectsInteractable.enabled = false;
        //gameObjectsRigidBody.mass = 5; // Set the GO's mass to 5 via the Rigidbody.
        print("BUILDING: ");
    }
}
