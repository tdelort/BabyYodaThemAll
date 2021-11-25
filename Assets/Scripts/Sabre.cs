using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sabre : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, speed*360*Time.deltaTime, 0, Space.Self);
    }
}
