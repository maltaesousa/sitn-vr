using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Objects")]
    public Camera thecamera = null;

    public void Show(bool value)
    {
        Debug.Log("MENU: " + value);
        gameObject.SetActive(value);
    }
}

