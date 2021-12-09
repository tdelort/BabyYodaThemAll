using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Sabre : NetworkBehaviour
{
    [SerializeField] private float speed = 1f;

    private void OnEnable()
    {
        StartCoroutine(RotateCoroutine());
    }

    private IEnumerator RotateCoroutine()
    {
        transform.localRotation = Quaternion.identity;
        while(true)
        {
            transform.Rotate(0, speed*360*Time.deltaTime, 0, Space.Self);
            yield return null;
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
