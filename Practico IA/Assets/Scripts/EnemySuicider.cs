﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemySuicider : Enemy
{

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void StartUpdating()
    {
        base.StartUpdating();
    }
    public override void StopUpdating()
    {
        base.StopUpdating();
    }
    protected override void FollowPlayer()
    {
        base.FollowPlayer();
    }
    protected override void LineOfSight()
    {
        base.LineOfSight();
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
