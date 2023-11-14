using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadingUILogic : MonoBehaviour
{
    public static LoadingUILogic instance;

    private UIDocument uiDocument;
    private ProgressBar progressBar;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        progressBar = root.Q<ProgressBar>("loadingProgressBar");

        uiDocument.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void addScenesToLaod(string sceneName)
    {
        scenesToLoad.Add(SceneManager.LoadSceneAsync(sceneName));
    }

    public void loadScenes()
    {
        uiDocument.enabled = true;
        StartCoroutine(loadingProgressBar());
        IEnumerator loadingProgressBar()
        {
            float totalProgress = 0;
            for (int i = 0; i < scenesToLoad.Count; i++)
            {
                while (!scenesToLoad[i].isDone)
                {
                    totalProgress += scenesToLoad[i].progress;
                    progressBar.value = totalProgress / scenesToLoad.Count;
                    Debug.Log("Load Scene Progress: " + progressBar.value);
                    yield return null;
                }
            }
        }
        uiDocument.enabled = false;
    }
}
