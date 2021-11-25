using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Translation
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 movement = input * speed * Time.deltaTime;

        // Rotation
        Quaternion rotation = ComputeRotation();

        transform.position += movement;
        transform.rotation = rotation;

        if (Input.GetMouseButtonDown(0))
        {
            Action1();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Action2();
        }
    }

    private Quaternion ComputeRotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            return targetRotation;
        }
        return Quaternion.Euler(0, 0, 0);
    }
    protected virtual void Action1()
    {
        Debug.Log("Action1");

    }

    protected virtual void Action2()
    {
        Debug.Log("Action2");
    }
}
