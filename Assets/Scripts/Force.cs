using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public float speed;

    private float time;
    private int height;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z + Time.deltaTime * speed);
    }

    private void OnEnable()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //faire les dégâts
        }
    }
}
