using UnityEngine;

public class VFX : MonoBehaviour
{
    [Header("Thoi gian song cua VFX")]
    public float lifeTime = 0.3f; // Chỉnh thời gian này khớp với độ dài hiệu ứng (0.3s - 0.5s)

    // Hàm OnEnable tự chạy mỗi khi VFX được lấy ra từ Pool
    void OnEnable()
    {
        // Hẹn giờ chuẩn xác: Sau khoảng thời gian 'lifeTime' sẽ tự động cất về kho
        Invoke("ReturnToPool", lifeTime);
    }

    void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            VfxPool.Instance.ReturnVfx(gameObject);
        }
    }

    void OnDisable()
    {
        // Hủy lịch hẹn giờ nếu object bị tắt đột ngột
        CancelInvoke();
    }
}