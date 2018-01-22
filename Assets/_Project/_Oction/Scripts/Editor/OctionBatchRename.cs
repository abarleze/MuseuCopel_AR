using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// To use, select a range of game objets that need renaming, and then choose Tools>Bath Rename... from the application menu
/// to display the Batch Rename tool window.
/// Then enter a new base name for all objects, as well as incremental numbering to be appended to the base.
/// Click Rename button to complete the operation
/// </summary>
public class OctionBatchRename : OctionScriptableWizard
{
    // Base name
    public string BaseName = "MyObject_";

    // Start Count
    public int StartNumber = 0;

    // Increment 
    public int Increment = 1;

    [MenuItem("Oction / Batch Rename...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard(" Batch Rename", typeof(OctionBatchRename), " Rename");
    }

    // Called when the window first appears 
    void OnEnable()
    {
        UpdateSelectionHelper();
    }

    // Function called when selection changes in scene 
    void OnSelectionChange()
    {
        UpdateSelectionHelper();
    }

    // Update selection counter 
    void UpdateSelectionHelper()
    {
        helpString = "";

        if (Selection.objects != null)
            helpString = "Number of objects selected: " + Selection.objects.Length;
    }

    // Rename
    void OnWizardCreate()
    {
        // If selection empty, then exit 
        if (Selection.objects == null)
            return;

        // Current Increment 
        int PostFix = StartNumber;

        //Cycle and rename 
        foreach (Object O in Selection.objects)
        {
            O.name = BaseName + PostFix;
            PostFix += Increment;
        }
    }
}
