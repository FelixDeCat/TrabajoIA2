﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IUpdateble
{
    public GameObject target;
    public float viewDistance;
    public float viewAngle;
    public float rotationSpeed;

    public bool DrawGizmos;

    [Space(10)]
    int life; public int Life { get { return life; } }
    bool red, green;
    public bool IsRed { get { return red; } }
    public bool IsGreen { get { return green; } }
    public Renderer myRender;
    public Color Color { set { myRender.material.color = value; if (value == Color.red) red = true; if (value == Color.green) green = true; } }

    public void Scare()
    {
        myRender.material.color = Color.grey;
        canMove = false;
    }

    public void Death()
    {
        if (myRender.material.color == Color.black) return;

        myRender.material.color = Color.black;
        transform.localScale = transform.localScale / 2;
        StopUpdating();
    }

    public void Eject()
    {
        _rb.AddExplosionForce(5000, transform.position, 1);
    }

    public void TakeDamage(int damage)
    {
        life -= damage;
    }

    bool canMove = true;
    private Rigidbody _rb;
    private Vector3 _directionToTarget;
    private float _angleToTarget;
    private float _distanceToTarget;
    private bool _playerInSight;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        myRender = GetComponent<Renderer>();
        myRender.material.color = red ? Color.red : Color.blue;
        life = UnityEngine.Random.Range(1, 60);
    }

    protected virtual void Start () {
        StartUpdating();
    }
	

    protected virtual void LineOfSight()
    {
        if (!canMove) return;

        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (_distanceToTarget > viewDistance)
        {
            _playerInSight = false;
            return;
        }

        _directionToTarget = (target.transform.position - transform.position).normalized;

        _angleToTarget = Vector3.Angle(transform.forward, _directionToTarget);

        if (_angleToTarget <= viewAngle)
        {
            RaycastHit raycastInfo;
            bool obstaclesBetween = false;

            if (Physics.Raycast(transform.position, _directionToTarget, out raycastInfo, _distanceToTarget))
                if (raycastInfo.collider.gameObject.layer == Layers.WORLD)
                    obstaclesBetween = true;


            _playerInSight = !obstaclesBetween ? true : false;

        }
        else
        {
            _playerInSight = false;
        }
    }
    protected virtual void FollowPlayer()
    {
        if (!canMove) return;

        if (!_playerInSight) return;

        float velY = _rb.velocity.y;
        _rb.velocity = new Vector3(_directionToTarget.x, _directionToTarget.y + velY, _directionToTarget.z);
        transform.forward = Vector3.Lerp(transform.forward, _directionToTarget, rotationSpeed * Time.deltaTime);
    }

    public virtual void StartUpdating() { UpdateManager.AddObjectUpdateable(this); }
    public virtual void StopUpdating() { UpdateManager.RemoveObjectUpdateable(this); }

    protected virtual void OnDrawGizmos()
    {
        if (!DrawGizmos) return;

        if (_playerInSight)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, target.transform.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * viewDistance));

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistance));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistance));
    }

    public virtual void OnUpdate()
    {
        LineOfSight();
        FollowPlayer();

        if (life <= 0) Death();
    }
}