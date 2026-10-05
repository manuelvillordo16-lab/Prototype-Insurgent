using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn settings")]
    public GameObject enemyPrefabToSpawn;
    public Transform player;
    public float detectionRange = 10f;

    private bool hasSpawned = false;

    public void SpawnThreeEnemies()
    {
        if (hasSpawned) return;
        {
            if (enemyPrefabToSpawn != null)
            {

                for (int i = 0; i < 3; i++)
                {

                    Vector3 randomOffset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                    Vector3 spawnPosition = transform.position + randomOffset;


                    GameObject newEnemy = Instantiate(enemyPrefabToSpawn, spawnPosition, Quaternion.identity);

                    EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();

                    if (enemyAI != null && player != null)
                    {

                        enemyAI.player = player;

                    }
                }

            }

            Debug.Log("Exactly 3 enemies have spawned upon detection!");
            hasSpawned = true;


        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

    }
}
