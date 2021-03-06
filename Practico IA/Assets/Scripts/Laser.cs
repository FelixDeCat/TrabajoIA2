﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Laser : MonoBehaviour
{
    public float speed;
    public float lifeSpan;
    private float _tick;
    private bool _alive;
    public Transform spawner;

    private void Awake()
    {
        spawner = GameObject.FindObjectsOfType<Hero>().First().spawnBullets;
    }


    void Update()
    {
        _tick += Time.deltaTime;
        if (_tick >= lifeSpan) DisposeLaser(this);
        else transform.position += transform.forward * speed;
    }

    public void Initialize()
    {
        _tick = 0;
        transform.position = spawner.position;
        transform.forward = spawner.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.ENEMY) other.GetComponent<Enemy>().TakeDamage(20, this.transform.forward);
    }

    public void Dispose() {  }

    /// <summary> Funcion Static para que podamos acceder desde el LaserSpawner,y que se encarga de Activar el GameObject(en caso de serlo) y llama a su funcion inicial </summary>
    public static void InitializeLaser(Laser bulletObj)
    {
        bulletObj.gameObject.SetActive(true);
        bulletObj.Initialize();
    }
    /// <summary> Funcion Static para que podamos acceder desde el LaserSpawner,y que se encarga de Desactivar el GameObject(en caso de serlo) y llama a su funcion Dispose </summary>
    public static void DisposeLaser(Laser bulletObj)
    {
        bulletObj.Dispose();
        bulletObj.gameObject.SetActive(false);
    }
}
