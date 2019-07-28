using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class MenuActions : MonoBehaviour
{
    public float defaultLength = 10f;
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SpawnBuilding(GameObject building)
    {     
        GameObject newObject = Instantiate(building);
        Vector3 buildingPosition = Camera.current.transform.TransformPoint(Vector3.forward * defaultLength);
        newObject.transform.position = buildingPosition;
        newObject.transform.localScale = new Vector3(1f, 1f, 1f);
        newObject.SetActive(true);
    }
}
