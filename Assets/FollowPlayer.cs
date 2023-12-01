using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FollowPlayer : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform targetTRANS;
    public Transform endTriggerTRANS;
    //public Collider2D endTriggerCOLL;
    public GameObject target;
    public Camera camera;
    public float permXPos;
    private float cameraTop;
    public float lowestPosInScene;
    
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        endTriggerTRANS = GameObject.FindGameObjectWithTag("END").GetComponent<Transform>();
        //endTriggerCOLL = GameObject.FindGameObjectWithTag("END").GetComponent<Collider2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        targetTRANS = target.transform;
        permXPos = transform.position.x;

            lowestPosInScene = 0.45f;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(permXPos, targetTRANS.position.y + yOffset, -10f);
        cameraTop = transform.position.y + camera.orthographicSize;
        if(cameraTop < endTriggerTRANS.position.y)
        {
            newPos.y = Mathf.Max(lowestPosInScene, newPos.y);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);    
        }
        else
        {
            newPos.y = Mathf.Max(lowestPosInScene, newPos.y);
            newPos = new Vector3(permXPos, endTriggerTRANS.position.y - camera.orthographicSize, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
        
    }
}

