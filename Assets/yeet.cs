using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class yeet : MonoBehaviour
{
    public Button a;
    // Start is called before the first frame update
    void Start()
    {
        a.onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void TaskOnClick()
    {
        Application.Quit();
    }
}
