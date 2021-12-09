using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Force : NetworkBehaviour
{
    public float speed;

    private float time;
    private int height;

    private void OnEnable()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
        while (true)
        {
            yield return null;
            transform.localPosition = new Vector3(0, 0.5f, transform.localPosition.z + Time.deltaTime * speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player nobj = GetComponentInParent<Player>();
        if(nobj.IsLocalPlayer)
        {
            if (other.tag == "Enemy")
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if(enemy != null)
                    enemy.TakeDamage(2, nobj.id.Value);
            }
        }
    }
}
