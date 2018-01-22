using UnityEngine;
using System.Collections;
using UnityEditor;

public class OctionScriptableWizard : ScriptableWizard {

    // name will display in window
    public string MyName = null;

    // name of menu option 
    [MenuItem("Oction / Oction Custom Tool")]
    static void CreateWizard()
    {
        // this function is called when the window is created 
        ScriptableWizard.DisplayWizard<OctionScriptableWizard>("Oction Tools");
    }
    void OnWizardCreate()
    {
        // this function is called when the user presses the create button
        Debug.Log(MyName);
    }
}
