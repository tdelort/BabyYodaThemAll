using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    
    private GameObject sabreLancer = null;
    private GameObject sabre = null;
    // Start is called before the first frame update
    void Start()
    {
        Camera camera = Camera.main;
        camera.transform.rotation = Quaternion.Euler(45, 0, 0);
        CameraController cameraController = camera.GetComponent<CameraController>();
        cameraController.target = transform;
        cameraController.offset = new Vector3(0, 10, -10);
        sabre = transform.GetChild(0).transform.gameObject;
        sabreLancer = transform.GetChild(1).transform.gameObject;
        sabreLancer.SetActive(false);
        sabre.SetActive(true);
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
            StartCoroutine(Action1());
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
    protected virtual IEnumerator Action1()
    {
        Debug.Log("Action1");
        sabreLancer.SetActive(true);
        sabre.SetActive(false);
        yield return new WaitForSeconds(1f);
        sabreLancer.SetActive(false);
        sabre.SetActive(true);
    }

    protected virtual void Action2()
    {
        Debug.Log("Action2");
    }
}
