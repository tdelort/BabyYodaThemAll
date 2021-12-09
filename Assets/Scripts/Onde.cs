using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Onde : NetworkBehaviour
{
    public float growSpeed;

    private float time;
    private int height;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.localScale = new Vector3(100 + time * growSpeed, 100 + time * growSpeed, 1000);
    }

    private void OnEnable()
    {
        transform.localScale = new Vector3(100, 100, 1000);
        time = 0;
        transform.localPosition = new Vector3(0, 0, 0);
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
