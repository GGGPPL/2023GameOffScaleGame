using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnder : MonoBehaviour
{
    private string curScene;
    private string toScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag  == "Player")
        {
            Time.timeScale = 0;
            curScene = SceneManager.GetActiveScene().name;
            // stageX
            // Debug.Log(curScene.Substring(0, 4));
            // Debug.LogWarning(System.Convert.ToInt32(curScene.Substring(5))+1);
            
            LoadingUILogic.instance.addScenesToLaod(curScene.Substring(0, 5) + (System.Convert.ToInt32(curScene.Substring(5))+1));
            LoadingUILogic.instance.loadScenes();
            Time.timeScale = 1;
        }
    }
}
