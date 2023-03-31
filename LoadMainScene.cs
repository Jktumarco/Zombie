using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadMainScene : MonoBehaviour
{

    public Slider loadingBar;
    public Text textLoading;

    public void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress / 0.9f;
            loadingBar.value = progress;
            textLoading.text = string.Format("{0:0}%", progress*100);
            yield return null;
        }
    }
}
