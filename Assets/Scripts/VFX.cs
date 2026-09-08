using UnityEngine;

public class VFX : MonoBehaviour
{
    // [GẮN ANIMATION EVENT]
    // Mở clip animation của cái VFX này lên, đi tới frame cuối cùng
    // Add Animation Event và chọn hàm OnVFXFinished() này.
    public void OnVFXFinished()
    {
        // Kiểm tra an toàn trước khi trả về Pool
        if (gameObject.activeInHierarchy)
        {
            VfxPool.Instance.ReturnVfx(gameObject);
        }
    }
}