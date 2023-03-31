
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] Scene scene;
    private AssetBundle myLoadedAssetBundle;
    [SerializeField] private int sceneName;

    // Use this for initialization
    void Start()
    {
        //myLoadedAssetBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/_Scenes");
        //scenePaths = myLoadedAssetBundle.GetAllScenePaths();
    }

    public void LoadPlayScene()
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}