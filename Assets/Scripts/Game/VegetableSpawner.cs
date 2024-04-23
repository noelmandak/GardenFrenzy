using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class VegetableSpawner : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carotPrefab;
    private int _totalPotato = 0; // Jumlah potato yang akan di-spawn
    private int _totalCarot = 0; // Jumlah carot yang akan di-spawn

    private GameObject[] spawnedVegetables; // Array untuk menyimpan semua objek potato dan carot yang sudah di-spawn

    public void Init(int totalPotato, int totalCarot)
    {
        _totalPotato = totalPotato;
        _totalCarot = totalCarot;
        spawnedVegetables = new GameObject[totalPotato + totalCarot];
        SpawnVegetables();
    }
    void SpawnVegetables()
    {
        for (int i = 0; i < _totalPotato + _totalCarot; i++)
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

            bool isCarrot = (i < _totalCarot);

            GameObject prefabToSpawn = isCarrot ? carotPrefab : potatoPrefab;
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
            vegetableComponent.isCarrot = isCarrot;
            spawnedVegetables[i] = spawnedObject;
        }
    }

    bool CheckPosition(float x, float y)
    {
        Vector2 spawnPosition = new Vector2(x, y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle")) return false;
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

    void RemoveAllVegetables()
    {
        foreach (GameObject vegetable in spawnedVegetables) Destroy(vegetable);
    }
    public void ResetAllVegetables()
    {
        RemoveAllVegetables();
        SpawnVegetables();
    }
}

