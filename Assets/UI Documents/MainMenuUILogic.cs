using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuUILogic : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update() {
        
    }

    void newGameButtonPressed() 
    {
        LoadingUILogic.instance.addScenesToLaod("stage1");
        LoadingUILogic.instance.loadScenes();
        Debug.Log("newGameButtonPressed");
    }

    void continueButtonPressed() 
    {
        Debug.Log("continueButtonPressed");
    }

    void historyButtonPressed() 
    {
        Debug.Log("historyButtonPressed");
    }

    void settingsButtonPressed() 
    {
        SettingsUILogic.instance.uiDocument.enabled = !SettingsUILogic.instance.uiDocument.enabled;
        Debug.Log("settingsButtonPressed");
    }

    void quitButtonPressed() 
    {
        Debug.Log("quitButtonPressed");
        Application.Quit();
    }
}
