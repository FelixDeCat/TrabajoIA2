﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA2;

//IA2-P3

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IUpdateble
{
    [Header("For Eject")]
    public float feedbackHit;



    float life; public float Life { get { return life; } }
    bool red, green;
    public bool IsRed { get { return red; } }
    public bool IsGreen { get { return green; } }
    Renderer myRender;
    public Renderer Render { get { return myRender; } }
    public Color Color { set { myRender.material.color = value; if (value == Color.red) red = true; if (value == Color.green) green = true; } }



    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        myRender = GetComponent<Renderer>();
        target = FindObjectOfType<Hero>().gameObject;
        myRender.material.color = red ? Color.red : Color.blue;
        life = UnityEngine.Random.Range(1, 60);
    }
    protected virtual void Start()
    {
        StartUpdating();
        StateMachine();
    }

    public void TakeDamage(float damage, Vector3 dir)
    {
        life -= damage;
        _rb.AddForce( dir * feedbackHit, ForceMode.Impulse);
    }

    public enum PlayerInputs { ON_LINE_OF_SIGHT, PROBOCATED, OUT_LINE_OF_SIGHT, TIME_OUT, IN_RANGE_TO_ATTACK, OUT_RANGE_TO_ATTACK, FREEZE, DIE }
    private EventFSM<PlayerInputs> _myFsm;
    void StateMachine()
    {
        #region STATE MACHINE CONFIGS
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        var idle = new State<PlayerInputs>(CommonState.IDLE);
        var onSigth = new State<PlayerInputs>(CommonState.ONSIGHT);
        var pursuit = new State<PlayerInputs>(CommonState.PURSUIRT);
        var searching = new State<PlayerInputs>(CommonState.SEARCHING);
        var attack = new State<PlayerInputs>(CommonState.ATTACKING);
        var freeze = new State<PlayerInputs>(CommonState.FREEZE);
        var die = new State<PlayerInputs>(CommonState.DIE);

        StateConfigurer.Create(idle)
            .SetTransition(PlayerInputs.ON_LINE_OF_SIGHT, onSigth)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(onSigth)
            .SetTransition(PlayerInputs.PROBOCATED, pursuit)
            .SetTransition(PlayerInputs.OUT_LINE_OF_SIGHT, searching)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(pursuit)
            .SetTransition(PlayerInputs.OUT_LINE_OF_SIGHT, searching)
            .SetTransition(PlayerInputs.IN_RANGE_TO_ATTACK, attack)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(searching)
            .SetTransition(PlayerInputs.TIME_OUT, idle)
            .SetTransition(PlayerInputs.ON_LINE_OF_SIGHT, pursuit)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(PlayerInputs.OUT_RANGE_TO_ATTACK, pursuit)
            .SetTransition(PlayerInputs.OUT_LINE_OF_SIGHT, searching)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.FREEZE, freeze)
            .Done();

        StateConfigurer.Create(freeze)
            .SetTransition(PlayerInputs.DIE, die)
            .Done();

        StateConfigurer.Create(die).Done();


        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        #endregion

        #region STATE MACHINE DO SOMETING
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////

        //******************
        //*** IDLE
        //******************
        idle.OnUpdate += () => {
            Deb_Estado = "IDLE";
            if (LineOfSight())
            {
                Debug.Log("Line of sigth");
                SendInputToFSM(PlayerInputs.ON_LINE_OF_SIGHT);
            }
        };

        //******************
        //*** ON SIGHT
        //******************
        onSigth.OnUpdate += () => {
            Deb_Estado = "ON SIGTH";
            if (LineOfSight()) OnSight_CountDownForProbocate();
            else { timer_to_probocate = 0; SendInputToFSM(PlayerInputs.OUT_LINE_OF_SIGHT); }
        };

        //******************
        //*** PURSUIT
        //******************
        pursuit.OnUpdate += () => {
            Deb_Estado = "PURSUIT";
            if (LineOfSight())
            {
                FollowPlayer();

                if (IsInDistanceToAttack())
                {
                    SendInputToFSM(PlayerInputs.IN_RANGE_TO_ATTACK);
                }
            }
            else { timer_to_probocate = 0; SendInputToFSM(PlayerInputs.OUT_LINE_OF_SIGHT); }
        };

        //******************
        //*** SEARCH
        //******************
        searching.OnUpdate += () => {
            Deb_Estado = "SEARCH";
            if (LineOfSight()) SendInputToFSM(PlayerInputs.ON_LINE_OF_SIGHT);
            else
            {
                OutSight_CountDownForIgnore();
            }
        };

        //******************
        //*** ATTACK
        //******************
        attack.OnUpdate += () => {
            Deb_Estado = "ATTACK";
            if (LineOfSight())
            {
                if (IsInDistanceToAttack()) Attack();
                else SendInputToFSM(PlayerInputs.OUT_RANGE_TO_ATTACK);
            }
            else SendInputToFSM(PlayerInputs.OUT_LINE_OF_SIGHT);
        };

        //******************
        //*** FREEZE
        //******************
        freeze.OnEnter += x => {
            Deb_Estado = "FREEZE";
            myRender.material.color = Color.grey;
            canMove = false;
        };

        //******************
        //*** DEATH
        //******************
        die.OnEnter += x => {
            Deb_Estado = "DEATH";
            canMove = false;
            if (myRender.material.color == Color.black) return;
            myRender.material.color = Color.black;
            transform.localScale = transform.localScale / 2;
            StopUpdating();
        };

        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        #endregion

        _myFsm = new EventFSM<PlayerInputs>(idle);
    }

    [Header("For Probocate")]
    public float Time_to_Probocate = 5f;
    float timer_to_probocate;
    void OnSight_CountDownForProbocate()
    {
        if (timer_to_probocate < Time_to_Probocate) timer_to_probocate = timer_to_probocate + 1 * Time.deltaTime;
        else { timer_to_probocate = 0; SendInputToFSM(PlayerInputs.PROBOCATED); }
    }

    [Header("For Distance To Attack")]
    public float minDisToAttack = 5f;
    bool IsInDistanceToAttack()
    {
        return Vector3.Distance(transform.position, target.transform.position) < minDisToAttack;
    }

    [Header("For Ignore in Time Out")]
    public float TimeToIgnore = 5f;
    float _timer_to_ignore;
    void OutSight_CountDownForIgnore()
    {
        if (_timer_to_ignore < TimeToIgnore) _timer_to_ignore = _timer_to_ignore + 1 * Time.deltaTime;
        else { _timer_to_ignore = 0; SendInputToFSM(PlayerInputs.TIME_OUT); }
    }

    [Header("For Line of Sight")]
    public float viewDistance;
    public float viewAngle;
    GameObject target;
    protected virtual bool LineOfSight()
    {
        //si no me puedo mover... al pedo calcular lo demas
        if (!canMove) return false;

        _distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (_distanceToTarget > viewDistance)
        {
            return false;
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
            return !obstaclesBetween ? true : false;
        }
        else return false;
    }

    [Header("For Pursuit")]
    public float rotationSpeed;
    bool canMove = true;
    Rigidbody _rb;
    Vector3 _directionToTarget;
    float _angleToTarget;
    float _distanceToTarget;
    bool _playerInSight;
    protected virtual void FollowPlayer()
    {
        if (!canMove) return;

        float velY = _rb.velocity.y;
        _rb.velocity = new Vector3(_directionToTarget.x, _directionToTarget.y + velY, _directionToTarget.z);
        transform.forward = Vector3.Lerp(transform.forward, _directionToTarget, rotationSpeed * Time.deltaTime);
    }

    void Attack()
    {
        Debug.Log("I'm attacking you (" + target.name + ")");
    }

    public void Scare()
    {
        SendInputToFSM(PlayerInputs.FREEZE);
    }
    public void Death()
    {
        SendInputToFSM(PlayerInputs.DIE);
    }
    public void Eject()
    {
        _rb.AddExplosionForce(5000, transform.position, 1);
    }
    public void TakeDamage(int damage, Vector3 v3direccion)
    {
        life -= damage;
        _rb.AddForce(v3direccion * feedbackHit, ForceMode.Impulse);
    }

    private void SendInputToFSM(PlayerInputs inp)
    {
        _myFsm.SendInput(inp);
    }
    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
    }


    public virtual void StartUpdating() { UpdateManager.AddObjectUpdateable(this); }
    public virtual void StopUpdating() { UpdateManager.RemoveObjectUpdateable(this); }


    public virtual void OnUpdate()
    {
        _myFsm.Update();
        //LineOfSight();
        //FollowPlayer();
        if (life <= 0) Death();
    }


    [Header("Gizmos & Feedback")]
    public bool DrawGizmos;
    [SerializeField]
    TextMesh debug_estado; public object Deb_Estado { set { debug_estado.text = value.ToString(); } }
    protected virtual void OnDrawGizmos()
    {
        if (!DrawGizmos) return;

        target = FindObjectOfType<Hero>().gameObject;

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
}