using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyPair
    {
        public EnemyTypes type;
        public GameObject entity;
    }

    [System.Serializable]
    public enum EnemyTypes { LilacRocker = 0, DiscoSteve = 1 }

    public EnemyPair[] enemies;

    Dictionary<EnemyTypes, GameObject> enemy_prefabs;
    Dictionary<EnemyTypes,Stack<GameObject>> stored;

    private void Start()
    {
        stored = new Dictionary<EnemyTypes, Stack<GameObject>>();
        enemy_prefabs = new Dictionary<EnemyTypes, GameObject>();

        foreach (EnemyPair pair in enemies)
        {
            enemy_prefabs[pair.type] = pair.entity;
            stored[pair.type] = new Stack<GameObject>();
        }
    }

    public GameObject Get(EnemyTypes type)
    {
        if (stored[type].Count > 0) return stored[type].Pop();
        else
        {
            GameObject go = (GameObject)Instantiate(enemy_prefabs[type]);
            go.SetActive(false);
            go.GetComponent<Enemy>().enemy_type = (int)type;
            return go;
        }
    }

    public void Store(GameObject enemy)
    { 
        stored[(EnemyTypes)enemy.GetComponent<Enemy>().enemy_type].Push(enemy);
    }
}
