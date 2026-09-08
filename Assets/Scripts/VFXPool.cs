using System.Collections.Generic;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    public static VfxPool Instance;

    [Header("Cài đặt Pool VFX")]
    public GameObject vfxPrefab; // Kéo Prefab VFX nhả đạn vào đây
    public int poolSize = 20;

    private Queue<GameObject> vfxQueue = new Queue<GameObject>();

    void Awake()
    {
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
            GameObject vfx = Instantiate(vfxPrefab);
            vfx.SetActive(false);
            vfx.transform.SetParent(transform); // Gom cho gọn Hierarchy
            vfxQueue.Enqueue(vfx);
        }
    }

    public GameObject GetVfx(Vector3 position, Quaternion rotation)
    {
        if (vfxQueue.Count == 0)
        {
            GameObject newVfx = Instantiate(vfxPrefab);
            newVfx.transform.SetParent(transform);
            return SetupVfx(newVfx, position, rotation);
        }

        GameObject vfx = vfxQueue.Dequeue();
        return SetupVfx(vfx, position, rotation);
    }

    GameObject SetupVfx(GameObject vfx, Vector3 position, Quaternion rotation)
    {
        vfx.transform.position = position;
        vfx.transform.rotation = rotation;
        vfx.SetActive(true);
        return vfx;
    }

    public void ReturnVfx(GameObject vfx)
    {
        vfx.SetActive(false);
        vfxQueue.Enqueue(vfx);
    }
}