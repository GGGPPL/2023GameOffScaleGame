using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXControl : MonoBehaviour
{
    public AudioSource BGM;

    public float volume;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Update() 
    {
        volume = SettingsUILogic.instance.mainVolumeValue;
        BGM.volume = volume;
    }
}
