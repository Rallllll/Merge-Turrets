using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Thông số bắn")]
    public int laneID; // Sẽ dùng sau khi kết hợp với hệ thống Grid
    public float fireRate = 1f; // Bắn 1 phát / giây
    public float attackRange = 10f; // Tầm quét quái (Độ dài tia laser)

    [Header("Tham chiếu")]
    public GameObject bulletPrefab;   // Kéo Prefab Đạn vào đây
    public GameObject shootVFXPrefab; // Kéo Prefab hiệu ứng lóe nòng súng (nếu có)
    public Transform firePoint;       // Kéo cái FirePoint vào đây

    private Animator anim;
    private float fireTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        // Nếu súng đã sẵn sàng bắn
        if (fireTimer <= 0f)
        {
            // Kiểm tra xem trên đường đạn bay (làn của súng này) có quái không
            if (CheckEnemyInLane())
            {
                Shoot();
                fireTimer = 1f / fireRate; // Đặt lại thời gian chờ
            }
        }
    }

    bool CheckEnemyInLane()
    {
        // Bắn một tia Raycast vô hình thẳng lên trên
        // Chỉ quét những vật thể thuộc Layer "Enemy"
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, Vector2.up, attackRange, LayerMask.GetMask("Enemy"));

        // Nếu tia này chạm vào cái gì đó -> Có quái
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    void Shoot()
    {
        // Gọi Animation bắn
        anim.SetTrigger("isShooting");
    }

    // [GẮN ANIMATION EVENT CỦA TURRET]
    // Mở Animation clip bắn của Turret, đến frame lóe sáng, Add Event gọi hàm này
    public void SpawnBulletAndVFX()
    {
        // 1. Lấy VFX từ Pool
        if (VfxPool.Instance != null)
        {
            VfxPool.Instance.GetVfx(firePoint.position, Quaternion.identity);
        }

        // 2. Lấy Đạn từ Pool
        if (bulletPrefab != null)
        {
            BulletPool.Instance.GetBullet(firePoint.position, Quaternion.identity);
        }
    }

    // Hàm này giúp bạn vẽ một tia laser màu đỏ trong màn hình Scene để dễ căn chỉnh tầm nhìn
    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(firePoint.position, Vector2.up * attackRange);
        }
    }
}