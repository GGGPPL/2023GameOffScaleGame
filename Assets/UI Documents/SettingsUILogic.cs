using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class SettingsUILogic : MonoBehaviour
{
    public static SettingsUILogic instance;

    private bool eventsSubscribed;

    public UIDocument uiDocument;
    private VisualElement overlay;
    private Button closeButton;

    private TextField mainVolumeTextField;
    private Slider mainVolumeSlider;
    public int mainVolumeValue; //SettingsUILogic.instace.mainVolumeValue

    private Button leftKeyButton;
    private Button rightKeyButton;
    private Button jumpKeyButton;
    private Button interactKeyButton;
    private Event keyEvent;
    public KeyCode leftKey {get; set;}
    public KeyCode rightKey {get; set;}
    public KeyCode jumpKey {get; set;}
    public KeyCode interactKey {get; set;}
    private KeyCode newKey;
    private string buttonText;
    private bool waitingForKey;

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

        instance = this;

        getPlyerPrefs();
    }

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        eventsSubscribed = false;
        uiDocument.enabled = false;
        waitingForKey = false;
    }

    // Update is called once per frame
    void Update() //important - on toggle pause write on the esc inside game
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            uiDocument.enabled = false;
        if (uiDocument.enabled && !eventsSubscribed)
            subscribeToEvents();
        else if (!uiDocument.enabled && eventsSubscribed)
            unSubscribeFromEvents();
    }

    void getPlyerPrefs()
    {
        mainVolumeValue = PlayerPrefs.GetInt("mainVolumeValue", 100);
        leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftkey", "A"));
        rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightkey", "D"));
        jumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpkey", "Space"));
        interactKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interactkey", "E"));
    }

    void setPlayerPrefsValue()
    {
        mainVolumeTextField.value = mainVolumeValue.ToString();
        mainVolumeSlider.value = mainVolumeValue;
        leftKeyButton.text = leftKey.ToString();
        rightKeyButton.text = rightKey.ToString();
        jumpKeyButton.text = jumpKey.ToString();
        interactKeyButton.text = interactKey.ToString();
    }

    void subscribeToEvents()
    {
        Debug.Log("EventsSubscribed");
        eventsSubscribed = true;

        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        closeButton = root.Q<Button>("closeButton");
        mainVolumeTextField = root.Q<TextField>("mainVolumeTextField");
        mainVolumeSlider = root.Q<Slider>("mainVolumeSlider");
        leftKeyButton = root.Q<Button>("leftKeyButton");
        rightKeyButton = root.Q<Button>("rightKeyButton");
        jumpKeyButton = root.Q<Button>("jumpKeyButton");
        interactKeyButton = root.Q<Button>("interactKeyButton");

        mainVolumeTextField.RegisterValueChangedCallback(onMainVolumeTextFieldValueChanged);
        mainVolumeSlider.RegisterValueChangedCallback(onMainVolumeSliderValueChanged);
        closeButton.clicked += closeButtonPressed;
        leftKeyButton.clicked += leftkeyButtonPressed;
        rightKeyButton.clicked += rightkeyButtonPressed;
        jumpKeyButton.clicked += jumpKeyButtonPressed;
        interactKeyButton.clicked += interactKeyButtonPressed;

        setPlayerPrefsValue();
        createInterceptOverlay();
    }

    void unSubscribeFromEvents()
    {
        Debug.Log("EventsUnSubscribed");
        eventsSubscribed = false;

        mainVolumeTextField.UnregisterValueChangedCallback(onMainVolumeTextFieldValueChanged);
        mainVolumeSlider.UnregisterValueChangedCallback(onMainVolumeSliderValueChanged);
        closeButton.clicked -= closeButtonPressed;
        leftKeyButton.clicked -= leftkeyButtonPressed;
        rightKeyButton.clicked -= rightkeyButtonPressed;
        jumpKeyButton.clicked -= jumpKeyButtonPressed;
        interactKeyButton.clicked -= interactKeyButtonPressed;
    }

    void closeButtonPressed()
    {
        Debug.Log("closeButtonPressed");
        uiDocument.enabled = false;
    }

    void onMainVolumeTextFieldValueChanged(ChangeEvent<string> evt)
    {
        string input = evt.newValue;
        if (int.TryParse(input, out int result))
        {
            mainVolumeValue = int.Parse(input);
            syncMainVolume();
        }
    }
    void onMainVolumeSliderValueChanged(ChangeEvent<float> evt)
    {
        mainVolumeValue = Mathf.FloorToInt(evt.newValue);
        syncMainVolume();
    }

    void syncMainVolume()
    {
        mainVolumeTextField.value = mainVolumeValue.ToString();
        mainVolumeSlider.value = mainVolumeValue;
        PlayerPrefs.SetInt("mainVolumeValue", mainVolumeValue);
    }

    void leftkeyButtonPressed()
    {
        Debug.Log("leftkeyButtonPressed");
        leftKeyButton.text = "< input >";
        startAssignment("left");
    }

    void rightkeyButtonPressed()
    {
        Debug.Log("rightkeyButtonPressed");
        rightKeyButton.text = "< input >";
        startAssignment("right");
    }

    void jumpKeyButtonPressed()
    {
        Debug.Log("jumpKeyButtonPressed");
        jumpKeyButton.text = "< input >";
        startAssignment("jump");
    }

    void interactKeyButtonPressed()
    {
        Debug.Log("interactKeyButtonPressed");
        interactKeyButton.text = "< input >";
        startAssignment("interact");
    }

    void createInterceptOverlay()
    {
        overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.top = 0;
        overlay.style.bottom = 0;
        overlay.style.left = 0;
        overlay.style.right = 0;

        overlay.RegisterCallback<MouseDownEvent>(e => e.StopPropagation());
        overlay.RegisterCallback<MouseUpEvent>(e => e.StopPropagation());
        overlay.RegisterCallback<ClickEvent>(e => e.StopPropagation());
    }

    void enableInterceptOverlay()
    {
        Debug.Log("interceptMouseEventOn");
        uiDocument.rootVisualElement.Add(overlay);
    }

    void disableInterceptOverlay()
    {
        Debug.Log("interceptMouseEventOff");
        overlay.RemoveFromHierarchy();
    }

    void OnGUI()
    {
        keyEvent = Event.current;
        if (keyEvent.isKey && waitingForKey) 
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
        }
    }

    void startAssignment(string keyName)
    {
        enableInterceptOverlay();
        if (!waitingForKey)
            StartCoroutine(assignKey(keyName));
    }
    
    IEnumerator waitForKey()
    {
        while(!keyEvent.isKey)
            yield return null;
    }
    IEnumerator assignKey(string keyName)
    {
        waitingForKey = true;

        yield return waitForKey();

        buttonText = newKey.ToString();
        if (buttonText != "Escape")
        {
            switch (keyName)
            {
                case "left":
                    leftKey = newKey;
                    leftKeyButton.text = buttonText;
                    PlayerPrefs.SetString("leftkey", buttonText);
                    break;
                case "right":
                    rightKey = newKey;
                    rightKeyButton.text = buttonText;
                    PlayerPrefs.SetString("rightkey", buttonText);
                    break;
                case "jump":
                    jumpKey = newKey;
                    jumpKeyButton.text = buttonText;
                    PlayerPrefs.SetString("jumpkey", buttonText);
                    break;
                case "interact":
                    interactKey = newKey;
                    interactKeyButton.text = buttonText;
                    PlayerPrefs.SetString("leftkey", buttonText);
                    break;
            }
        }
        
        disableInterceptOverlay();
        yield return null;
    }
}
