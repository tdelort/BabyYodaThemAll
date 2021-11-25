using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    public override void OnNetworkSpawn()
    {
        Debug.Log("Player started");
        if (IsLocalPlayer)
        {
            var camera = Camera.main;
            camera.transform.position = transform.position + new Vector3(0, 10, -10);
            camera.transform.rotation = Quaternion.Euler(45, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer)
        {
            // gathering inputs
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 movement = input * speed * Time.deltaTime;
            MovePlayerServerRpc(movement);

            if (Input.GetMouseButtonDown(0))
            {
                //Action1();
            }

        }
    }

    [ServerRpc]
    public void MovePlayerServerRpc(Vector3 movement)
    {
        Debug.Log("MovePlayerServerRpc : " + movement);
        transform.position += movement;
    }
}
