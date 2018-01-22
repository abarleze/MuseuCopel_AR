using System.Collections.Generic;
using System.IO;
using UnityEditor;

/*
Directory creation script for quick creation of directories in a new Unity3D or 2D project...
Use :
1. Create a new C# script and save as BuildProjectFolders, put in a flder for future use
2. Create a folder in your project called Editor, import or drag script into it
3. Click Edit -> Create Project Folders * as long as there are no build errors, you will see a new menu item near the bootom of the Edit menu
4.  If you want to include a Resources folder, clicking the checkbox will add or remove it
5. If you are using Namespaces, clicking the checkbox will include three basic namesapce folders
6.  Right clicking on a list item will let you delete the item, if you want
7.  Increasing the List size will add another item with the prior items name, click in the space to rename.
8.  Clicking "Create" will create all the files listed, the Namespace folders will be added to the script directory.
*/

public class BuildProjectFolders : ScriptableWizard
{
    public bool IncludeResourceFolder = false;
    public bool UseNamespace = false;
    private string SFGUID;
    public List<string> nsFolders = new List<string>();
    public List<string> folders = new List<string>()
    {
        "_Project/_Oction",
        "_Project/Graphics",
           "_Project/Graphics/Characters",
            "_Project/Graphics/Characters/_ItemName/Animations",
            "_Project/Graphics/Characters/_ItemName/Meshes",
            "_Project/Graphics/Characters/_ItemName/Materials",
            "_Project/Graphics/Characters/_ItemName/Prefabs",
            "_Project/Graphics/Characters/_ItemName/Sprites",
            "_Project/Graphics/Characters/_ItemName/Textures",
            "_Project/Graphics/InteractiveItens",
            "_Project/Graphics/InteractiveItens/_ItemName",
            "_Project/Graphics/InteractiveItens/_ItemName/Animations",
            "_Project/Graphics/InteractiveItens/_ItemName/Meshes",
            "_Project/Graphics/InteractiveItens/_ItemName/Materials",
            "_Project/Graphics/InteractiveItens/_ItemName/Prefabs",
            "_Project/Graphics/InteractiveItens/_ItemName/Sprites",
            "_Project/Graphics/InteractiveItens/_ItemName/Textures",
            "_Project/Graphics/StaticItens",
            "_Project/Graphics/StaticItens/_ItemName/Animations",
            "_Project/Graphics/StaticItens/_ItemName/Meshes",
            "_Project/Graphics/StaticItens/_ItemName/Materials",
            "_Project/Graphics/StaticItens/_ItemName/Prefabs",
             "_Project/Graphics/StaticItens/_ItemName/Sprites",
            "_Project/Graphics/StaticItens/_ItemName/Textures",
            "_Project/Graphics/UserInterface",
            "_Project/Graphics/UserInterface/Fonts",
            "_Project/Graphics/StaticItens/_ItemName/Materials",
            "_Project/Graphics/UserInterface/_ItemName/Prefabs",
            "_Project/Graphics/UserInterface/_ItemName/Sprites",
            "_Project/Graphics/UserInterface/_ItemName/Textures",
            "_Project/Graphics/Packages",
        "_Project/Particles",
        "_Project/Prefabs",
        "_Project/Scripts",
        "_Project/Scenes",
        "_Project/Sounds",
        "_Project/Videos"
    };

    [MenuItem("Oction / Create Project Folders...")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard("Create Project Folders", typeof(BuildProjectFolders), "Create");
    }

    //Called when the window first appears
    void OnEnable()
    {

    }
    //Create button click
    void OnWizardCreate()
    {
        //create all the folders required in a project
        //primary and sub folders
        //foreach (string folder in folders)
        //{
        //    string guid = AssetDatabase.CreateFolder("Assets", folder);
        //    string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
        //    if (folder == "Scripts")
        //        SFGUID = newFolderPath;
        //}

        foreach (string folderTest in folders)
        {
            var folder = Directory.CreateDirectory("Assets/" + folderTest);
        }

        AssetDatabase.Refresh();
        if (UseNamespace == true)
        {
            foreach (string nsf in nsFolders)
            {
                //AssetDatabase.Contain
                string guid = AssetDatabase.CreateFolder("Assets/Scripts", nsf);
                string newFolderPath = AssetDatabase.GUIDToAssetPath(guid);

            }
        }
    }
    //Runs whenever something changes in the editor window...
    void OnWizardUpdate()
    {
        if (IncludeResourceFolder == true && !folders.Contains("Resources"))
            folders.Add("Resources");
        if (IncludeResourceFolder == false && folders.Contains("Resources"))
            folders.Remove("Resources");

        if (UseNamespace == true)
            addNamespaceFolders();
        if (UseNamespace == false)
            removeNamespceFolders();

    }
    void addNamespaceFolders()
    {


        if (!nsFolders.Contains("Interfaces"))
            nsFolders.Add("Interfaces");

        if (!nsFolders.Contains("Classes"))
            nsFolders.Add("Classes");


        if (!nsFolders.Contains("States"))
            nsFolders.Add("States");

        // (nsFolders);
    }

    void removeNamespceFolders()
    {
        if (nsFolders.Count > 0) nsFolders.Clear();
    }
}
