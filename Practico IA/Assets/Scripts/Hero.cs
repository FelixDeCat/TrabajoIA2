using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IA2;
using System.Linq;

//IA2-P3
public class Hero : MonoBehaviour, IUpdateble
{
    public float cameraSpeedRotation;
    Rigidbody _rb;

    Queries querie;
    HeroQueriesActions qActions;

    public Transform spawnBullets;

    public enum PlayerInputs { MOVE, JUMP, IDLE, DIE, CROUCH }
    private EventFSM<PlayerInputs> _myFsm;

    [Header("Salto")]
    public float gravity = 0.75f;
    public float jumpForce = 5f;
    bool GoToJump = false;
    public Vector3 moveFall;
    float inputJump;
    public CheckIsGrounded check;
    bool isJumpFall;
    public ParticleSystem particles_JumpFall;

    [Header("Movimiento")]
    public float speed;
    Vector3 movehorizontal;
    Vector3 movevertical;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        querie = GetComponent<Queries>();
        querie.isBox = false;

        qActions = GetComponent<HeroQueriesActions>();

        StateMachine();
    }

    void Start()
    {
        AddListUpdateables();
    }

    //////////////////////////////////////////////////////
    // STATE MACHINE
    //////////////////////////////////////////////////////

    void StateMachine()
    {
        #region STATE MACHINE CONFIGS

        var idle = new State<PlayerInputs>(CommonState.IDLE);
        var moving = new State<PlayerInputs>(CommonState.MOVING);
        var jumping = new State<PlayerInputs>(CommonState.JUMPING);
        var crouch = new State<PlayerInputs>(CommonState.CROUCH);
        var die = new State<PlayerInputs>(CommonState.DIE);

        StateConfigurer.Create(idle)
            .SetTransition(PlayerInputs.MOVE, moving)
            .SetTransition(PlayerInputs.JUMP, jumping)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.CROUCH, crouch)
            .Done();

        StateConfigurer.Create(moving)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.JUMP, jumping)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.CROUCH, crouch)
            .Done();

        StateConfigurer.Create(jumping)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.MOVE, moving)
            .SetTransition(PlayerInputs.DIE, die)
            .SetTransition(PlayerInputs.CROUCH, crouch)
            .Done();

        StateConfigurer.Create(die).Done();

        StateConfigurer.Create(crouch)
            .SetTransition(PlayerInputs.IDLE, idle)
            .SetTransition(PlayerInputs.DIE, die)
            .Done();

        #endregion
        #region STATE MACHINE DO SOMETING

        // <IDLE>
        idle.OnEnter += x =>
        {
            Deb_Est = "IDLE";
        };
        idle.OnUpdate += () =>
        {
            if (Input.GetAxis(PHYSICAL_INPUT.HORIZONTAL) != 0 || Input.GetAxis(PHYSICAL_INPUT.VERTICAL) != 0)
            {
                SendInputToFSM(PlayerInputs.MOVE);
            }
            else if (Input.GetButtonDown(PHYSICAL_INPUT.JUMP))
            {
                SendInputToFSM(PlayerInputs.JUMP);
            }
            else if (Input.GetButtonDown(PHYSICAL_INPUT.CROUCH))
            {
                SendInputToFSM(PlayerInputs.CROUCH);
            }
        };
        idle.OnFixedUpdate += () =>
        {
            StateStill();
        };
        // </IDLE>

        // <MOVING>
        moving.OnEnter += x =>
        {
            Deb_Est = "MOVING";
        };
        moving.OnUpdate += () =>
        {
            if (Input.GetAxis(PHYSICAL_INPUT.HORIZONTAL) == 0 && Input.GetAxis(PHYSICAL_INPUT.VERTICAL) == 0)
            { SendInputToFSM(PlayerInputs.IDLE); }
            else if (Input.GetKeyDown(KeyCode.Space))
                SendInputToFSM(PlayerInputs.JUMP);
            else if (Input.GetButtonDown(PHYSICAL_INPUT.CROUCH))
                SendInputToFSM(PlayerInputs.CROUCH);
        };
        moving.OnFixedUpdate += () =>
        {
            if (!check.IsGrounded) moveFall.y -= gravity;
            else moveFall = Vector3.zero;
            movevertical = transform.forward * Input.GetAxis("Vertical") * speed;
            movehorizontal = transform.right * Input.GetAxis("Horizontal") * speed;
        };
        moving.GetTransition(PlayerInputs.JUMP).OnTransition += x =>
        {
            Deb_Trans = "salto en largo";
        };
        moving.GetTransition(PlayerInputs.CROUCH).OnTransition += x =>
        {
            Deb_Trans = "Rodar";
        };
        // </MOVING>

        // <JUMP>
        jumping.OnEnter += x =>
        {
            GoToJump = true;
            Deb_Est = "JUMP";
        };
        jumping.OnFixedUpdate += () =>
        {
            if (GoToJump && check.IsGrounded)// aca salto
            {
                moveFall.y = jumpForce;
                GoToJump = false;
            }
            else if (!GoToJump && check.IsGrounded)//aca no salto mas y toque el piso
            {
                moveFall.y = 0;
                if (Input.GetAxis(PHYSICAL_INPUT.HORIZONTAL) == 0 && Input.GetAxis(PHYSICAL_INPUT.VERTICAL) == 0)
                    SendInputToFSM(PlayerInputs.IDLE);
                else if (Input.GetAxis(PHYSICAL_INPUT.HORIZONTAL) != 0 || Input.GetAxis(PHYSICAL_INPUT.VERTICAL) != 0)
                    SendInputToFSM(PlayerInputs.MOVE);
            }
            else// la caida
            {
                moveFall.y -= gravity;

                if (Input.GetButtonDown(PHYSICAL_INPUT.CROUCH))
                {
                    SendInputToFSM(PlayerInputs.CROUCH);
                }

            }
        };
        jumping.GetTransition(PlayerInputs.CROUCH).OnTransition += x =>
        {
            isJumpFall = true;
            Deb_Trans = "Caida Potente";
        };
        // </JUMP>

        // <CROUCH>
        crouch.OnEnter += x =>
        {
            Deb_Est = "CROUCH";
            if (x != PlayerInputs.JUMP)
            {
                movehorizontal = Vector3.zero;
                movevertical = Vector3.zero;
            }

            transform.localScale = new Vector3(1, 0.5f, 1);
        };
        crouch.OnUpdate += () =>
        {
            if (Input.GetButtonUp(PHYSICAL_INPUT.CROUCH))
            {
                SendInputToFSM(PlayerInputs.IDLE);
                transform.localScale = new Vector3(1, 1, 1);
            }
        };
        crouch.OnFixedUpdate += () =>
        {
            if (check.IsGrounded && isJumpFall)
            {
                particles_JumpFall.Play();
                isJumpFall = false;

                qActions.Button_JumpKill();
                qActions.EjectWeakAndScared();
            }

            if (!check.IsGrounded) moveFall.y -= gravity * 3;
            else
            {
                movevertical = transform.forward * Input.GetAxis(PHYSICAL_INPUT.VERTICAL) * (speed / 2);
                movehorizontal = transform.right * Input.GetAxis(PHYSICAL_INPUT.HORIZONTAL) * (speed / 2);
            }
        };
        // </CROUCH>

        #endregion
        _myFsm = new EventFSM<PlayerInputs>(idle);
    }

    //////////////////////////////////////////////////////
    // UPDATES AND INPUT_SENDERS
    //////////////////////////////////////////////////////

    private void SendInputToFSM(PlayerInputs inp)
    {
        _myFsm.SendInput(inp);
    }
    public void OnUpdate()
    {
        _myFsm.Update();
        RotatePlayerWithCamera();
    }
    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
        ConstantVelocity();
    }

    //////////////////////////////////////////////////////
    // FOR MOVE AND JUMP
    //////////////////////////////////////////////////////

    void StateStill()
    {
        if (!check.IsGrounded) moveFall.y -= gravity;
        else
        {
            if (isJumpFall)
            {
                particles_JumpFall.Play();
                isJumpFall = false;
            }
            moveFall = Vector3.zero;
        }
        if (Input.GetAxis(PHYSICAL_INPUT.HORIZONTAL) == 0) movehorizontal = Vector3.zero;
        if (Input.GetAxis(PHYSICAL_INPUT.VERTICAL) == 0) movevertical = Vector3.zero;
    }
    void RotatePlayerWithCamera()
    {
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * cameraSpeedRotation, Space.World);
    }
    public void ConstantVelocity()
    {
        Vector3 movement = movehorizontal + movevertical + moveFall;
        _rb.velocity = movement;
    }

    //////////////////////////////////////////////////////
    // FOR UPDATE MANAGER
    //////////////////////////////////////////////////////

    public void AddListUpdateables() { UpdateManager.AddObjectUpdateable(this); }
    public void StopUpdating() { UpdateManager.RemoveObjectUpdateable(this); }

    //////////////////////////////////////////////////////
    // FOR DEBUGGING
    //////////////////////////////////////////////////////

    [Header("Solo para debug")]
    [SerializeField]
    Text deb_Estado; string Deb_Est { set { deb_Estado.text = value; } }
    [SerializeField]
    Text deb_Trans; string Deb_Trans { set { deb_Trans.text = value; } }
    [SerializeField]
    Text deb_Queries; object Deb_Queries { set { deb_Queries.text = value.ToString(); } }


    //////////////////////////////////////////////////////
    // FOR UPDATE QUERIES
    //////////////////////////////////////////////////////

    //IA2-P1 (Select, Where)
    public IEnumerable<Enemy> GetCurrentEnemies()
    {
        return querie.Query()
            .Where(x => x.gameObject.GetComponent<Enemy>() != null)
            .Select(x => x.GetComponent<Enemy>()).ToList();
    }
}
