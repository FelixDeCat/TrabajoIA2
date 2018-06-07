using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed;
    public float lifeSpan;
    private float _tick;
    private bool _alive;
    public GameObject spawner;
    

    void Update()
    {
        _tick += Time.deltaTime;
        if (_tick >= lifeSpan) DisposeLaser(this);
        else    transform.position += transform.forward * speed;
    }

    public void Initialize()
    {
        _tick = 0;
        transform.position = spawner.transform.position;
    }

    public void Dispose()
    {
     
    }

    /// <summary>
    /// Funcion Static para que podamos acceder desde el LaserSpawner,y que se encarga de Activar el GameObject(en caso de serlo) y llama a su funcion inicial
    /// </summary>
    /// <param name="bulletObj"></param>
    public static void InitializeLaser(Laser bulletObj)
    {
        bulletObj.gameObject.SetActive(true);
        bulletObj.Initialize();
    }

    /// <summary>
    ///  Funcion Static para que podamos acceder desde el LaserSpawner,y que se encarga de Desactivar el GameObject(en caso de serlo) y llama a su funcion Dispose
    /// </summary>
    /// <param name="bulletObj"></param>
    public static void DisposeLaser(Laser bulletObj)
    {
        bulletObj.Dispose();
        bulletObj.gameObject.SetActive(false);
    }
}
