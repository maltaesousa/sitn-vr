﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableBuilding : MonoBehaviour
{
    private GameObject building;
    private readonly Vector3[] colliderVertices = new Vector3[4];
    // Start is called before the first frame update

    public Vector3[] GetColliderVertices()
    {
        building = transform.GetChild(0).gameObject;
        BoxCollider bc = transform.GetComponent<BoxCollider>();
        colliderVertices[0] = bc.transform.position + new Vector3(bc.size.x, -bc.size.y, bc.size.z) * 0.5f;
        colliderVertices[1] = bc.transform.position + new Vector3(-bc.size.x, -bc.size.y, bc.size.z) * 0.5f;
        colliderVertices[2] = bc.transform.position + new Vector3(-bc.size.x, -bc.size.y, -bc.size.z) * 0.5f;
        colliderVertices[3] = bc.transform.position + new Vector3(bc.size.x, -bc.size.y, -bc.size.z) * 0.5f;
        return colliderVertices;
    }

    public void SetValid(bool value)
    {
        print("Building is " + value);
    }

}