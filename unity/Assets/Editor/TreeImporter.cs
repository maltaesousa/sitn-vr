//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This script imports JSON files as TreeInstances. The JSON must look like this:
// { "trees": [
//  {
//    "coordinates": [0.0206300, 0.5813496],
//    "prototypeIndex": 2, /** make sure to have tree prototypes set up*/
//    "heightScale": 1.00, 
//    "widthScale": 1.00,
//    "rotation" : 6.26 /** optional */
//  }, ...
//
//=====================================================================================================================

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;

namespace SITN
{
    public class TreeImporter : ScriptableWizard
    {
        public Terrain terrain;
        public TextAsset treeData;
        public bool randomRotation;
        public bool replaceExisting;
        private List<TreeInstance> trees;

        public TreeImporter()
        {
            terrain = null;
            treeData = null;
            trees = new List<TreeInstance>();
        }

        //-------------------------------------------------
        // Make it visible as a menu in Editor
        //-------------------------------------------------
        [MenuItem("SITN/Import trees")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<TreeImporter>("Import trees", "Import");
        
        }

        //-------------------------------------------------
        // Called when button Import is clicked
        // Serializes Trees and put them on terrain
        //-------------------------------------------------
        void OnWizardCreate()
        {
            try
            {
                TreeStruct loadedTrees = JsonUtility.FromJson<TreeStruct>(treeData.ToString());
                if (replaceExisting)
                {
                    foreach (TreeStruct.SITNTree tree in loadedTrees.trees)
                    {
                        trees.Add(TreeFactory(tree));
                    }
                    terrain.terrainData.SetTreeInstances(trees.ToArray(), true);
                } else
                {
                    foreach (TreeStruct.SITNTree tree in loadedTrees.trees)
                    {
                        terrain.AddTreeInstance(TreeFactory(tree));
                    }
                }
                Debug.Log("Number of trees: " + terrain.terrainData.treeInstanceCount);
            }
            catch (UnityException)
            {
                EditorUtility.DisplayDialog("Error", "Something went terribly wrong!", "Cancel");
                return;
            }

        }

        //-------------------------------------------------
        // Called when dialog is rendered
        //-------------------------------------------------
        void OnWizardUpdate()
        {
            helpString = "Before importing trees, please make sure your terrain has at least one Tree prototype";
            isValid = (terrain != null && treeData != null);
        }

        //-------------------------------------------------
        // Called when user clicks on the menu entry
        // Gets first Terrain and populate dialog
        //-------------------------------------------------
        private void Awake()
        {
            try
            {
                GameObject[] rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    Terrain currentTerrain = obj.GetComponent<Terrain>();
                    if (currentTerrain != null)
                    {
                        terrain = currentTerrain;
                        // Number of trees on terrain before importing
                        Debug.Log("START Number of trees: " + terrain.terrainData.treeInstanceCount);
                        break;
                    }
                }
            }
            catch (UnityException)
            {
                EditorUtility.DisplayDialog("Error", "Something went terribly wrong!", "Cancel");
                return;
            }
        }

        //-----------------------------------------------------
        // Creates TreeInstance objects based on a tree struct
        //-----------------------------------------------------
        private TreeInstance TreeFactory(TreeStruct.SITNTree tree)
        {
            return new TreeInstance
            {
                position = new Vector3(tree.coordinates[0], 0.0f, tree.coordinates[1]),
                prototypeIndex = tree.prototypeIndex,
                heightScale = tree.heightScale,
                widthScale = tree.widthScale,
                rotation = randomRotation ? Random.Range(0f, 2.0f * Mathf.PI) : tree.rotation
            };
        }
    }
}
