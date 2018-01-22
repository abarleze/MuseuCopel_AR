//#define TESTING_UNINSTALL // Un-comment for in-house testing: Package Uninstaller

using UnityEditor;
//using UnityEngine;
//using System;
//using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XeirGYA
{
    // Catch Editor shutdown?
    //public class GYAAssetModification : UnityEditor.AssetModificationProcessor {
    //	public static string[] OnWillSaveAssets(string[] paths) {
    //		Debug.Log("OnWillSaveAssets");
    //		foreach(string path in paths)
    //			Debug.Log(path);
    //		return paths;
    //	}
    //}

    // Asset Post Processing
    public class GYAPostprocessor : AssetPostprocessor
    {
        static bool packageDetected = false;
        //static Dictionary<string, string> guidList = new Dictionary<string, string>();

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            //Check for imported unitypackages AND ignore changes to ProjectSettings.asset
            if (importedAssets.Any() && Path.GetFileName(importedAssets[0]).ToLower() != "projectsettings.asset")
                //if (importedAssets.Count() > 0 && importedAssets[0].ToLower() != "projectsettings/projectsettings.asset")
                //if (importedAssets.Count() > 0)
            {
#if TESTING_UNINSTALL
				Dictionary<string, string> guidList = new Dictionary<string, string>();
#endif

                foreach (string t in importedAssets)
                {
                    // Check if a unitypackage was extracted into the local project during import
                    if (Path.GetExtension(t).ToLower() == ".unitypackage")
                        packageDetected = true;

                    //Debug.Log("GYAPostprocessor importedAssets: " + importedAssets[i]);

#if TESTING_UNINSTALL // UnitySync  - works with exception noted below
// GYAAsync   - 
// UnityAsync - 

// If importing when Unity is compiling, may receive INVALID RESULTS.
// Check for compiling before allowing import?
// Example: Just saved 'GYAPostprocessor.cs' via SI3 without compiling, started imported:
//
// GYAPostprocessor - Installed: /Users/user/Library/Unity/Asset Store-5.x/Unity Technologies/Editor ExtensionsUtilities/Asset Importer.unitypackage
// WRONG -> "c0f203e80b4bf4a6794c21bf67a61677": "Assets/Plugins/Editor/GrabYerAssets/GYAPostprocessor.cs"
					
					if (GYA.pkgBeingInstalled != null)
						guidList.Add(AssetDatabase.AssetPathToGUID(importedAssets[i]), importedAssets[i]);
#endif
                }

#if TESTING_UNINSTALL
				if (GYA.pkgBeingInstalled != null)
				{
					// Include Path & if AS pkg: ID, vID
					Debug.Log("GYAPostprocessor - Installed: " + GYA.pkgBeingInstalled + "\n");
					GYAExt.LogAsJson(guidList);
					//guidList = null;
					GYA.pkgBeingInstalled = null;
				}
#endif
            }

            // Check for deleted unitypackages
            if (deletedAssets.Any())
            {
                foreach (string t in deletedAssets)
                {
                    // Check if a unitypackage was deleted from the local project
                    if (Path.GetExtension(t).ToLower() == ".unitypackage")
                        packageDetected = true;

                    //Debug.Log("GYAPostprocessor deletedAssets: " + deletedAssets[i]);
                }
            }

            // Check for moved unitypackages
            if (movedAssets.Any())
            {
                foreach (string t in movedAssets)
                {
                    // Check if a unitypackage was moved in the local project
                    if (Path.GetExtension(t).ToLower() == ".unitypackage")
                        packageDetected = true;

                    //Debug.Log("GYAPostprocessor movedAssets: " + movedAssets[i]);
                }
            }

            if (movedFromAssetPaths.Any())
            {

            }

            // If a unitypackage was imported/deleted/moved, rescan the local project
            if (packageDetected)
            {
                packageDetected = false;
                GYAPackage.RefreshProject();
                GYA.Instance.RefreshSV();
            }
        }
    }
}