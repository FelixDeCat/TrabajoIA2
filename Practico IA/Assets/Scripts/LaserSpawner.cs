using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public int amount;
    public Laser prefab;
    private Pool<Laser> _bulletPool;

    private static LaserSpawner _instance;
    public static LaserSpawner Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
        //Creamos un pool de T cosas,le pasamos una cantidad inicial,le pasamos como crear los objeto que queremos que guarde,y sus funciones de Inicializarse y de Finalizarse,ademas le indicamos si queremos que sea Dinamico o no
        _bulletPool = new Pool<Laser>(amount, BulletFactory, Laser.InitializeLaser, Laser.DisposeLaser, true);
    }

    void Update()
    {
        if (Input.GetButtonDown(PHYSICAL_INPUT.FIRE1) || Input.GetKeyDown(KeyCode.E))
        {
            _bulletPool.GetObjectFromPool();
        }
    }
    /// <summary>
    /// La Factory de como crear el objeto que va a guardar
    /// </summary>
    /// <returns></returns>
    private Laser BulletFactory()
    {
        return Instantiate<Laser>(prefab);
    }

    /// <summary>
    /// El spawner es el unico que conoce al pool, asi que nos llega un objeto por parametro y a ese lo mandamos por parametro para que el pool se encargue de desactivarlo de su lista y que pueda volver a usarlo
    /// </summary>
    /// <param name="laser"></param>
    public void ReturnBulletToPool(Laser laser)
    {
        _bulletPool.DisablePoolObject(laser);
    }
}
