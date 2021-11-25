using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public Transform target = null;

    // Update is called once per frame
    void Update()
    {   
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
