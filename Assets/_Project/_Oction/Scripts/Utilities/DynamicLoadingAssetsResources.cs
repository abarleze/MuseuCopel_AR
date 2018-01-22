using UnityEngine;
using System.Collections;

public class DynamicLoadingAssetsResource : MonoBehaviour
{

    // Internal reference to Renderer Component
    private MeshRenderer ThisRenderer = null;

    // Maintain an internal reference to resource object 
    private Object MyAsset = null;

    // Called at object start 
    void Start()
    {
        // Get Mesh Renderer component on this object 
        ThisRenderer = GetComponent<MeshRenderer>();

        // Load resource 
        MyAsset = Resources.Load(" MyTextureFile", typeof(Texture2D));

        // Assign to material 
        ThisRenderer.material.mainTexture = MyAsset as Texture2D;
    }

    // Called when object is destroyed 
    void OnDestroy()
    {
        // Unload asset
        Resources.UnloadAsset(MyAsset);
    }
}
