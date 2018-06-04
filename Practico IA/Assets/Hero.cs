using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IA2;

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

        //deb example
        Deb = "hola";

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

    /// <summary> No modificar esta variable, pera debugear, hacerlo con Deb </summary>
    [Header("Solo para debug")][SerializeField] Text deb_Estado;
    string Deb { set { deb_Estado.text = value; } }
}
