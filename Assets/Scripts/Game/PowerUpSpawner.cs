using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    private GameObject[] spawnedPowerUps = new GameObject[4];

    void Start()
    {
        SpawnPowerUps();
    }

    void SpawnPowerUps()
    {
        // Menentukan jumlah titik spawn secara acak di dalam persegi
        for (int i = 0; i < 4; i++)
        {
            float randomX = Random.Range(-15f, 15f);
            float randomY = Random.Range(-15f, 15f);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            GameObject prefabToSpawn = powerUpPrefabs[i];
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            PowerUp powerUpComponent = spawnedObject.AddComponent<PowerUp>();
            powerUpComponent.powerUpType = i + 1; // 1 = red, 2 = blue, 3 = purple, 4 = yellow

            spawnedPowerUps[i] = spawnedObject;
        }
        Debug.Log("powerup spawned");
    }

    public void RemoveAllPowerUps()
    {
        foreach (GameObject powerUp in spawnedPowerUps)
        {
            Destroy(powerUp);
        }
        Debug.Log("powerup removed");
    }

    public void ResetPowerUps()
    {
        Debug.Log("powerup start reset");
        RemoveAllPowerUps();
        SpawnPowerUps();
        Debug.Log("powerup end reset");
    }
}

