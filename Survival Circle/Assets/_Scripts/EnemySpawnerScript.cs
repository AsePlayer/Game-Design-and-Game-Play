using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{

    public Enemy enemy;
    float randx;
    float randy;
    Vector2 whereToSpawn;
    public float spawnRate = 2f;
    float nextSpawn = 0f;

    GameObject retryButton;

    // Start is called before the first frame update
    void Start()
    {
        retryButton = GameObject.Find("RetryButton");
        retryButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is dead, stop spawning enemies
        if(enemy.isPlayerDead())
        {
            retryButton.SetActive(true);
            return;
        }

        // Spawn enemies at random rates
        spawnRate -= Time.deltaTime;
        if (spawnRate < 0.0f)
        {
            SpawnEnemy(Random.Range(1, 3));
            spawnRate += Random.Range(0.5f, 2.5f);
        }

    }

    void SpawnEnemy(int enemies)
    {
        for (int i = 0; i < enemies; i++)
        {
            randx = Random.Range(-15, 15);
            randy = Random.Range(-15, 15);
            if (randx <= 10)
            {
                randy = 6;
            }
            else if (randx >= -10)
            {
                randy = -6;
            }
            else if (randy <= 6)
            {
                randx = 10;
            }
            else if (randy >= -6)
            {
                randx = -10;
            }
            whereToSpawn = new Vector2(randx, randy);
            Instantiate(enemy.gameObject, whereToSpawn, Quaternion.identity);
        }
    }

    // Reset scene
    public void ResetScene()
    {
        // Reload unity scene
        enemy.resetPlayerDead();
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

}
