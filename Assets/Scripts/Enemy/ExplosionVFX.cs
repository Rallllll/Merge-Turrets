using UnityEngine;

public class ExplosionVFX : MonoBehaviour
{
    public float lifeTime = 0.4f; // Thời gian nổ

    void OnEnable()
    {
        Invoke("ReturnToPool", lifeTime);
    }

    void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            ExplosionPool.Instance.ReturnExplosion(gameObject);
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}