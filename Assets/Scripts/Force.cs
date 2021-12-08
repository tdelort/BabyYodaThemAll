using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
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
}
