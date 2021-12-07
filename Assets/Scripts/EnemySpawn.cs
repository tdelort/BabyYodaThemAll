using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject enemy1Prefab;
    [SerializeField] GameObject enemy2Prefab;
    [SerializeField] float spawnDelay = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        int i = 0;
        while (true)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                i = 1 - i;
                var enemy = Instantiate(i == 0 ? enemy1Prefab : enemy2Prefab, transform.position, Quaternion.identity);
                enemy.GetComponent<NetworkObject>().Spawn();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
