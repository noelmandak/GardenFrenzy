using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] powerUpPrefabs;
    private GameObject[] spawnedPowerUps = new GameObject[4];

    public void Init()
    {
        SpawnPowerUps();
    }

    void SpawnPowerUps()
    {
        for (int i = 0; i < 4; i++)
        {
            float randomX;
            float randomY;
            bool validPosition;
            do
            {
                randomX = Random.Range(-15f, 15f);
                randomY = Random.Range(-15f, 15f);
                validPosition = CheckPosition(randomX, randomY);
            } while (!validPosition);
            Vector3 spawnPosition = transform.TransformPoint(new Vector3(randomX, randomY, 0f));

            GameObject prefabToSpawn = powerUpPrefabs[i];
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            PowerUp powerUpComponent = spawnedObject.AddComponent<PowerUp>();
            powerUpComponent.powerUpType = i + 1; // 1 = red, 2 = blue, 3 = purple, 4 = yellow

            spawnedPowerUps[i] = spawnedObject;
        }
    }

    bool CheckPosition(float x, float y)
    {
        Vector2 spawnPosition = new Vector2(x, y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle")) return false;
            if (collider.CompareTag("Player")) return false;
        }
        return true;
    }

    //bool CheckPosition(float x, float y)
    //{
    //    if ((Mathf.Abs(x) < 4f && Mathf.Abs(y) < 1f) || (Mathf.Abs(x) > 12 && Mathf.Abs(y) > 12))
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    void RemoveAllPowerUps()
    {
        foreach (GameObject powerUp in spawnedPowerUps) Destroy(powerUp);
    }
    public void ResetPowerUps()
    {
        RemoveAllPowerUps();
        SpawnPowerUps();
    }
}

