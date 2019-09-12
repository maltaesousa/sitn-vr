//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This controls the state of the building
//
//=====================================================================================================================

using UnityEngine;

namespace SITN
{
    //----------------------------------------------------------------------------------------------------------------
    // Playable building is mean de be added to a building wrapper
    // The building wrapper needs a BoxCollider.
    //----------------------------------------------------------------------------------------------------------------
    public class PlayableBuilding : MonoBehaviour
    {
        private GameObject buildingWrapper;
        private readonly Vector3[] colliderVertices = new Vector3[4];
        private bool isValid;
        private Material wallMaterial;
        private readonly string wallMaterialName = "white";

        private void Start()
        {
            buildingWrapper = transform.GetChild(0).gameObject;
        }

        //------------------------------------------------------------------------------------------------------------
        // Retrieves the 4 bottom vertices of the box collider
        //------------------------------------------------------------------------------------------------------------
        public Vector3[] GetColliderVertices()
        {
            BoxCollider bc = transform.GetComponent<BoxCollider>();
            colliderVertices[0] = bc.transform.position + new Vector3(bc.size.x, -bc.size.y, bc.size.z) * 0.5f;
            colliderVertices[1] = bc.transform.position + new Vector3(-bc.size.x, -bc.size.y, bc.size.z) * 0.5f;
            colliderVertices[2] = bc.transform.position + new Vector3(-bc.size.x, -bc.size.y, -bc.size.z) * 0.5f;
            colliderVertices[3] = bc.transform.position + new Vector3(bc.size.x, -bc.size.y, -bc.size.z) * 0.5f;
            return colliderVertices;
        }

        //------------------------------------------------------------------------------------------------------------
        // Setter for building validity
        //------------------------------------------------------------------------------------------------------------
        public void SetValid(bool value)
        {
            if (value != isValid)
            {
                isValid = value;
                ToogleMaterial();
            }
        }

        //------------------------------------------------------------------------------------------------------------
        // TODO: Use original color instead of hardcoded white
        // Changes the color of material according to validity
        //------------------------------------------------------------------------------------------------------------
        private void ToogleMaterial()
        {
            wallMaterial = GetMaterialByName(wallMaterialName);
            if (isValid)
            {
                wallMaterial.color = Color.white;
            } else
            {
                wallMaterial.color = Color.red;
            }
        }

        //------------------------------------------------------------------------------------------------------------
        // Helper to get a material by name. Be careful as
        // material instances are renamed (use of StartsWith())
        //------------------------------------------------------------------------------------------------------------
        private Material GetMaterialByName(string name)
        {
            Material[] materials = buildingWrapper.GetComponentInChildren<Renderer>().materials;
            foreach (Material material in materials)
            {
                if (material.name.StartsWith(name))
                {
                    return material;
                }
            }
            // if no material is found by name, return the first one.
            return materials[0];
        }

        //------------------------------------------------------------------------------------------------------------
        // Triggers the very poor special effects when building touches the ground
        //------------------------------------------------------------------------------------------------------------
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.GetType() == typeof(TerrainCollider))
            {
                ParticleSystem dust = GetComponentInChildren<ParticleSystem>();
                AudioSource boom = GetComponentInChildren<AudioSource>();
                if (dust != null && boom != null)
                {
                    dust.Play(); // Bang bang, I hit the ground
                    boom.Play(); // Bang bang, that awful sound
                }
            }
        }
    }
}
