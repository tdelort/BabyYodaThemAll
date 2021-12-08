using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Basique sabreBasique;

    private GameObject sabreLancer = null;
    private GameObject ondes = null;
    private GameObject sabre = null;
    private GameObject force = null;
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
        ondes = transform.GetChild(2).transform.gameObject;
        force = transform.GetChild(3).transform.gameObject;
        sabreLancer.SetActive(false);
        ondes.SetActive(false);
        force.SetActive(false);
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
            Debug.Log("attack");
            //StartCoroutine(sabreBasique.Attack());
        }

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Action2());
        }

        if (Input.GetMouseButtonDown(2))
        {
            StartCoroutine(Action3());
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

    protected virtual IEnumerator Action2()
    {
        Debug.Log("Action2");
        ondes.SetActive(true);
        yield return new WaitForSeconds(5f);
        ondes.SetActive(false);
    }

    protected virtual IEnumerator Action3()
    {
        Debug.Log("Action3");
        force.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        force.SetActive(false);
    }
}
