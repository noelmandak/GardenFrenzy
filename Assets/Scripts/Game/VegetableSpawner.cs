using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetableSpawner : MonoBehaviour
{
    public GameObject kentangPrefab;
    public GameObject wortelPrefab;
    public int jumlahKentang = 20; // Jumlah kentang yang akan di-spawn
    public int jumlahWortel = 20; // Jumlah wortel yang akan di-spawn

    private GameObject[] spawnedVegetables; // Array untuk menyimpan semua objek kentang dan wortel yang sudah di-spawn

    void Start()
    {
        SpawnVegetables();
    }

    void SpawnVegetables()
    {
        // Menentukan jumlah titik spawn secara acak di dalam persegi
        for (int i = 0; i < jumlahKentang + jumlahWortel; i++)
        {
            float randomX = Random.Range(-18f, 18f);
            float randomY = Random.Range(-18f, 18f);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            // Memilih prefab yang akan di-spawn
            bool isCarrot = (i < jumlahWortel);

            GameObject prefabToSpawn = isCarrot ? wortelPrefab : kentangPrefab;

            // Menyimpan objek yang telah di-spawn ke dalam variabel
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            // Menambahkan komponen Vegetable pada objek yang di-spawn
            Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
            vegetableComponent.isCarrot = isCarrot;
        }
    }

    public void RemoveAllVegetables()
    {
        // Menghapus semua objek kentang dan wortel yang sudah di-spawn
        spawnedVegetables = GameObject.FindGameObjectsWithTag("Kentang");
        foreach (GameObject kentang in spawnedVegetables)
        {
            Destroy(kentang);
        }

        spawnedVegetables = GameObject.FindGameObjectsWithTag("Wortel");
        foreach (GameObject wortel in spawnedVegetables)
        {
            Destroy(wortel);
        }
    }
}

