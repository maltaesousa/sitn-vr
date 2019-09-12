//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// Needs to be added to a box collider that has IsTrigger enabled. Checks if building is entirely inside its boundaries
//
//=====================================================================================================================

using UnityEngine;


namespace SITN
{
    public class BuildingAuthorizer : MonoBehaviour
    {
        private BoxCollider bCollider; // box collider

        //--------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        //--------------------------------------------------------------------------------------------------
        void Start()
        {
            bCollider = GetComponent<BoxCollider>();
        }

        //--------------------------------------------------------------------------------------------------
        // Fires every time another collider touches this collider boundaries
        //--------------------------------------------------------------------------------------------------
        private void OnTriggerStay(Collider other)
        {
            Vector3[] groundBounds;
            
            // only if the other collider is a PlayableBuilding
            if (other.GetComponentInParent<PlayableBuilding>() != null)
            {
                PlayableBuilding building = other.GetComponentInParent<PlayableBuilding>();

                // gets the ground vertices from building
                groundBounds = building.GetColliderVertices();
                bool isFullyContained = true;

                // for every ground vertex, control if it's inside
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
}
