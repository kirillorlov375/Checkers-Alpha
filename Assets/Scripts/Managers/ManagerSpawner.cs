using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSpawner : MonoBehaviour
{
    [SerializeField] GameObject localGameManagerPrefab;

    public void SpawnLocalGameManager()
    {
        Instantiate(localGameManagerPrefab);
    }
}
