using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEnder : MonoBehaviour
{
    private string curScene;
    public GameObject Player;
    public Rigidbody2D playerRB;
    public BoxCollider2D playerCOLL;
    public Transform playerTRANS;
    private string toScene;
    private bool isOut;
    // Start is called before the first frame update
    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player != null)
        {
            playerTRANS = Player.GetComponent<Transform>();
            playerCOLL = Player.GetComponent<BoxCollider2D>();
            playerRB = Player.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag  == "Player")
        {
            if(playerTRANS.position.y < transform.position.y)
            {
                PlayerPrefs.SetString("PlayerDirection", "out");
                isOut = true;
            }
            else
            {
                PlayerPrefs.SetString("PlayerDirection", "in");
                isOut = false;
            }
            
            PlayerPrefs.SetFloat("PlayerVelocityX", playerRB.velocity.x);
            PlayerPrefs.SetFloat("PlayerValocityY", playerRB.velocity.y);
            PlayerPrefs.SetFloat("PlayerPositionX", playerTRANS.position.x);
            PlayerPrefs.SetFloat("PlayerJuice", Player.GetComponent<MainPlayerMovement>().juiceAmount);
            Time.timeScale = 0;
            curScene = SceneManager.GetActiveScene().name;
            // stageX
            // Debug.Log(curScene.Substring(0, 4));
            // Debug.LogWarning(System.Convert.ToInt32(curScene.Substring(5))+1);
            if(isOut)
            {
                Debug.Log(curScene.Substring(0, 5) + (System.Convert.ToInt32(curScene.Substring(5))+1));
                LoadingUILogic.instance.addScenesToLaod(curScene.Substring(0, 5) + (System.Convert.ToInt32(curScene.Substring(5))+1));
            }
            else
            {
                LoadingUILogic.instance.addScenesToLaod(curScene.Substring(0, 5) + (System.Convert.ToInt32(curScene.Substring(5))-1));
            }
            LoadingUILogic.instance.loadScenes();
            Time.timeScale = 1;
        }
    }
}
