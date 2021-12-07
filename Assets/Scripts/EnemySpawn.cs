using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnDelay = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                enemy.GetComponent<NetworkObject>().Spawn();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
