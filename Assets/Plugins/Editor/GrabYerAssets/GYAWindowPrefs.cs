﻿#if UNITY_5 || UNITY_2017
#define UNITY_5_0_OR_NEWER
#endif

#if (UNITY_5_0_OR_NEWER && !UNITY_5_0)
#define UNITY_5_1_OR_NEWER
#endif

#if (UNITY_5_1_OR_NEWER && !UNITY_5_1)
#define UNITY_5_2_OR_NEWER
#endif

#if (UNITY_5_2_OR_NEWER && !UNITY_5_2)
#define UNITY_5_3_OR_NEWER
#endif

//#define TEST_PAL

// Unity 5.3.4 and newer, auto assigns: UNITY_x_y_OR_NEWER

using UnityEngine;
using UnityEditor;
//using UnityEditor.AnimatedValues;
//using UnityEditorInternal;
using System;
using System.IO;
//using System.Linq;
//using System.Collections;
using System.Collections.Generic;

namespace XeirGYA
{
    public class GYAWindowPrefs : EditorWindow
    {
        internal Vector2 svPosition; //, svPosition2;

        internal static int toolbarInt; // Set default tab when opening window

        //internal static int grpInt;
        public string[] toolbarStrings =
            {"Quick Ref", "Preferences", "User Folders", "Groups", "Maintenance", "Info"};
        //{"Quick Ref", "Preferences", "User Folders", "Groups", "Old Assets", "Maintenance", "Info"};

        internal string kharmaSessionID = "";

        public static void Init(int pVal = 1)
        {
            toolbarInt = pVal;

            float width = 700f;
            float height = 520f;

            var window = (GYAWindowPrefs) GetWindow(typeof(GYAWindowPrefs), true, "GrabYerAssets Preferences", true);
            window.minSize = new Vector2(width, height);
            window.maxSize = new Vector2(width, height);
            window.CenterOnMainWin();
        }

        void OnDestroy()
        {
            GYAFile.SaveGYAPrefs();
            //GYA.Instance.BuildPrevNextList();
            //GYA.Instance.CheckIfGUISkinHasChanged(true); // Force reload
            GYA.Instance.Focus();
        }

        void OnGUI()
        {
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.Height(26f));
            switch (toolbarInt)
            {
                case 0:
                    ShowQuickRef();
                    break;
                case 1:
                    ShowPreferences();
                    break;
                case 2:
                    ShowUserFolders();
                    break;
                case 3:
                    ShowGroups();
                    break;
                //case 4:
                    //ShowOldAssets();
                    //break;
                case 4:
                    ShowMaintenance();
                    break;
                case 5:
                    ShowStatus();
                    break;
                default:
                    break;
            }
        }

        void ShowQuickRef()
        {
            //EditorGUILayout.PrefixLabel("");
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(340));

            EditorGUILayout.HelpBox("General Usage:\n" +
                                    "\nRefresh Icon\t= Click to rescan your downloaded packages" +
                                    "\nLeft-Click  \t= Toggle asset for multi-import" +
                                    "\nRight-Click\t= Display asset specific popup", MessageType.None);
            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox("Color Chart for Collections:\n" +
                                    "\nGreen\t= Asset Store Assets" +
                                    "\nBlue\t= User Assets" +
                                    "\nPlum\t= Standard Assets" +
                                    "\nOrange\t= Old Assets that HAVE been consolidated" +
                                    "\nRed\t= Old Assets that HAVE NOT been consolidated", MessageType.None);
            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox("Icons In List (LEFT Side):\n" +
                                    "\nSolid\t= Unity Assets for the Current Running Version" +
                                    "\nOutline\t= Unity Assets for Older Unity Versions" +
                                    "\n\nCube\t= Asset Store Asset" +
                                    "\nPerson\t= User Asset (Contained within a User Folder)" +
                                    "\nPuzzle\t= Standard Asset" +
                                    "\nCircle\t= Old Asset" +
                                    //, MessageType.None);
                                    //EditorGUILayout.PrefixLabel("");

                                    //EditorGUILayout.HelpBox(
                                    "\n\nPinned Icons in List (RIGHT Side):\n" +
                                    "\nYellow Star\t\t= Favorite (is part of the Favorites group)" +
                                    "\nOrange Hazard\t= Deprecated" +
                                    "\nGreeen Down Arrow\t= Not Downloaded" +
                                    "\nRed Warning\t= Damaged" +
                                    "\n\nNOTE: The version of Unity that a package was submitted with does not necessarily relate to what versions of Unity that package may support."
                , MessageType.None);

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(340));

            EditorGUILayout.HelpBox("1st Row Icons:\n" +
                                    "\nGear\t= Main Menu" +
                                    "\nPages\t= Categories Drop-down" +
                                    "\nPeople\t= Publishers Drop-down" +
                                    "\nMagnifier\t= Sort & Search Drop-down" +
                                    "\nTextfield\t= Search for Assets" +
                                    "\nRefresh\t= Refresh the Package List", MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox("2nd Row Icons:\n" +
                                    "\n#\t= Multi-Asset Drop-down" +
                                    "\nReset\t= Reset main view back to defaults" +
                                    "\nTitle Bar\t= Collection/Group Drop-down" +
                                    "\nUp\t= Move Up a Collection/Group" +
                                    "\nDown\t= Move Down a Collection/Group", MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox("3rd Row Icons:\n" +
                                    "\n#\t= Assets in current list" +
                                    "\nIcons\t= In order: All, Store, User, Standard, Old" +
                                    "\n\nSelect an icon from above to quickly change to a selected collection",
                MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox("Bottom Row Foldout:\n" +
                                    "\nShows general package info for the hi-lighted asset." +
                                    //, MessageType.None);

                                    //EditorGUILayout.PrefixLabel("");
                                    //EditorGUILayout.HelpBox(
                                    "\n\nMenu Items:\n" +
                                    "\n* = 'Purchased Assets' list required to function", MessageType.None);

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PrefixLabel("");
        }

        // General Settings

        void ShowPreferences()
        {
            //EditorGUILayout.PrefixLabel("");
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Application Options");
            EditorGUI.indentLevel++;

            // Disable for Linux until proper handling for the 'Standard Assets' folder
            if (GYAExt.IsOSLinux)
                GYA.gyaVars.Prefs.isPersist = false;

            //EditorGUI.BeginDisabledGroup (!GYA.gyaVars.Prefs.isPersist);
            GYA.gyaVars.Prefs.isPersist = EditorGUILayout.ToggleLeft("Persist Mode", GYA.gyaVars.Prefs.isPersist);
            //EditorGUI.EndDisabledGroup();


            //gyaVars.Prefs.reportPackageErrors = EditorGUILayout.ToggleLeft("Report Asset Errors", gyaVars.Prefs.reportPackageErrors);
            GYA.gyaVars.Prefs.isSilent = EditorGUILayout.ToggleLeft("Silent Mode", GYA.gyaVars.Prefs.isSilent);
            bool bToolbarCollections = GYA.gyaVars.Prefs.enableToolbarCollections;
            GYA.gyaVars.Prefs.enableToolbarCollections = EditorGUILayout.ToggleLeft("Show Toolbar: Collections",
                GYA.gyaVars.Prefs.enableToolbarCollections);
            EditorGUI.indentLevel--;

            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("Enable Toolbar");
            ////EditorGUILayout.DelayedTextField("Application Options", "Label2");
            //EditorGUI.indentLevel++;

            //bool bToolbarCollections = GYA.gyaVars.Prefs.enableToolbarCollections;
            //GYA.gyaVars.Prefs.enableToolbarCollections = EditorGUILayout.ToggleLeft("Collections", GYA.gyaVars.Prefs.enableToolbarCollections);

            //EditorGUI.indentLevel--;

            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("Progress Bar Options");
            //EditorGUI.indentLevel++;
            //gyaVars.Prefs.showProgressBarDuringRefresh = EditorGUILayout.ToggleLeft("Show during Refresh", gyaVars.Prefs.showProgressBarDuringRefresh);
            //gyaVars.Prefs.showProgressBarDuringFileAction = EditorGUILayout.ToggleLeft("Show during File Actions", gyaVars.Prefs.showProgressBarDuringFileAction);
            //EditorGUI.indentLevel--;

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.LabelField("Display Options");
            EditorGUI.indentLevel++;
            //gyaVars.Prefs.nestedDropDowns = EditorGUILayout.ToggleLeft("Nested Cat/Pub Drop-downs", gyaVars.Prefs.nestedDropDowns);
            //gyaVars.Prefs.nestedVersions = EditorGUILayout.Toggle("Nested Versions in List", gyaVars.Prefs.nestedVersions);

            // TBPopUpCallback ("Headers");
            bool bHeaders = GYA.gyaVars.Prefs.enableHeaders;
            GYA.gyaVars.Prefs.enableHeaders = EditorGUILayout.ToggleLeft("Headers", GYA.gyaVars.Prefs.enableHeaders);

            // Enable/Disable Color
            var bColor = GYA.gyaVars.Prefs.enableColors;
            GYA.gyaVars.Prefs.enableColors = EditorGUILayout.ToggleLeft("Colors", GYA.gyaVars.Prefs.enableColors);

            // Enable/Disable Icons
            var bCollectionIcons = GYA.gyaVars.Prefs.enableCollectionTypeIcons;
            GYA.gyaVars.Prefs.enableCollectionTypeIcons =
                EditorGUILayout.ToggleLeft("Icons", GYA.gyaVars.Prefs.enableCollectionTypeIcons);

            // Enable/Disable Outline Icons
            var bAltIcon = GYA.gyaVars.Prefs.enableAltIconForOldVersions;
            GYA.gyaVars.Prefs.enableAltIconForOldVersions =
                EditorGUILayout.ToggleLeft("Alt Icons for Old Package Versions",
                    GYA.gyaVars.Prefs.enableAltIconForOldVersions);
	        //EditorGUILayout.LabelField("(Note: Alt Icons tempoarily disabled.)");
	        EditorGUI.indentLevel--;

            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("Old Asset Handling");
            //EditorGUI.indentLevel++;
            //GYA.gyaVars.Prefs.autoConsolidate = EditorGUILayout.ToggleLeft("Auto Consolidate", GYA.gyaVars.Prefs.autoConsolidate);
            ////gyaVars.Prefs.autoDeleteConsolidated = EditorGUILayout.ToggleLeft("Auto Delete Consolidated", gyaVars.Prefs.autoDeleteConsolidated);
            //EditorGUI.indentLevel--;

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.LabelField("Asset Store Options");
            EditorGUI.indentLevel++;

            // Deprecated
            //gyaVars.Prefs.scanAllAssetStoreFolders = EditorGUILayout.ToggleLeft("Scan All Asset Store Folders", gyaVars.Prefs.scanAllAssetStoreFolders);
            // Deprecated
            //gyaVars.Prefs.showAllAssetStoreFolders = EditorGUILayout.ToggleLeft("Show All Asset Store Folders", gyaVars.Prefs.showAllAssetStoreFolders);
            //EditorGUI.indentLevel--;

            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("Asset Store Processing");
            //EditorGUI.indentLevel++;
            //gyaVars.Prefs.getPackagesInfoFromUnity = EditorGUILayout.ToggleLeft("Retrieve Package Icons (Local)", gyaVars.Prefs.getPackagesInfoFromUnity);
            GYA.gyaVars.Prefs.getPurchasedAssetsListDuringRefresh =
                EditorGUILayout.ToggleLeft("Retrieve Purchased Assets List",
                    GYA.gyaVars.Prefs.getPurchasedAssetsListDuringRefresh);
            EditorGUI.indentLevel++;
            //gyaVars.Prefs.updateOnChangesOnly = EditorGUILayout.ToggleLeft("Update On Changes Only", gyaVars.Prefs.updateOnChangesOnly);
            //EditorGUI.indentLevel--;
            //gyaVars.Prefs.autoDeleteConsolidated = EditorGUILayout.ToggleLeft("Auto Delete Consolidated", gyaVars.Prefs.autoDeleteConsolidated);
            EditorGUI.indentLevel--;

            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("Asset Protection");
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("  ", GUILayout.ExpandWidth(false));
            //gyaVars.Prefs.AutoPreventASOverwrite = EditorGUILayout.ToggleLeft("Auto Protect Asset Store Files", gyaVars.Prefs.AutoPreventASOverwrite);
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("  ", GUILayout.ExpandWidth(false));
            //gyaVars.Prefs.autoBackup = EditorGUILayout.ToggleLeft("Enable Auto Backup", gyaVars.Prefs.autoBackup);
            //GYA.Instance.TBPopUpCallback("AutoBackup");
            //EditorGUILayout.EndHorizontal();

            // Open (x) URLs Per Batch -
            //EditorGUILayout.PrefixLabel("");
            GYA.gyaVars.Prefs.openURLInUnity =
                EditorGUILayout.ToggleLeft("Open URLs In Unity", GYA.gyaVars.Prefs.openURLInUnity);
            EditorGUILayout.LabelField("Open (x) URLs Per Batch");
            GYA.gyaVars.Prefs.urlsToOpenPerBatch =
                EditorGUILayout.IntSlider(GYA.gyaVars.Prefs.urlsToOpenPerBatch, 1, 30, GUILayout.MaxWidth(230));

            // Multi-import
            //EditorGUILayout.PrefixLabel("");
            GYA.gyaVars.Prefs.multiImportOverride =
                (GYAImport.MultiImportType) EditorGUILayout.EnumPopup("Multi-Import Override:",
                    GYA.gyaVars.Prefs.multiImportOverride, GUILayout.MaxWidth(230));

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(400));

            EditorGUILayout.HelpBox(
                "Persist Mode - Maintains the latest version of GYA within the 'Standard Assets' folder for quickly adding GYA into a new project via 'Assets->Import Package'." +
                "\n\nSilent Mode - Hide tooltips & most console messages." +
                "\n\nCollections Toolbar - Enable/Disable the Collections Toolbar"
                //+ "\n\nReport Asset Errors - Errors detected when reading the JSON data contained within unityPackages will be noted in the console when performing a 'Refresh'.  Damaged assets will have a RED Warning icon on the right side of the list."
                , MessageType.None);
            EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.PrefixLabel("");

            //EditorGUILayout.HelpBox("Collections Toolbar - Enable/Disable the Collections Toolbar", MessageType.None);
            //EditorGUILayout.PrefixLabel("");
            ////EditorGUILayout.PrefixLabel("");

            EditorGUILayout.HelpBox(
                //"Nested Cat/Pub Drop-downs - Change the way data is displayed in the Category & Publisher drop-down menus." +
                //"\n\nHeaders/Colors/Icons - Change how data is displayed in the asset list."
                "Headers/Colors/Icons - Change appearance of the asset list." +
                "\n\nAlt Icons - Packages published with an older major Unity version will show an alternate icon.",
                MessageType.None);
            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.PrefixLabel("");

            //EditorGUILayout.PrefixLabel("");

            //EditorGUILayout.HelpBox("Auto Consolidate - Automatically cleans up assets that may be left-over when upgrading assets via the Asset Store.  Old Assets will be moved to the 'Old Assets' Folder.  The newest version of each asset will remain in the 'Asset Store' folder.  Consolidated assets can be deleted via the 'Old Assets' tab." // or by enabling the following toggle."
            //    //+ "\n\nAuto Delete Consolidated - Automatically delete all Consolidated assets.  This will keep your 'Asset Store' folder clean of any old versions of assets.", MessageType.None);
            //EditorGUILayout.PrefixLabel("");

            EditorGUILayout.HelpBox(
                //"Retrieve Package Icons - Refresh Unity's built-in info for packages in the 'Asset Store' folder.  The ONLY feature this adds to GYA's data is the package icons.\n\n" +
                "Purchased Asset List (PAL) - Enables denoting deprecated assets and access to menu options marked with a (*) by downloading your Assets List."
                + "  This will be updated during 'Refresh' if GYA detects a change in the Asset Store folder."
                //+ "\n\nRequires that you have logged into the Asset Store from within Unity and checked remember login."
                ,
                MessageType.None);
            //EditorGUILayout.PrefixLabel("");

            EditorGUILayout.HelpBox(
    "Open URLs In Unity - Toggle between opening URLs in Unity or Browser." +
    "\nDoes not apply to 'Open URL of Selected Packages'", MessageType.None);
            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.PrefixLabel("");

            EditorGUILayout.HelpBox("How many URls to open when selecting 'Open URL of Selected Packages'",
                MessageType.None);
            //EditorGUILayout.PrefixLabel("");

            EditorGUILayout.HelpBox(
                "Multi Import Override - Override the method used to perform Multi-Import's." +
                "\'Default will select the best option for current version of Unity." +
                //"\n\n5.0x - 5.2x\tDefault = Unity Async\n5.3x\t\tDefault = GYA Sync\n5.4x +\t\tDefault = Unity Sync"
                //"\n\nDefaults:\t(5.0x - 5.2x Unity Async)\t(5.3x GYA Sync)\t(5.4x + Unity Sync)", MessageType.None);
                "\nGYA may override selected option if it is known to cause problems.", MessageType.None);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PrefixLabel("");

            // Update Main Window
            if (bHeaders != GYA.gyaVars.Prefs.enableHeaders)
                GYA.Instance.svMain.headerCount =
                    GYA.Instance.SVGetHeaderCount(); // Count optional lines/headers to draw

            if (bColor != GYA.gyaVars.Prefs.enableColors)
                GYA.Instance.CheckIfGUISkinHasChanged(true); // Force reload

            if (
                bToolbarCollections != GYA.gyaVars.Prefs.enableToolbarCollections ||
                bHeaders != GYA.gyaVars.Prefs.enableHeaders ||
                bColor != GYA.gyaVars.Prefs.enableCollectionTypeIcons ||
                bCollectionIcons != GYA.gyaVars.Prefs.enableCollectionTypeIcons ||
                bAltIcon != GYA.gyaVars.Prefs.enableAltIconForOldVersions
            )
            {
                GYA.Instance.Repaint();
                GYAFile.SaveGYAPrefs();
            }
        }

        // User Folders

        void ShowUserFolders()
        {
            int pathCount = GYA.gyaVars.Prefs.pathUserAssets.Count;

            //EditorGUILayout.PrefixLabel("");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("");
            // Add new Folder
            if (GUILayout.Button("Add New Folder", GUILayout.ExpandWidth(false)))
            {
                string pathTemp =
                    EditorUtility.SaveFolderPanel(GYA.gyaVars.abbr + " Select User Assets Folder:", null, null);
                if (pathTemp != "")
                {
                    GYA.gyaVars.Prefs.pathUserAssets.Add("");
                    GYA.gyaVars.Prefs.pathUserAssets[pathCount] = pathTemp;
                    GYAFile.SaveGYAPrefs();
                }
            }
            EditorGUILayout.PrefixLabel("");
            // Apply
            if (GUILayout.Button("Refresh User Folder", GUILayout.Width(130)))
            {
                GYAPackage.RefreshUser();
                GYA.Instance.RefreshSV();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.PrefixLabel("");

            // -- Begin SV

            // Linux beta error fix: NullReferenceException ??
            try
            {
                svPosition = EditorGUILayout.BeginScrollView(svPosition, false, false, GUILayout.ExpandWidth(true));
            }
            catch (Exception)
            {
            }


            for (int i = 0; i < pathCount; i++)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField((i + 1).ToString(), GUILayout.Width(40));
                // Change
                if (GUILayout.Button("Change", GUILayout.ExpandWidth(false)))
                {
                    string pathTemp = EditorUtility.SaveFolderPanel(GYA.gyaVars.abbr + " Select User Assets Folder:",
                        GYA.gyaVars.Prefs.pathUserAssets[i], "");
                    if (pathTemp != "")
                    {
                        GYA.gyaVars.Prefs.pathUserAssets[i] = pathTemp;
                        GYAFile.SaveGYAPrefs();
                    }
                }

                // Show button if not last/empty entry
                if (GYA.gyaVars.Prefs.pathUserAssets[i] != "")
                {
                    if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
                    {
                        GYA.gyaVars.Prefs.pathUserAssets.RemoveAt(i);
                        GYAFile.SaveGYAPrefs();
                        continue;
                    }
                }
                EditorGUILayout.HelpBox(GYA.gyaVars.Prefs.pathUserAssets[i], MessageType.None);

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            // Linux beta error fix: NullReferenceException ??
            try
            {
                EditorGUILayout.EndScrollView();
            }
            catch (Exception)
            {
            }

            // -- End SV

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(280));

            EditorGUILayout.HelpBox("The first folder is considered the\nDEFAULT folder for 'Copy To User Folder'.",
                MessageType.Info);
            EditorGUILayout.HelpBox(
                "After adding any/all folders as desired, click the 'Refresh' button to scan the newly added folders.",
                MessageType.Info);
            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.HelpBox("You can set as many User Folders as is required.", MessageType.None);
            EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.HelpBox("Do NOT assign the 'Asset Store' Folder or a Sub-Folder within as a User Folder." + "\n\nDo NOT assign a User Folder within another User Folder." + "\n\nDoing either of these MAY result in unwanted duplicates in the list and have unforeseen consequences should you perform any File Actions on them." + "\n\nExample: Overlapping User folders COULD make you think you have duplicate files.  Deleting the duplicate in this case would likely have the consequence of actually deleting the original as they would be one in the same.", MessageType.Warning);
            EditorGUILayout.HelpBox(
                "It is *not* recommended to assign the 'Asset Store' Folder or a Sub-Folder within, as a User Folder." +
                "\n\nIt is *not* recommended to assign a User Folder within another User Folder." +
                "\n\nDoing either of these MAY result in unwanted duplicates in the list and have unforeseen consequences should you perform any File Actions on them." +
                "\n\nExample: Overlapping User folders COULD make you think you have duplicate files.  Deleting the duplicate in this case would likely have the consequence of actually deleting the original as they would be one in the same.",
                MessageType.Warning);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PrefixLabel("");
        }

        // Groups

        void ShowGroups()
        {
            //EditorGUILayout.PrefixLabel("");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("");
            // Add new Group
            if (GUILayout.Button("Add New Group", GUILayout.ExpandWidth(false)))
            {
                GYA.Instance.GroupCreate("New Group");
                GYA.Instance.BuildPrevNextList();
            }
            EditorGUILayout.PrefixLabel("");
            // Apply
            if (GUILayout.Button("Apply Changes", GUILayout.Width(100)))
            {
                GYA.Instance.BuildPrevNextList();
                GYAFile.SaveGYAGroups();
            }
            //EditorGUILayout.LabelField("Accepted Characters: a-zA-Z0-9 ()-+_");
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // Field labels
            EditorGUILayout.PrefixLabel("");
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(62));
            EditorGUILayout.LabelField("Group Name:", GUILayout.Width(186));
            EditorGUILayout.LabelField("# of Assets:", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // View/Edit all Groups
            svPosition = EditorGUILayout.BeginScrollView(svPosition, false, false, GUILayout.ExpandWidth(true));

            for (int i = 0; i < GYA.gyaData.Groups.Count; i++)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField((i).ToString(), GUILayout.Width(40));

                EditorGUI.BeginDisabledGroup((i == 0));
                // Remove
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog(GYA.gyaVars.abbr + " - Remove Selected Group",
                        "Are you sure you want to REMOVE this group?\n\n" +
                        "No Assets will be deleted, only the virtual group.\n\n" +
                        GYA.gyaData.Groups[i].name
                        , "Cancel", "REMOVE"))
                    {
                        // Cancel as default - Do nothing
                    }
                    else
                    {
                        GYA.Instance.GroupDelete(i);
                        //grpItems.RemoveAt(i);
                        GYA.Instance.BuildPrevNextList();
                    }
                }
                EditorGUI.EndDisabledGroup();

                // If Group then show Name & Count
                // Rename

                if (i <= GYA.gyaData.Groups.Count - 1)
                {
                    // Only allow these chars
                    char chr = Event.current.character;
                    if (
                        (chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9') &&
                        (chr != ' ') && (chr != '(') && (chr != ')') && (chr != '-') && (chr != '+') && (chr != '_')
                    )
                    {
                        Event.current.character = '\0';
                    }
                    // Group Name
                    EditorGUI.BeginDisabledGroup((i == 0));
                    GYA.gyaData.Groups[i].name =
                        EditorGUILayout.TextField(GYA.gyaData.Groups[i].name, GUILayout.Width(200));
                    EditorGUI.EndDisabledGroup();

                    // # of Assets in Group [i]
                    EditorGUILayout.LabelField(GYA.gyaData.Groups[i].Assets.Count.ToString(), GUILayout.Width(64));
                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(320));

            EditorGUILayout.HelpBox(
                "Accepted Characters: a-zA-Z0-9 ()-+_\n\nRemoving a Group does NOT delete any assets.",
                MessageType.Info);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox(
                "This allows you to organize your unityPackages into Groups as needed.\n\n" +
                "You can also use a Group as a Project Group if desired, so that you know what packages and versions (see 'Version Locking' below) are used in a given project.",
                MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox(
                "'Version Locking' allows you to lock a specific version of an 'Asset Store' package to a group if required. (Example: When you know that a project requires a certain version of a specific asset.)\n\n" +
                "Enable/Disable 'Version Locking' for a package within a Group by:\n" +
                "1) Right-Clicking on a package in the list and\n" + "2) Selecting 'Lock Version for this Group'\n\n" +
                "When viewing a Group, Right-Clicking on an asset and selecting 'Import' will either:\n" +
                "1) Import the latest version available OR\n" +
                "2) If it is 'Version Locked', it will import the assigned version.", MessageType.None);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PrefixLabel("");
        }

        // Old Assets

        //void ShowOldAssets()
        //{
        //    // Old Assets in Old Assets Folder
        //    int fCount = GYAPackage.CountAssetsInFolder(GYA.gyaVars.pathOldAssetsFolder);

        //    // Calc's to help in column positioning
        //    float winWidth = Screen.width;
        //    float wCol1 = 130;
        //    float wGap = 10;
        //    float wCol2 = winWidth - wCol1 - wGap;

        //    //EditorGUILayout.PrefixLabel("");
        //    EditorGUILayout.BeginVertical();

        //    // -- Consolidate Old Assets

        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.BeginVertical();

        //    EditorGUI.BeginDisabledGroup(!(GYA.gyaVars.FilesCount.oldToMove > 0));
        //    if (GUILayout.Button("Consolidate", GUILayout.Width(wCol1)))
        //    {
        //        GYA.Instance.TBPopUpCallback("MoveOldAssets");
        //    }
        //    EditorGUI.EndDisabledGroup();
        //    EditorGUILayout.LabelField(" ( " + GYA.gyaVars.FilesCount.oldToMove + " ) Old Assets",
        //        GUILayout.Width(120));

        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.BeginVertical(GUILayout.Width(wCol2));

        //    EditorGUILayout.HelpBox(
        //        "This will move any 'Old Assets' left behind by Unity when upgrading assets, to the 'Old Assets' folder." +
        //        "  Consolidated assets can later be backed up or deleted as needed.\n\n" +
        //        "Only the Asset Store folder(s) are processed.  Your assigned 'User folder(s)' are NOT processed.\n\n" +
        //        "MOVE TO:  " + GYA.gyaVars.pathOldAssetsFolder, MessageType.None);

        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.EndHorizontal();

        //    // -- Delete Old Assets

        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.BeginVertical();

        //    EditorGUI.BeginDisabledGroup(!(GYA.gyaVars.FilesCount.old > 0));
        //    if (GUILayout.Button("Delete Consolidated", GUILayout.Width(wCol1)))
        //    {
        //        //GYA.Instance.TBPopUpCallback("OldAssetsDeleteAll");
        //        if (EditorUtility.DisplayDialog(GYA.gyaVars.abbr + " - Delete ALL Consolidated Assets",
        //            "WARNING!!  This will DELETE ALL files currently located within: " +
        //            GYA.gyaVars.pathOldAssetsFolder +
        //            "\n\nPlease backup any Old versions you wish to keep before proceeding." +
        //            "\n\nConfirm: DELETE ALL Consolidated Assets?"
        //            , "Cancel", "DELETE ALL"))
        //        {
        //            // Cancel as default - Do nothing
        //        }
        //        else
        //        {
        //            // Yes, delete all
        //            GYAFile.DeleteAllFilesInOldAssetsFolder();
        //        }
        //    }
        //    EditorGUI.EndDisabledGroup();
        //    EditorGUILayout.LabelField(" ( " + fCount + " ) Old Assets", GUILayout.Width(120));

        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.BeginVertical(GUILayout.Width(wCol2));

        //    EditorGUILayout.HelpBox(
        //        "WARNING: This will DELETE ALL files that are currently located within the 'Old Assets' Folder.\n\n" +
        //        "Backup any 'Old Versions' you wish to keep before proceeding.\n\n" + "DELETE FROM:  " +
        //        GYA.gyaVars.pathOldAssetsFolder, MessageType.None);

        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.EndHorizontal();

        //    // -- End Sections

        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.PrefixLabel("");
        //}


        // Maintenance

        void ShowMaintenance()
        {
            //EditorGUILayout.PrefixLabel("");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            // User File
            EditorGUILayout.LabelField("User File");
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("  ", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Backup", GUILayout.ExpandWidth(false)))
            {
                GYA.Instance.TBPopUpCallback("BackupUserFile");
            }
            if (GUILayout.Button("Restore", GUILayout.ExpandWidth(false)))
            {
                GYA.Instance.TBPopUpCallback("RestoreUserFile");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // Export Asset Data

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.LabelField("Export Asset Data");
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("");
            if (GUILayout.Button("as CSV", GUILayout.ExpandWidth(false)))
            {
                GYA.Instance.TBPopUpCallback("ExportAsCSV");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // Offline Mode

            EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Offline Mode");
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GYA.gyaVars.Prefs.enableOfflineMode = EditorGUILayout.ToggleLeft("Enable",
                GYA.gyaVars.Prefs.enableOfflineMode, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // Save Alt

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("");
            if (GUILayout.Button("Save Alternate GYA Data File", GUILayout.ExpandWidth(false)))
            {
                //string fileName = GYA.gyaVars.Files.Assets.fileName;
                string path = EditorUtility.SaveFilePanel(GYA.gyaVars.abbr + " - Save GYA Data File To:", "",
                    GYA.gyaVars.Files.Assets.fileName, "json");
                //string path = EditorUtility.SaveFolderPanel(GYA.gyaVars.abbr + " - Save GYA Data File To:", "", GYA.gyaVars.Files.Assets.fileName);
                //string path = EditorUtility.SaveFilePanelInProject(GYA.gyaVars.abbr + " - Save GYA Data File To:", GYA.gyaVars.Files.Assets.fileName, "json", "", "");

                if (path.Length != 0)
                {
                    File.Copy(GYA.gyaVars.Files.Assets.file, path);
                }
            }
            EditorGUILayout.EndHorizontal();

            // Load Alt

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("");
            if (GUILayout.Button("Load Alternate GYA Data File", GUILayout.ExpandWidth(false)))
            {
                //GYA.Instance.TBPopUpCallback("LoadAltDataFile");
                string path = EditorUtility.OpenFilePanel(GYA.gyaVars.abbr + " Select User Assets Folder:", "", "json");
                //string path = EditorUtility.OpenFilePanelWithFilters(GYA.gyaVars.abbr + " Select User Assets Folder:", "", GYA.gyaVars.Files.Assets.fileName);

                if (path.Length != 0)
                {
                    // Auto Enable Offline Mode since we are loading a data file
                    GYA.gyaVars.Prefs.enableOfflineMode = true;
                    GYAFile.SaveGYAPrefs();

                    File.SetAttributes(GYA.gyaVars.Files.Assets.file, FileAttributes.Normal);
                    File.Copy(path, @GYA.gyaVars.Files.Assets.file, true);
                    GYAFile.LoadGYAAssets();
                    GYAPackage.RefreshProject();
                    GYA.Instance.RefreshSV();
                }
                GUIUtility.ExitGUI(); // Prevent OpenFilePanel from causing a NullReferenceException after loading 
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // Asset Store Specific

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.LabelField("Asset Store Specific");
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal(); // nullref
            EditorGUILayout.PrefixLabel("");
            if (GUILayout.Button("Clean 'Asset Store' Folder", GUILayout.ExpandWidth(false)))
            {
                GYA.Instance.TBPopUpCallback("DeleteEmptySubFolders");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;

            // GYA Specific

            //EditorGUILayout.PrefixLabel("");
            ////EditorGUILayout.LabelField("");
            //EditorGUILayout.LabelField("GYA Specific");
            //EditorGUI.indentLevel++;

            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.PrefixLabel("");
            //if (GUILayout.Button("Clean up Outdated GYA Support Files", GUILayout.ExpandWidth(false)))
            //{
            //    GYA.Instance.TBPopUpCallback("CleanOutdatedGYASupportFiles");
            //}
            //EditorGUILayout.EndHorizontal();
            //EditorGUI.indentLevel--;


            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(400));


            EditorGUILayout.HelpBox(
                "Backup & Restore - Back or Restore the GYA Data files (Prefs & Groups) by making a copy of the files in GYA's Data Folder.",
                MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox(
                "Export Asset Data - Export the Asset List as a CSV data file for use in other programs.",
                MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox(
                "Offline Mode - This allows you to copy your '" + GYA.gyaVars.Files.Assets.fileName +
                "' over to another system that may not have your assets locally.  This way you can still browse your collection.\n\nNote: Importing and File functions will not be active in Offline Mode.",
                MessageType.None);

            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.PrefixLabel("");
            EditorGUILayout.HelpBox("Clean Asset Store Folder - Delete any Empty folders in the Asset Store folder.",
                MessageType.None);

            //EditorGUILayout.PrefixLabel("");
            //EditorGUILayout.HelpBox(
                //"Clean Up Outdated GYA Support Files - Delete any GYA specific files from Older versions of GYA that may be left behind.\n\nThis can happen, for example, when you open another project that is running an older Major version of GYA.  The older version will see that its data files are missing, and automatically re-create new ones.\n\nOnce all projects have been updated, select this to clean up any left over files.",
                //MessageType.None);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PrefixLabel("");
        }

        // Status

        void ShowStatus()
        {
            GUIStyle infoStyle = GUI.skin.GetStyle("HelpBox");
            infoStyle.richText = true;
            //EditorGUILayout.PrefixLabel("");

            var kharmaToShow = "";

            //var kharmaSessionID = GYAFile.GetDataFromUnityCookies("kharma_session");
            if (kharmaSessionID.Length == 0)
                kharmaSessionID = GYAFile.GetFromUnityCookies_KharmaSession();

            //kharmaToShow += "'Purchased Assets List' (PAL) Info:";
            kharmaToShow += "The 'Purchased Assets List' (PAL) ";

            if (kharmaSessionID.Length > 0)
            {
                kharmaToShow += "should be accessible ..";
            }
            else
            {
                kharmaToShow += "is NOT currently accessible ..";
            }

            kharmaToShow += "\n\nSession Detected:\t\t" + (kharmaSessionID.Length > 0);

#if TEST_PAL
            kharmaToShow += "\nCookie File Exists:\t\t" + File.Exists(GYAExt.PathUnityCookiesFile);
            kharmaToShow += "\nCookie File Path:\t\t" + GYAExt.PathUnityCookiesFile;
            kharmaToShow += "\nKharma SessionID:\t\t" + kharmaSessionID;
            //kharmaToShow += "\nKharma SessionID2:\t" + GYAFile.GetDataFromUnityCookies2("kharma_session");
#endif

            kharmaToShow +=
                "\n\nIf there is a problem retrieving the SessionID or PAL, please verify that Unity is logged into the Asset Store.";

            EditorGUILayout.BeginVertical(GUILayout.Width(680));
            EditorGUILayout.TextArea(kharmaToShow, infoStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(200));

            Dictionary<string, string> infoToShowAll = new Dictionary<string, string>
            {
                {"gyaVars.version:", GYA.gyaVars.version},
                {"Application.unityVersion:", Application.unityVersion},
                {"SystemInfo.operatingSystem:", SystemInfo.operatingSystem},
                {"GYAExt.PathUnityApp:", Path.GetFullPath(GYAExt.PathUnityApp)},
                {"GYAExt.PathUnityStandardAssets:", GYAExt.PathUnityStandardAssets},
                {"GYAExt.PathUnityProject:", Path.GetFullPath(GYAExt.PathUnityProject)},
                {"GYAExt.PathUnityProjectAssets:", GYAExt.PathUnityProjectAssets},
                {"GYAExt.PathGYADataFiles:", GYAExt.PathGYADataFiles},
                {"gyaVars.pathOldAssetsFolder:", GYA.gyaVars.pathOldAssetsFolder},
                {"gyaVars.Files.Prefs.file:", GYA.gyaVars.Files.Prefs.file},
                {"gyaVars.Files.Groups.file:", GYA.gyaVars.Files.Groups.file},
                {"gyaVars.Files.Assets.file:", GYA.gyaVars.Files.Assets.file},
                {"gyaVars.Files.ASPackage.file:", GYA.gyaVars.Files.ASPackage.file},
                {"gyaVars.Files.ASPurchase.file:", GYA.gyaVars.Files.ASPurchase.file},
                {"GYAExt.PathUnityDataFiles:", GYAExt.PathUnityDataFiles},
                {"GYAExt.PathUnityAssetStoreActive:", GYAExt.PathUnityAssetStoreActive}
            };


            var infoToShowKey = "";
            var infoToShowValue = "";
            foreach (var line in infoToShowAll)
            {
                infoToShowKey += line.Key + "\n";
                infoToShowValue += line.Value + "\n";

                // Add an extra Return for display
                if (line.Key == "SystemInfo.operatingSystem:" ||
                    line.Key == "GYAExt.PathUnityProjectAssets:" ||
                    line.Key == "gyaVars.Files.ASPurchase.file:"
                )
                {
                    infoToShowKey += "\n";
                    infoToShowValue += "\n";
                }
            }

            EditorGUILayout.TextArea(infoToShowKey, infoStyle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.Width(480));

            EditorGUILayout.TextArea(infoToShowValue, infoStyle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            // Clipboard
            if (GUILayout.Button("Copy To Clipboard"))
            {
                GYAFile.CopyToClipboard(GYAExt.ToJson(infoToShowAll, true), true);
                //GYAFile.CopyToClipboard( kharmaToShow + "\n\n" + GYAExt.ToJson(infoToShowAll, true), true );
            }

            EditorGUILayout.PrefixLabel("");
        }
    }
}