using System;
using System.Collections.Generic;
using GYAInternal.Json;

//using UnityEngine;
//using System.Collections;

namespace XeirGYA
{
    public class GYAVars
    {
        public string name { get; set; }
        public string abbr { get; set; }
        public string version { get; set; } // GYA Version that made the Package file
        public int version_id { get; set; }
        public int version_idV2 { get; set; }

        public string pathOldAssetsFolderName { get; set; }

        public string pathOldAssetsFolder { get; set; }

        // Symlink Status AS Folder
        //public bool isSymLinkPathAS { get; set; }
        //public bool isSymLinkPathAS5 { get; set; }
        //public string symlinkTargetAS { get; set; }
        //public string symlinkTargetAS5 { get; set; }

        public GYAPrefs Prefs { get; set; }
        public GYAFiles Files { get; set; }
        public GYAFilesCount FilesCount { get; set; }
        public GYAFilesSize FilesSize { get; set; }

        public GYAVars()
        {
            name = "Grab Yer Assets";
            abbr = "GYA";
            version = String.Empty;
            version_id = 72902;
            version_idV2 = 15398;

            pathOldAssetsFolderName = "Asset Store-Old";
            //isSymLinkPathAS = false;
            //isSymLinkPathAS5 = false;
            //symlinkTargetAS = "";
            //symlinkTargetAS5 = "";

            Prefs = new GYAPrefs();
            Files = new GYAFiles();
            FilesCount = new GYAFilesCount();
            FilesSize = new GYAFilesSize();
        }
    }

    // GYA Settings file
    public class GYAPrefs
    {
        //public bool refreshOnStartup { get; set; }
        //public bool scanAllAssetStoreFolders { get; set; }
        //public bool showAllAssetStoreFolders { get; set; }
        public bool isPersist { get; set; }

        public List<string> pathUserAssets { get; set; }
        public bool enableHeaders { get; set; }
        public bool enableColors { get; set; }

        public bool showSVInfo { get; set; }

        //public bool reportPackageErrors { get; set; }
        public bool nestedVersions { get; set; }

        public bool enableCollectionTypeIcons { get; set; }

        public bool enableAltIconForOldVersions { get; set; }

        //public bool autoPreventASOverwrite { get; set; }
        //public bool autoRenameASFiles { get; set; }
        //public bool autoConsolidate { get; set; }

        //public bool autoDeleteConsolidated { get; set; }
        public bool enableOfflineMode { get; set; }

        public bool openURLInUnity { get; set; }

        //public bool getPackagesInfoFromUnity { get; set; }
        public bool getPurchasedAssetsListDuringRefresh { get; set; }

        //public bool autoBackup { get; set; }
        public GYAImport.MultiImportType multiImportOverride { get; set; }

        public bool enableToolbarCollections { get; set; }
        //public bool enablePackageFolding { get; set; }
        public bool enablePopupDetails { get; set; }
        public bool isSilent { get; set; }
        public int urlsToOpenPerBatch { get; set; }

        [JsonIgnore]
        public string kharmaSession { get; set; }

        public GYAPrefs()
        {
            //refreshOnStartup = false;
            //scanAllAssetStoreFolders = true;
            //showAllAssetStoreFolders = true;
            isPersist = false;
            pathUserAssets = new List<string>();
            enableHeaders = true;
            enableColors = true;
            showSVInfo = true;
            //reportPackageErrors = false;
            nestedVersions = false;
            enableCollectionTypeIcons = true;
            enableAltIconForOldVersions = true;
            //autoPreventASOverwrite = false;
            //autoConsolidate = false;
            //autoDeleteConsolidated = false;
            enableOfflineMode = false;
            openURLInUnity = false;
            //getPackagesInfoFromUnity = false;			 // Unity's internal pkg gatherer (limited to AS folder)
            getPurchasedAssetsListDuringRefresh = false; // Default false, Get purchased list form Unity
            //autoBackup = false;
            multiImportOverride = GYAImport.MultiImportType.Default;
            enableToolbarCollections = true;
            //enablePackageFolding = true; // SV pkg vID folding - NOT USED YET
            enablePopupDetails = false; // Show asset details in right-click popup
            isSilent = false; // Silent Mode - Hide default console msg's
            urlsToOpenPerBatch = 10;

            kharmaSession = String.Empty;
        }
    }

    public class GYAFiles
    {
        public GYAFileInfo Prefs { get; set; }
        public GYAFileInfo Groups { get; set; }
        public GYAFileInfo Assets { get; set; }
        public GYAFileInfo ASPackage { get; set; }
        public GYAFileInfo ASPurchase { get; set; }

        public GYAFiles()
        {
            Prefs = new GYAFileInfo();
            Groups = new GYAFileInfo();
            Assets = new GYAFileInfo();
            ASPackage = new GYAFileInfo();
            ASPurchase = new GYAFileInfo();
        }
    }

    public class GYAFileInfo
    {
        public string fileName { get; set; }
        public string file { get; set; }

        public bool fileExists { get; set; }
        //public bool isLoaded { get; set; }

        public GYAFileInfo()
        {
            fileName = String.Empty;
            file = String.Empty;
            fileExists = false;
            //isLoaded = false;
        }
    }

    // Data Tallys
    public class GYAFilesCount
    {
        public int all { get; set; }
        public int store { get; set; }
        public int user { get; set; }
        public int standard { get; set; }
        public int old { get; set; }
        public int oldToMove { get; set; }
        public int project { get; set; }
        // TODO: Remove following entries and auto-calc on the fly
        //public int u2017verPackages { get; set; }
        //public int u2017verFolder { get; set; }
        //public int u5verPackages { get; set; }
        //public int u5verFolder { get; set; }
        //public int u4verPackages { get; set; }
        //public int u4verFolder { get; set; }
        //public int u3verPackages { get; set; }
        //public int u3verFolder { get; set; }
        //public int uUverPackages { get; set; }
        //public int uUverFolder { get; set; }
    }

    public class GYAFilesSize
    {
        public double all { get; set; }
        public double store { get; set; }
        public double user { get; set; }
        public double standard { get; set; }
        public double old { get; set; }
        public double oldToMove { get; set; }
        public double project { get; set; }
    }
}