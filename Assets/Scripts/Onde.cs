using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : MonoBehaviour
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
}
