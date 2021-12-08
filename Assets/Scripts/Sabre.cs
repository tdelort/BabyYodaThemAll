using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Sabre : MonoBehaviour
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

}
