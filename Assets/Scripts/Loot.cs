using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Loot : MonoBehaviour
{
    public void OnTriggerEnter(Collider other) {
        if (NetworkManager.Singleton.IsServer) {
            if (other.tag == "Player") {
                other.GetComponent<Player>().AddScore();
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
