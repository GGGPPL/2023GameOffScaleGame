using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PausedUILogic : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button closeButton;
    private Button settingsButton;
    private Button backToMainMenuButton;

    private bool eventsSubscribed = false;

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        uiDocument.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            uiDocument.enabled = !uiDocument.enabled;
        if (uiDocument.enabled && !eventsSubscribed)
        {
            subscribeToEvents();
            HistoryUILogic.instance.stopwatch.pause();
            HistoryUILogic.instance.plusTotalTime();
        }
        else if (!uiDocument.enabled && eventsSubscribed)
        {
            unSubscribeFromEvents();
            HistoryUILogic.instance.stopwatch.resume();
            HistoryUILogic.instance.plusTotalTime();
        }
    }

    void subscribeToEvents()
    {
        Debug.Log("EventsSubscribed");
        eventsSubscribed = true;

        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        closeButton = root.Q<Button>("closeButton");
        settingsButton = root.Q<Button>("settingsButton");
        backToMainMenuButton = root.Q<Button>("backToMainMenuButton");

        closeButton.clicked += closeButtonPressed;
        settingsButton.clicked += settingsButtonPressed;
        backToMainMenuButton.clicked += backToMainMenuButtonPressed;
    }

    void unSubscribeFromEvents()
    {
        Debug.Log("EventsUnSubscribed");
        eventsSubscribed = false;

        closeButton.clicked -= closeButtonPressed;
        settingsButton.clicked -= settingsButtonPressed;
        backToMainMenuButton.clicked -= backToMainMenuButtonPressed;
    }

    void closeButtonPressed()
    {
        Debug.Log("closeButtonPressed");
        uiDocument.enabled = false;
    }

    void settingsButtonPressed()
    {
        Debug.Log("settingsButtonPressed");
        SettingsUILogic.instance.uiDocument.enabled = true;
    }

    void backToMainMenuButtonPressed()
    {
        Debug.Log("backToMainMenuButtonPressed");
        HistoryUILogic.instance.stopwatch.reset();
        HistoryUILogic.instance.plusTotalTime();
        HistoryUILogic.instance.saveStopwatchTime();
        LoadingUILogic.instance.addScenesToLaod("Main Menu");
        LoadingUILogic.instance.loadScenes();
        uiDocument.enabled = false;
    }
}
