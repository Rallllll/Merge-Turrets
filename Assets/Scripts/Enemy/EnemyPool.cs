using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    // Dùng Dictionary để phân loại nhiều Prefab quái khác nhau
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject GetEnemy(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;

        // Nếu kho chưa có loại quái này, tạo một hàng đợi mới cho nó
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        if (poolDictionary[key].Count == 0)
        {
            GameObject newEnemy = Instantiate(prefab, position, rotation);
            newEnemy.name = key; // Đặt lại tên xóa chữ (Clone) để lúc trả về kho không bị lỗi
            newEnemy.transform.SetParent(transform);
            return newEnemy;
        }
        else
        {
            GameObject enemy = poolDictionary[key].Dequeue();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
            return enemy;
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        // Nhét lại vào đúng kho dựa theo tên
        poolDictionary[enemy.name].Enqueue(enemy);
    }
}