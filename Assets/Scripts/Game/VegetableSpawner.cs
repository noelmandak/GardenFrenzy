using System.Collections;
using System.Collections.Generic;
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
        SpawnVegetables();
    }

    void SpawnVegetables()
    {
        for (int i = 0; i < _totalPotato + _totalCarot; i++)
        {
            float randomX = Random.Range(-15f, 15f);
            float randomY = Random.Range(-15f, 15f);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            bool isCarrot = (i < _totalCarot);

            GameObject prefabToSpawn = isCarrot ? carotPrefab : potatoPrefab;
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
            vegetableComponent.isCarrot = isCarrot;
        }
    }

    public void RemoveAllVegetables()
    {
        spawnedVegetables = GameObject.FindGameObjectsWithTag("Potato");
        foreach (GameObject potato in spawnedVegetables)
        {
            Destroy(potato);
        }

        spawnedVegetables = GameObject.FindGameObjectsWithTag("Carot");
        foreach (GameObject carot in spawnedVegetables)
        {
            Destroy(carot);
        }
    }
}

