using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuUILogic : MonoBehaviour
{
    private bool inGame;

    private UIDocument uiDocument;
    private Button newGameButton;
    private Button continueButton;
    private Button historyButton;
    private Button settingsButton;
    private Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        newGameButton = root.Q<Button>("newGameButton");
        continueButton = root.Q<Button>("continueButton");
        historyButton = root.Q<Button>("historyButton");
        settingsButton = root.Q<Button>("settingsButton");
        quitButton = root.Q<Button>("quitButton");
        
        newGameButton.clicked += newGameButtonPressed;
        continueButton.clicked += continueButtonPressed;
        historyButton.clicked += historyButtonPressed;
        settingsButton.clicked += settingsButtonPressed;
        quitButton.clicked += quitButtonPressed;

        inGame = PlayerPrefs.GetInt("inGame", 0) == 0 ? false : true;
    }

    // Update is called once per frame
    void Update() {
        
    }

    void newGameButtonPressed() 
    {
        Debug.Log("newGameButtonPressed");
        if (!inGame)
        {
            inGame = true;
            PlayerPrefs.SetInt("inGame", 1);
            LoadingUILogic.instance.addScenesToLaod("Game"); //scene 1
            LoadingUILogic.instance.loadScenes();
            HistoryUILogic.instance.createStopwatch();
            HistoryUILogic.instance.plusTotalTime();
            HistoryUILogic.instance.stopwatch.start();
        }
        else
        {
            LoadingUILogic.instance.addScenesToLaod("Game"); //scene 1
            LoadingUILogic.instance.loadScenes();
            HistoryUILogic.instance.resetSavedStopwatchTime();
            HistoryUILogic.instance.createStopwatch();
            HistoryUILogic.instance.plusTotalTime();
            HistoryUILogic.instance.stopwatch.start();
        }
        
    }

    void continueButtonPressed() 
    {
        Debug.Log("continueButtonPressed");
        if (inGame)
        {
            LoadingUILogic.instance.addScenesToLaod("Game"); //playerPrefs scene
            LoadingUILogic.instance.loadScenes();
            HistoryUILogic.instance.createStopwatch();
            HistoryUILogic.instance.plusTotalTime();
            HistoryUILogic.instance.stopwatch.start();
        }
        else
        {
            //do nothing
        }
        
    }

    void historyButtonPressed() 
    {
        Debug.Log("historyButtonPressed");
        HistoryUILogic.instance.uiDocument.enabled = true;   
    }

    void settingsButtonPressed() 
    {
        Debug.Log("settingsButtonPressed");
        SettingsUILogic.instance.uiDocument.enabled = true;
    }

    void quitButtonPressed() 
    {
        Debug.Log("quitButtonPressed");
    }
}
