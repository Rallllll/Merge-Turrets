using System.Collections.Generic;
using UnityEngine;

public class ExplosionPool : MonoBehaviour
{
    public static ExplosionPool Instance;
    public GameObject explosionPrefab;
    public int poolSize = 20;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject exp = Instantiate(explosionPrefab);
            exp.SetActive(false);
            exp.transform.SetParent(transform);
            poolQueue.Enqueue(exp);
        }
    }

    public GameObject GetExplosion(Vector3 position, Quaternion rotation)
    {
        if (poolQueue.Count == 0)
        {
            GameObject newExp = Instantiate(explosionPrefab);
            newExp.transform.SetParent(transform);
            newExp.transform.position = position;
            newExp.transform.rotation = rotation;
            newExp.SetActive(true);
            return newExp;
        }

        GameObject exp = poolQueue.Dequeue();
        exp.transform.position = position;
        exp.transform.rotation = rotation;
        exp.SetActive(true);
        return exp;
    }

    public void ReturnExplosion(GameObject exp)
    {
        exp.SetActive(false);
        poolQueue.Enqueue(exp);
    }
}