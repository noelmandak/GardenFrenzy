using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class VegetableSpawner : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    private int _totalPotato = 0; // Jumlah potato yang akan di-spawn
    private int _totalCarrot = 0; // Jumlah carrot yang akan di-spawn

    private GameObject[] spawnedVegetables; // Array untuk menyimpan semua objek potato dan carrot yang sudah di-spawn

    public void Init(int totalPotato, int totalCarrot)
    {
        _totalPotato = totalPotato;
        _totalCarrot = totalCarrot;
        spawnedVegetables = new GameObject[totalPotato + totalCarrot];
        SpawnVegetables();
    }
    void SpawnVegetables()
    {
        for (int i = 0; i < (_totalPotato+_totalCarrot); i++)
        {
            Vector2 randomPosition = GetEmptyRandomPosition(-10, -10, 10, 10);
            Vector3 spawnPosition = transform.TransformPoint(new Vector3(randomPosition.x, randomPosition.y, 0f));

            bool isCarrot = (i < _totalCarrot);
            //bool isCarrot = Random.Range(1, 3) == 2; //random 1=potato 2=carrot

            GameObject prefabToSpawn = isCarrot ? carrotPrefab : potatoPrefab;
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
            vegetableComponent.isCarrot = isCarrot;
            spawnedVegetables[i] = spawnedObject;
        }
    }

    public void SpawnAVegetable(int vegetableType) //potato = 1 carrot =2
    {
        float randomX;
        float randomY;
        bool validPosition;
        do
        {
            randomX = Random.Range(-8f, 8f);
            randomY = Random.Range(-8f, 8f);
            validPosition = CheckPosition(randomX, randomY);
        } while (!validPosition);

        Vector3 spawnPosition = transform.TransformPoint(new Vector3(randomX, randomY, 0f));

        bool isCarrot = (vegetableType == 2);

        GameObject prefabToSpawn = isCarrot ? carrotPrefab : potatoPrefab;
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
        vegetableComponent.isCarrot = isCarrot;
        spawnedVegetables[0] = spawnedObject;
    }

    public void RemoveAVegetable()
    {
        if (spawnedVegetables[0]) Destroy(spawnedVegetables[0]);
    }

    Vector2 GetEmptyRandomPosition(float x1, float y1, float x2, float y2)
    {
        float randomX;
        float randomY;
        bool validPosition;
        do
        {
            randomX = Random.Range(x1, x2);
            randomY = Random.Range(y1, y2);
            validPosition = CheckPosition(randomX, randomY);
        } while (!validPosition);
        return new Vector2(randomX, randomY);
    }

    bool CheckPosition(float x, float y)
    {
        Vector2 spawnPosition = new Vector2(x, y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 2f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle")) return false;
        }
        return true;
    }


    void RemoveAllVegetables()
    {
        foreach (GameObject vegetable in spawnedVegetables) Destroy(vegetable);
    }
    public void ResetAllVegetables()
    {
        RemoveAllVegetables();
        SpawnVegetables();
    }

    public void ResetAllVegetables(int totalPotato, int totalCarrot)
    {
        RemoveAllVegetables();
        Init(totalPotato, totalCarrot);
    }
}

