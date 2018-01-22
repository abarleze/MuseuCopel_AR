using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public float delay = 0.0f;
    public float minimumTransitionTime = 1.0f;
    public GameEvent sceneLoading;
    public GameEvent sceneLoaded;

    private bool _isLoading = false;
    
    public void Load(string sceneName)
    {
        if (_isLoading)
            return;
        
        StartCoroutine(LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        _isLoading = true;
        if (delay > 0.0f)
            yield return new WaitForSeconds(delay);

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        sceneLoading.Raise();
        
        yield return new WaitForSeconds(minimumTransitionTime);

        if (!op.isDone)
            yield return null;

        sceneLoaded.Raise();
        op.allowSceneActivation = true;
        _isLoading = false;
    }
}