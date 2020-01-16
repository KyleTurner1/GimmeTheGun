﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject EnemyTarget;

    public List<(GameObject, int)> Wave;

    private BoxCollider2D SpawnArea;
    private float MinX;
    private float MaxX;
    private float MinY;
    private float MaxY;

    private float time = 0;

    public void Start()
    {
        SpawnArea = GetComponent<BoxCollider2D>();
        MinX = transform.position.x + SpawnArea.offset.x - SpawnArea.size.x / 2;
        MinY = transform.position.y + SpawnArea.offset.y - SpawnArea.size.y / 2;
        MaxX = transform.position.x + SpawnArea.offset.x + SpawnArea.size.x / 2;
        MaxY = transform.position.y + SpawnArea.offset.y + SpawnArea.size.y / 2;
        Debug.Log(string.Format("MinX: {0}. MaxX: {1}. MinY: {2}. MaxY: {3}", MinX, MaxX, MinY, MaxY));
    }

    public void Update()
    {
        if (Wave != null)
        {
            time += Time.deltaTime;
            if (time < 1)
            {
                SpawnWave();
                time += 1;
            }
        }
    }

    public void SpawnWave()
    {
        foreach ((GameObject, int) set in Wave)
        {
            Spawn(set.Item1, set.Item2);
        }
    }

    public GameObject[] Spawn(GameObject enemy, int amount)
    {
        GameObject[] ret = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            GameObject SpawnedEnemy = Instantiate(enemy, new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY)), Quaternion.identity);
            SpawnedEnemy.GetComponent<Enemy>().Target = EnemyTarget;
            ret[i] = SpawnedEnemy;
        }

        return ret;
    }
}
