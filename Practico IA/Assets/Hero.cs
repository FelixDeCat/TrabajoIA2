using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, IUpdateble
{
    public float speed;
    public float jumpForce;
    public float cameraSpeedRotation;
    Rigidbody _rb;

    void Start()
    {
        // Agrego a la lista de los actualizables
        UpdateManager.AddObjectUpdateable(this);


        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        Move();
    }

    // Nuevo update para los actualizables
    public void OnUpdate()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * cameraSpeedRotation, Space.World);
    }

    void Move()
    {

    }

    

    public void StopUpdating()
    {
        // Saco de las lista de actualizables y deja de actualizarse
        UpdateManager.RemoveObjectUpdateable(this);
    }
         
}
