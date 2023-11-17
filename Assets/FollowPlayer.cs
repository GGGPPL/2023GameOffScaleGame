using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform targetTRANS;
    public GameObject target;
    public float permXPos;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        targetTRANS = target.transform;
        permXPos = transform.position.x;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(permXPos, targetTRANS.position.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
    }
}
