using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, IUpdateble
{
    public float speed;
    public float maxVel;
    public float cameraSpeedRotation;
    Rigidbody _rb;

    void Start()
    {
        StartUpdating();

        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    // Solo Fisicas
    void FixedUpdate()
    {
        Move();
    }

    public void StartUpdating()
    {
        // Agrego a la lista de los actualizables
        UpdateManager.AddObjectUpdateable(this);
    }

    // Nuevo update para los actualizables
    public void OnUpdate()
    {
        RotatePlayerWithCamera();
    }

    public void StopUpdating()
    {
        // Saco de las lista de actualizables y deja de actualizarse
        UpdateManager.RemoveObjectUpdateable(this);
    }

    void RotatePlayerWithCamera()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * cameraSpeedRotation, Space.World);
    }

    void Move()
    {
        Vector2 dir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 targetVelocity = new Vector3(dir.x, 0, dir.y);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        Vector3 velocity = _rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVel, maxVel);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVel, maxVel);
        velocityChange.y = 0;
        _rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
