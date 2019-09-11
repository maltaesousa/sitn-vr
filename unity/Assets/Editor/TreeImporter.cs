//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This script is intended to wrap playable buildings with a box collider and PlayableBuilding Script
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

        TreeImporter()
        {
            terrain = null;
            treeData = null;
            trees = new List<TreeInstance>();
        }

        [MenuItem("SITN/Import trees")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<TreeImporter>("Import trees", "Import");
        
        }

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

        void OnWizardUpdate()
        {
            helpString = "Before importing trees, please make sure your terrain has at least one Tree prototype";
            isValid = (terrain != null && treeData != null);
        }

        // Autoselect Terrain if one is found
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
