using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Thông số bắn")]
    [HideInInspector] public int laneID;
    public float fireRate = 1f; // Bắn 1 phát / giây
    public float attackRange = 10f; // Tầm quét quái

    [Header("Tham chiếu")]
    public GameObject bulletPrefab;     // Kéo Prefab Đạn vào đây
    public GameObject shootVFXPrefab; // Kéo Prefab hiệu ứng lóe nòng súng

    [Header("Nhiều nòng súng (Fire Points)")]
    public Transform[] firePoints;    // <--- Đã đổi thành Mảng (Array) để chứa n vị trí bắn!

    private Animator anim;
    private float fireTimer;

    [Header("Merge Info")]
    public int turretLevel = 1;
    public Slot currentSlot;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (laneID == -1) return;

        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            // Kiểm tra raycast (Dùng firePoints[0] làm mốc dò quái chính)
            if (CheckEnemyInLane())
            {
                Shoot();
                fireTimer = 1f / fireRate;
            }
        }
    }

    bool CheckEnemyInLane()
    {
        if (firePoints == null || firePoints.Length == 0 || firePoints[0] == null) return false;

        // Bắn tia Raycast từ nòng đầu tiên lên trên
        RaycastHit2D hit = Physics2D.Raycast(firePoints[0].position, Vector2.up, attackRange, LayerMask.GetMask("Enemy"));

        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    void Shoot()
    {
        anim.SetTrigger("isShooting");
    }

    // [GẮN ANIMATION EVENT CỦA TURRET]
    // Hàm này sẽ tự động lặp qua TẤT CẢ các vị trí nòng súng để nhả đạn và VFX đồng loạt
    public void SpawnBulletAndVFX()
    {
        if (laneID == -1) return;
        if (firePoints == null || firePoints.Length == 0) return;

        // Vòng lặp foreach duyệt qua từng nòng súng trong mảng
        foreach (Transform fp in firePoints)
        {
            if (fp == null) continue;

            // Sinh VFX tại từng nòng
            if (VfxPool.Instance != null && shootVFXPrefab != null)
            {
                VfxPool.Instance.GetVfx(fp.position, Quaternion.identity);
            }

            // Sinh đạn tại từng nòng
            if (bulletPrefab != null)
            {
                BulletPool.Instance.GetBullet(fp.position, Quaternion.identity);
            }
        }
    }

    // Vẽ tia laser kiểm tra tầm nhìn cho tất cả các nòng trong Scene
    private void OnDrawGizmosSelected()
    {
        if (firePoints == null) return;

        foreach (Transform fp in firePoints)
        {
            if (fp != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(fp.position, Vector2.up * attackRange);
            }
        }
    }
}