using System.Collections;
using UnityEngine;

public enum EnemyType 
{ 
    Ground, 
    Flying, 
    Hybrid  
}
public class Enemy : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public EnemyType type;
    public int laneID;
    public float speed = 1.5f;
    public int maxHP = 100;
    private int currentHP;

    [Header("Hiệu ứng Trúng đạn")]
    public Color damageColor = Color.red; // Màu khi bị bắn
    public float flashDuration = 0.1f;    // Thời gian chớp đỏ (0.1 giây)
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("VFX")]
    public GameObject explosionPrefab;

    private Animator anim;
    private Collider2D col;
    
    private bool isMoving = true; 
    private bool isDead = false;

    public int goldReward = 5;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        
        // Lấy SpriteRenderer (Nếu bạn để Sprite ở Object con tên Visual thì dùng GetComponentInChildren)
        sr = GetComponent<SpriteRenderer>(); 
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        originalColor = sr.color; // Lưu lại màu gốc (thường là màu trắng)
    }

    void OnEnable()
    {
        // Reset lại các chỉ số mỗi khi quái được sinh ra (chuẩn bị cho Object Pool)
        currentHP = maxHP;
        isMoving = true;
        isDead = false;
        col.enabled = true;
        if (sr != null) sr.color = originalColor;
    }

    void Update()
    {
        if (isMoving && !isDead)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;

        // Gọi hiệu ứng chớp đỏ
        if (sr != null)
        {
            StartCoroutine(FlashRed());
        }

        if (DamageTextPool.Instance != null)
        {
            Vector3 textPos = transform.position + new Vector3(0, 0.5f, 0);
            DamageTextPool.Instance.SpawnDamageText(textPos, damage);
        }

        if (type == EnemyType.Hybrid && currentHP <= maxHP / 2 && isMoving)
        {
            TriggerTransformation();
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Coroutine xử lý chớp màu
    private IEnumerator FlashRed()
    {
        sr.color = damageColor; // Đổi sang màu đỏ
        yield return new WaitForSeconds(flashDuration); // Chờ 0.1s
        if (!isDead) sr.color = originalColor; // Trả về màu gốc
    }

    private void TriggerTransformation()
    {
        isMoving = false; 
        anim.SetTrigger("isTransforming"); 
        type = EnemyType.Flying; 
    }

    public void FinishTransformation()
    {
        isMoving = true; 
        speed *= 1.2f; 
    }

    private void Die()
    {
        isDead = true;

        // Gọi cục nổ từ Pool
        if (explosionPrefab != null)
        {
            ExplosionPool.Instance.GetExplosion(transform.position, Quaternion.identity);
        }

        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(goldReward);
        }

        // Trả quái về Pool thay vì Destroy
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BaseLine") && !isDead)
        {
            Debug.Log($"Quái lọt vào căn cứ ở Lane {laneID}! Trừ máu Căn cứ!");
            EnemyPool.Instance.ReturnEnemy(gameObject); // Trả về Pool
        }
    }
}