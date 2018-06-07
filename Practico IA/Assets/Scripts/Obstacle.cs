using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Obstacle : MonoBehaviour {

    float scale_jumpeable = 0.8f;
    float scale_no_jumpeable = 1f;

    float pos_Croucheable = 1;

    private void Awake()
    {
        this.gameObject.tag = "Floor";
        transform.GetComponentsInChildren<Transform>().ToList().ForEach(x => x.gameObject.tag = "Floor");
    }

    public bool IsJumpeable {
        set {
            Vector3 scale = transform.localScale;
            scale.y = value ? Random.Range(0.1f, scale_jumpeable) : Random.Range(scale_no_jumpeable, 5);
            transform.localScale = scale;
        }
        get { return transform.localScale.y <= scale_jumpeable; }
    }

    public bool IsCroucheable {
        set {
            Vector3 pos = transform.position;
            pos.y = value ? 1 : -0.5f;
            transform.position = pos;
        }
        get { return transform.position.y >= pos_Croucheable; }
    }

    public Vector2 WidthHeigth
    {
        set {
            Vector3 scale = transform.localScale;
            scale.x = value.x;
            scale.z = value.y;
            transform.localScale = scale;
        }
    }


}
