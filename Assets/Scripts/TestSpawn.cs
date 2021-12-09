using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class TestSpawn : MonoBehaviour
{
    [SerializeField] GameObject enemy1Prefab;
    [SerializeField] GameObject enemy2Prefab;

    void Start() {
        StartCoroutine(SpawnEnemy());
    }
    IEnumerator SpawnEnemy()
    {
        Player[] targets = GameObject.FindObjectsOfType<Player>();
        while (targets.Length <= 0) {
            targets = GameObject.FindObjectsOfType<Player>();
            yield return new WaitForSeconds(1);
        }

        while (true)
        {
            Vector3 pos = transform.position;
            var enemy1 = Instantiate(enemy1Prefab, pos, Quaternion.identity);
            enemy1.GetComponent<NetworkObject>().Spawn();
            yield return new WaitForSeconds(1);
            
            var enemy2 = Instantiate(enemy2Prefab, pos, Quaternion.identity);
            enemy2.GetComponent<NetworkObject>().Spawn();
            yield return new WaitForSeconds(1);

            yield return new WaitForSeconds(1);
        }
    }
}
