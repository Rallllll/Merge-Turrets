using System.Collections.Generic;
using UnityEngine;

public class DamageTextPool : MonoBehaviour
{
    public static DamageTextPool Instance;
    public GameObject damageTextPrefab;
    public int poolSize = 30;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public void SpawnDamageText(Vector3 position, int damageAmount)
    {
        GameObject obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : Instantiate(damageTextPrefab, transform);
        obj.transform.position = position;
        obj.SetActive(true);
        obj.GetComponent<DamageText>().Setup(damageAmount);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}