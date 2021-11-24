using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer)
        {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 movement = input * speed * Time.deltaTime;
            Debug.Log("Input : " + input);
            transform.Translate(movement);
        }
    }

    //[ServerRpc]
    //public void MovePlayerServerRpc(Vector3 movement)
    //{
    //    Debug.Log("MovePlayerServerRpc : " + movement);
    //    transform.position += movement;
    //}
}
