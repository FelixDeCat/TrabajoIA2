using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIsGrounded : MonoBehaviour {

    private bool isGrounded;

    public TextMesh txtMeshDeb;

    public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; txtMeshDeb.text = value ? "G" : "F"; } }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            IsGrounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IsGrounded = false;
    }
}
