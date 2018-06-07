using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemySuicider : MonoBehaviour, IUpdateble
{
    public GameObject target;
    public float viewDistance;
    public float viewAngle;
    public float rotationSpeed;

    [Space(10)]
    int life; public int Life { get { return life; } }
    bool red, green;
    public bool IsRed { get { return red; } } 
    public bool IsGreen { get { return green; } }
    Renderer myRender;
    public Color Color { set { myRender.material.color = value; if (value == Color.red) red = true; if (value == Color.green) green = true; } }

    private Rigidbody _rb;
    private Vector3 _directionToTarget;
    private float _angleToTarget;
    private float _distanceToTarget;
    private bool _playerInSight;

    void Start ()
    {
        _rb = GetComponent<Rigidbody>();
        StartUpdating();

        myRender = GetComponent<Renderer>();
        myRender.material.color = red ? Color.red : Color.blue;
        life = Random.Range(1,100);
	}

    

   

    // Nuevo update para los actualizables
    public void OnUpdate()
    {
        LineOfSight();
        FollowPlayer();
    }

    public void StartUpdating()
    {
        // Agrego a la lista de los actualizables
        UpdateManager.AddObjectUpdateable(this);
    }
    public void StopUpdating()
    {
        // Saco de las lista de actualizables y deja de actualizarse
        UpdateManager.RemoveObjectUpdateable(this);
    }

    void FollowPlayer()
    {
        if (!_playerInSight) return;

        float velY = _rb.velocity.y;
        _rb.velocity = new Vector3(_directionToTarget.x, _directionToTarget.y + velY, _directionToTarget.z);
        transform.forward = Vector3.Lerp(transform.forward, _directionToTarget, rotationSpeed * Time.deltaTime);
    }

    void LineOfSight()
    {
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

    void OnDrawGizmos()
    {
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
