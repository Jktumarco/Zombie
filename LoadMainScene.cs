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
    [SerializeField] Animator loagingAnim;

    public void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        loagingAnim.Play("loaging");
        while (asyncLoad.isDone)
        {
            loagingAnim.Play("loaging");
            yield return null;
        }
    }
}
