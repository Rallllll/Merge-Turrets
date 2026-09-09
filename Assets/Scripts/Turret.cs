using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Thông số bắn")]
    [HideInInspector] public int laneID; // Sẽ dùng sau khi kết hợp với hệ thống Grid
    public float fireRate = 1f; // Bắn 1 phát / giây
    public float attackRange = 10f; // Tầm quét quái (Độ dài tia laser)

    [Header("Tham chiếu")]
    public GameObject bulletPrefab;   // Kéo Prefab Đạn vào đây
    public GameObject shootVFXPrefab; // Kéo Prefab hiệu ứng lóe nòng súng (nếu có)
    public Transform firePoint;       // Kéo cái FirePoint vào đây

    private Animator anim;
    private float fireTimer;

    [Header("Merge Info")]
    public int turretLevel = 1; // Súng cấp 1
    public Slot currentSlot;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Nếu súng đang ở MergeZone (ID = -1) thì không làm gì cả, return luôn.
        if (laneID == -1) return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            // Kiểm tra raycast xem có quái không
            if (CheckEnemyInLane())
            {
                Shoot();
                fireTimer = 1f / fireRate;
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
        // THÊM DÒNG NÀY: Nếu ID là -1 thì không sinh đạn/VFX gì hết, cấm tuyệt đối!
        if (laneID == -1) return;

        if (VfxPool.Instance != null && shootVFXPrefab != null)
        {
            VfxPool.Instance.GetVfx(firePoint.position, Quaternion.identity);
        }

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