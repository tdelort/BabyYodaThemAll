using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Basique : NetworkBehaviour
{
    [SerializeField] private float speed = 1f;

    private bool left = false;
    private bool right = false;

    void OnEnable()
    {
        StartCoroutine(Attack());
    }

    private void Update()
    {
        if (left)
        {
            transform.Rotate(0, -360 * speed * Time.deltaTime, 0);
        }
        if (right)
        {
            transform.Rotate(0, 360* speed * Time.deltaTime, 0);
        }
    }

    private IEnumerator Attack()
    {
        //transform.localRotation = Quaternion.identity;
        left = true;
        yield return new WaitForSeconds(0.2f);
        left = false;
        right = true;
        yield return new WaitForSeconds(0.2f);
        right = false;
    }
}
