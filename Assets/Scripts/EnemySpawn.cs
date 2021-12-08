using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] GameObject enemy1Prefab;
    [SerializeField] GameObject enemy2Prefab;

    [System.Serializable]
    struct EnemySpawnWave
    {
        public int enemy1Count;
        public int enemy2Count;
        public BoxCollider spawnArea;
        public float spawnDelay;
        public float spawnDelayAfterWave;
    }

    static Vector3 GetRandomSpawnPosition(BoxCollider col)
    {
        return new Vector3(
            Random.Range(col.bounds.min.x, col.bounds.max.x),
            1,
            Random.Range(col.bounds.min.z, col.bounds.max.z)
        );
    }

    [SerializeField] EnemySpawnWave[] enemySpawnWaves;

    bool isActived = false;

    IEnumerator SpawnEnemy()
    {
        foreach(EnemySpawnWave wave in enemySpawnWaves)
        {
            Debug.Log("Spawning wave " + wave.enemy1Count + " " + wave.enemy2Count);
            for(int j = 0; j < wave.enemy1Count; j++)
            {
                Vector3 pos = GetRandomSpawnPosition(wave.spawnArea);
                var enemy = Instantiate(enemy1Prefab, pos, Quaternion.identity);
                enemy.GetComponent<NetworkObject>().Spawn();
                yield return new WaitForSeconds(wave.spawnDelay);
            }
            for(int j = 0; j < wave.enemy2Count; j++)
            {
                Vector3 pos = GetRandomSpawnPosition(wave.spawnArea);
                var enemy = Instantiate(enemy2Prefab, pos, Quaternion.identity);
                enemy.GetComponent<NetworkObject>().Spawn();
                yield return new WaitForSeconds(wave.spawnDelay);
            }
            yield return new WaitForSeconds(wave.spawnDelayAfterWave);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            if (other.tag == "Player" && !isActived)
            {
                isActived = true;
                StartCoroutine(SpawnEnemy());
            }
        }
    }
}
