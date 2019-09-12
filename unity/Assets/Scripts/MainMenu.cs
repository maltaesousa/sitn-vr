//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// Main menu state control
//
//=====================================================================================================================

using UnityEngine;

namespace SITN
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Scene Objects")]
        public Camera thecamera = null; // expose camera

        //-------------------------------------------------
        // Activates menu
        //-------------------------------------------------
        public void Show(bool value)
        {
            Debug.Log("MENU: " + value);
            gameObject.SetActive(value);
        }
    }

}
