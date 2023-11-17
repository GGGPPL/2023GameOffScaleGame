using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class juice : MonoBehaviour
{
    public float timer;
    public GameObject[] splashList;
    public int splashCount;
    // Start is called before the first frame update
    void Start()
    {
        splashList = GameObject.FindGameObjectsWithTag("Splashed juice");
        foreach(GameObject splash in splashList)
        {
            splashCount++;
        }
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 60 && splashCount > 1)
        {
            Destroy(gameObject);
        }
    }
}
