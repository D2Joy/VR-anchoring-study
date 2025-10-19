using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public GameObject platform;
    //public Transform plane;
	Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
    	offset= transform.position-platform.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position= platform.transform.position + offset;
        //transform.LookAt(plane);
    }
}
