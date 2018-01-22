using System.Collections;
using UnityEngine;

public class LoadSceneOverlay : MonoBehaviour
{
    private static LoadSceneOverlay _instance;

    public CanvasGroup group;
    public float inOutTime = 0.5f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
            DestroyImmediate(this.gameObject);
    }

    private void OnEnable()
    {
        group.alpha = 0.0f;
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1.0f));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(0.0f));
    }

    private IEnumerator Fade(float value)
    {
        group.blocksRaycasts = true;
        group.alpha = 1.0f - value;

        while (group.alpha != value)
        {
            yield return null;
            group.alpha = Mathf.MoveTowards(group.alpha, value, Time.deltaTime / inOutTime);
        }

        if (group.alpha < 1.0f)
            group.blocksRaycasts = false;
    }
}