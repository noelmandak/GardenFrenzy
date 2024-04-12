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
        for (int i = 0; i < 1; i++)
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

            //bool isCarrot = (i < _totalCarot);
            bool isCarrot = Random.Range(1, 3) == 2; //random 1=potato 2=carot

            GameObject prefabToSpawn = isCarrot ? carotPrefab : potatoPrefab;
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
            vegetableComponent.isCarrot = isCarrot;
            spawnedVegetables[i] = spawnedObject;
        }
    }

    public void SpawnAVegetable(int vegetableType) //potato = 1 carot =2
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

        GameObject prefabToSpawn = isCarrot ? carotPrefab : potatoPrefab;
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Vegetable vegetableComponent = spawnedObject.AddComponent<Vegetable>();
        vegetableComponent.isCarrot = isCarrot;
        spawnedVegetables[0] = spawnedObject;
    }

    public void RemoveAVegetable()
    {
        if (spawnedVegetables[0]) Destroy(spawnedVegetables[0]);
    }

    bool CheckPosition(float x, float y)
    {
        //if ((Mathf.Abs(x) < 4f && Mathf.Abs(y) < 1f) || (Mathf.Abs(x) > 12 && Mathf.Abs(y) > 12))
        //{
        //    return false;
        //}
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
}

