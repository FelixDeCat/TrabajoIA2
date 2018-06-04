using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControllerCamera : MonoBehaviour,IUpdateble
{
    public Vector3 offset;
    Camera myCamera;
    public GameObject target;
    public float timeLerp = 0.2f;
    public float range;


    float speed = 5;
    float posY;

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
        posY = transform.position.y;
    }
    void FixedUpdate ()
    {

        var rotateVector3 = myCamera.transform.rotation.eulerAngles;
        var vectorToRotate = Quaternion.Euler(rotateVector3.x, Input.mousePosition.x, rotateVector3.z);
        myCamera.transform.rotation = Quaternion.Lerp(myCamera.transform.rotation, vectorToRotate, timeLerp);


        if (Input.GetKeyUp(KeyCode.D))
            UpdateManager.AddObjectUpdateable(this);

        if (Input.GetKeyUp(KeyCode.F))
            UpdateManager.RemoveObjectUpdateable(this);

        Debug.DrawRay(myCamera.transform.position, (myCamera.transform.forward * 10) * -1, Color.blue);
    }

    public void OnUpdate()
    {
        Debug.Log(name);
    }
}
