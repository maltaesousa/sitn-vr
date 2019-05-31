using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class ExportSplatmap : ScriptableWizard
{
    public Texture2D splatmap;


    ExportSplatmap()
    {
        splatmap = null;
    }

    [MenuItem("Terrain/Export Splatmap (from WM)")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<ExportSplatmap>("Export Splatmap", "Export");
    }

    void OnWizardCreate()
    {
        try
        {
            byte[] bytes = splatmap.EncodeToPNG();
			File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        }
        catch (UnityException)
        {
            EditorUtility.DisplayDialog("Not readable", "The 'New' splatmap must be readable. Make sure the type is Advanced and enable read/write and try again!", "Cancel");
            return;
        }

    }

    void OnWizardUpdate()
    {
        helpString = "Export the existing splatmap of your terrain with a new one.\n1) Drag the embedded splatmap texture of your terrain to the 'Splatmap box'.\n2) Then drag the Exportment splatmap texture to the 'New' box\n3) Then hit 'Export'.";
        isValid = (splatmap != null);
    }
}