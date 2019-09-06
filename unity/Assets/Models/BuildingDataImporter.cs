﻿//======================================= 2019, Stéphane Malta e Sousa, sitn-vr =======================================
//
// This script was generated by FME: 00_ImportAttributes
//
//=====================================================================================================================

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace SITN
{
    public class BuildingDataImporter : ScriptableWizard
    {
        private List<string> notFounds = new List<string>();
        private Dictionary<string, Dictionary<string, string>> buildings = new Dictionary<string, Dictionary<string, string>>
        {
           { "102305", new Dictionary<string, string>
                {
                    { "Aire", "85 m²"},
                    { "Altitude", "533.99 m" },
                    { "Propriétaire", "SCHNELLY Benôit et WYSS Tessalia"},
                    { "Servitudes", "Servitude interdisant totalement de bâtir sur le N° 317 du présent article"}
                }
            },
           { "102301", new Dictionary<string, string>
                {
                    { "Aire", "123 m²"},
                    { "Altitude", "532.32 m" },
                    { "Propriétaire", "MUCHAS PATATAS Henrique"},
                    { "Servitudes", "Acte du 13 mars 1897, reçu Eugène BEAUJON, notaire, réglant les rapports tels que : droits de passage, de jours et de surplomb"}
                }
            },
           { "107870", new Dictionary<string, string>
                {
                    { "Aire", "10 m²"},
                    { "Altitude", "533.34 m" },
                    { "Propriétaire", "BARTELS Livia et MOREIRA Marcella"},
                    { "Servitudes", "Ch. Interdiction de modifier le bâtiment, accès aux experts fédéraux"}
                }
            },
           { "107873", new Dictionary<string, string>
                {
                    { "Aire", "57 m²"},
                    { "Altitude", "530.89 m" },
                    { "Propriétaire", "Hoirie LE ROUX"},
                    { "Servitudes", "Passage à pied pour sortie de secours"}
                }
            },
           { "102295", new Dictionary<string, string>
                {
                    { "Aire", "183 m²"},
                    { "Altitude", "529.79 m" },
                    { "Propriétaire", "ALMEIDA Nelson et Monika"},
                    { "Servitudes", "droit de passage perpétuel en faveur des articles 888, 8888, 9999"}
                }
            },
           { "102302", new Dictionary<string, string>
                {
                    { "Aire", "72 m²"},
                    { "Altitude", "533.68 m" },
                    { "Propriétaire", "BARTELS Livia"},
                    { "Servitudes", "Servitude de passage et d'entretien de canaux égouts"}
                }
            },
           { "102307", new Dictionary<string, string>
                {
                    { "Aire", "65 m²"},
                    { "Altitude", "534.24 m" },
                    { "Propriétaire", "BEYELER Florian"},
                    { "Servitudes", "Acte collectif reçu Alphonse Auguste BACHELIN, notaire, constatant le droit de faire usage du canal collectif traversant l'article 1031"}
                }
            },
           { "102312", new Dictionary<string, string>
                {
                    { "Aire", "610 m²"},
                    { "Altitude", "532.82 m" },
                    { "Propriétaire", "NICOLIER Lucie"},
                    { "Servitudes", "Acte reçu Emile Lambelet, notaire, interdisant de bâtir et de rien changer à l'état actuel des lieux "}
                }
            },
           { "102328", new Dictionary<string, string>
                {
                    { "Aire", "245 m²"},
                    { "Altitude", "532.31 m" },
                    { "Propriétaire", "YANG Ting"},
                    { "Servitudes", "Acte reçu Ed. Petitpierre, notaire, interdisant l'exhaussement du mur mitoyen"}
                }
            },
           { "102327", new Dictionary<string, string>
                {
                    { "Aire", "43 m²"},
                    { "Altitude", "533.51 m" },
                    { "Propriétaire", "PERRIARD Marie-José"},
                    { "Servitudes", "Jouissance du garage double, Le présent article est grevé d'une limitation de hauteur des bâtiments"}
                }
            },
           { "102324", new Dictionary<string, string>
                {
                    { "Aire", "249 m²"},
                    { "Altitude", "529.40 m" },
                    { "Propriétaire", "ALMEIDA Nelson et Monika"},
                    { "Servitudes", "droit de passage perpétuel en faveur des articles 888, 8888, 9999"}
                }
            },
           { "102325", new Dictionary<string, string>
                {
                    { "Aire", "78 m²"},
                    { "Altitude", "533.03 m" },
                    { "Propriétaire", "Fonds de la famille TEMMERMAN"},
                    { "Servitudes", "Passage de conduites d'eau et d'électricité"}
                }
            },
           { "102322", new Dictionary<string, string>
                {
                    { "Aire", "327 m²"},
                    { "Altitude", "530.17 m" },
                    { "Propriétaire", "BOVARD Rémi"},
                    { "Servitudes", "Passage à pied et pour tous véhicules"}
                }
            },
           { "102326", new Dictionary<string, string>
                {
                    { "Aire", "7 m²"},
                    { "Altitude", "533.14 m" },
                    { "Propriétaire", "MUCHAS FREITAS Henrique"},
                    { "Servitudes", "Conduite de gaz et conditions, Conduite de téléphone et conditions"}
                }
            },
           { "102351", new Dictionary<string, string>
                {
                    { "Aire", "229 m²"},
                    { "Altitude", "532.61 m" },
                    { "Propriétaire", "MOREIRA Marcella et LE ROUX Christophe"},
                    { "Servitudes", "Servitude de passage et d'entretien de canaux égouts"}
                }
            }
        };


        [MenuItem("SITN/Import building data")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<BuildingDataImporter>("Import building data", "OK!");
        
        }
        
        
        void OnWizardCreate() { }
        void OnWizardUpdate()
        {
            if (notFounds.ToArray().Length > 0)
            {
                helpString = String.Join(", ", notFounds.ToArray()) + " not found!";
                helpString += Environment.NewLine;
                helpString += "Please be sure the names used in this script match the names of your GameObjects.";
            }
        }

        // Find objects and add attributes to them
        private void Awake()
        {
            try
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> entry in buildings)
                {
                    GameObject go = GameObject.Find(entry.Key);
                    if (go != null)
                    {
                        Process("Import", entry, go);
                    } else
                    {
                        notFounds.Add(entry.Key);
                    }
                }
            }
            catch (UnityException)
            {
                EditorUtility.DisplayDialog("Error", "Something went terribly wrong!", "Cancel");
                return;
            }
        }
 
        private void Process(string mode, KeyValuePair<string, Dictionary<string, string>> entry, GameObject go)
        {
            if (mode.ToLower() == "import")
            {
                QueryableBuilding qo = go.AddComponent<QueryableBuilding>();
                qo.aire = entry.Value["Aire"];
                qo.altitude = entry.Value["Altitude"];
                qo.owner = entry.Value["Propriétaire"];
                qo.easements = entry.Value["Servitudes"];
                go.AddComponent<BoxCollider>();
            }
            else
            {
                DestroyImmediate(go.GetComponent<QueryableBuilding>());
                DestroyImmediate(go.GetComponent<BoxCollider>());
            }
        }
    }
}

