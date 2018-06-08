using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Beam : MonoBehaviour
{
    public float distanceToTarget;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Shoot()
    {
        RaycastHit[] raycastInfo = Physics.RaycastAll(transform.position, transform.forward);
        
        // raycastInfo.Where(x => x.collider.gameObject.layer == Layers.ENEMY).Aggreg
        
    }
}
