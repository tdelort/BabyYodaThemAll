using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Sabre : NetworkBehaviour
{
    [SerializeField] private float speed = 1f;

    public void Rotate()
    {
        StartCoroutine(RotateCoroutine());
    }

    private IEnumerator RotateCoroutine()
    {
        while(true)
        {
            transform.Rotate(0, speed*360*Time.deltaTime, 0, Space.Self);
            yield return null;
        }
    }
}
