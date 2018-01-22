using UnityEditor;

/// <summary>
/// In the Project Panel, select all assets to be included in the bundle,
/// then from the Object Inspector preview window, click the Asset Bundle drop-down 
/// menu and choose New to create a new category that collects together all similarly 
/// tagged assets into one Bundle. Then use the same drop-down menu to pick and assign 
/// the tag to the selected assets in the project panel. In this example, 
/// I’ve used the group MyTextures to collect together some textures into a single Bundle. 
/// To generate the Bundle, then choose Assets > Build Bundle Assets from the application menu, 
/// and a single Bundle file will be generated at the file location specified.
/// </summary>
public class AssetBundleBuilder
{
    // Adds item to menu for exporting labelled asset bundles
    [MenuItem("Oction / Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        // Replace “e:\ bundles” with your own folder 
        //BuildPipeline.BuildAssetBundles(@" i:\ bundles");
    }
}
