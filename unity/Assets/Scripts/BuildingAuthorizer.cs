using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAuthorizer : MonoBehaviour
{
    private BoxCollider bCollider;
    // Start is called before the first frame update
    void Start()
    {
        bCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3[] groundBounds;
        if (other.GetComponentInParent<PlayableBuilding>() != null)
        {
            PlayableBuilding building = other.GetComponentInParent<PlayableBuilding>();
            groundBounds = building.GetColliderVertices();
            bool isFullyContained = true;
            for (int i = 0; i < groundBounds.Length; i++)
            {
                if (!bCollider.bounds.Contains(groundBounds[i]))
                {
                    isFullyContained = false;
                }
            }
            building.SetValid(isFullyContained);
        }
    }
}
