using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("Cài đặt Pool")]
    public GameObject bulletPrefab;
    public int poolSize = 20; // Số lượng đạn khởi tạo sẵn trong kho

    private Queue<GameObject> bulletQueue = new Queue<GameObject>();

    void Awake()
    {
        // Thiết lập Singleton pattern đơn giản
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            // Gom chung vào một Parent trong Hierarchy cho gọn mắt
            bullet.transform.SetParent(transform);
            bulletQueue.Enqueue(bullet);
        }
    }

    // Lấy đạn ra từ Pool
    public GameObject GetBullet(Vector3 position, Quaternion rotation)
    {
        // Nếu kho hết đạn dự trữ, tự động sinh thêm để game không bị lỗi
        if (bulletQueue.Count == 0)
        {
            GameObject newBullet = Instantiate(bulletPrefab);
            newBullet.transform.SetParent(transform);
            return SetupBullet(newBullet, position, rotation);
        }

        GameObject bullet = bulletQueue.Dequeue();
        return SetupBullet(bullet, position, rotation);
    }

    GameObject SetupBullet(GameObject bullet, Vector3 position, Quaternion rotation)
    {
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.SetActive(true);
        return bullet;
    }

    // Trả đạn về kho khi không dùng nữa (trúng quái hoặc bay ra khỏi màn hình)
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletQueue.Enqueue(bullet);
    }
}