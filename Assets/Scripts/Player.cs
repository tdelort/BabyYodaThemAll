using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    [SerializeField] private GameObject cursorPrefab;
    GameObject cursor;
    public override void OnNetworkSpawn()
    {
        Debug.Log("Player started");
        if (IsLocalPlayer)
        {
            Camera camera = Camera.main;
            camera.transform.rotation = Quaternion.Euler(45, 0, 0);
            CameraController cameraController = camera.GetComponent<CameraController>();
            cameraController.target = transform;
            cameraController.offset = new Vector3(0, 10, -10);
        }

        if (NetworkManager.Singleton.IsServer)
        {
            cursor = Instantiate(cursorPrefab);
            cursor.GetComponent<NetworkObject>().Spawn();
        }

        InitAction1();
        InitAction2();
        InitAction3();

    }

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer)
        {
            // Translation
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 movement = input * speed * Time.deltaTime;

            // Rotation
            Quaternion rotation = ComputeRotation();

            TransformPlayerServerRpc(movement, rotation);

            if (Input.GetButtonDown("Action1"))
            {
                Action1();
            }

            if (Input.GetButtonDown("Action2"))
            {
                Action2();
            }

            if (Input.GetButtonDown("Action3"))
            {
                Action3();
            }
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
            MoveCursorServerRpc(targetPoint);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            return targetRotation;
        }
        return Quaternion.Euler(0, 0, 0);
    }

    protected virtual void Action1()
    {
        Debug.Log("Player.Action1");
    }

    protected virtual void Action2()
    {
        Debug.Log("Player.Action2");
    }

    protected virtual void Action3()
    {
        Debug.Log("Player.Action3");
    }

    protected virtual void InitAction1()
    {
        Debug.Log("Player.InitAction1");
    }

    protected virtual void InitAction2()
    {
        Debug.Log("Player.InitAction2");
    }

    protected virtual void InitAction3()
    {
        Debug.Log("Player.InitAction3");
    }

    [ServerRpc]
    public void TransformPlayerServerRpc(Vector3 movement, Quaternion rotation)
    {
        transform.position += movement;
        transform.rotation = rotation;
    }

    [ServerRpc]
    public void MoveCursorServerRpc(Vector3 position)
    {
        cursor.transform.position = position;
    }
}
