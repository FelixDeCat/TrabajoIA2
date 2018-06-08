using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Beam : MonoBehaviour
{

    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.O)) Shoot();
    }

    // IA 2 P1 AGGREGATE

    public void Shoot()
    {
        RaycastHit[] raycastInfo = Physics.RaycastAll(transform.position, transform.forward);

        raycastInfo.Where(x => x.collider.gameObject.layer == Layers.ENEMY)
                   .Select(x => x.collider.GetComponent<Enemy>())
                   .Aggregate(100f,
                   (acum, current) =>
                   {
                       current.TakeDamage(acum);
                       acum = acum / 2;
                       return acum;
                   });

        
    }
}
