using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 25;

    [Header("Giới hạn tầm bắn (Khi chưa có Enemy)")]
    public float maxTravelDistance = 15f; // Bay quá 15 đơn vị là tự động thu hồi
    private Vector3 startPosition;

    void OnEnable()
    {
        // Lưu lại vị trí xuất phát mỗi khi đạn được lấy ra từ Pool
        startPosition = transform.position;
    }

    void Update()
    {
        // Đạn bay thẳng lên trên
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // [MẸO TEST]: Nếu bay quá khoảng cách cho phép mà chưa trúng gì thì tự động trả về Pool
        // Tránh tình trạng bắn vài viên là hết đạn trong kho khi chưa có quái.
        if (Vector3.Distance(startPosition, transform.position) >= maxTravelDistance)
        {
            ReturnToPool();
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
       // Enemy enemy = collision.GetComponent<Enemy>();
       // if (enemy != null)
       // {
        //    enemy.TakeDamage(damage);
         //   ReturnToPool(); // Trúng quái thì trả về Pool
       // }
    //}

    void ReturnToPool()
    {
        // Kiểm tra an toàn trước khi trả về để tránh lỗi object bị hủy
        if (gameObject.activeInHierarchy)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
    }
}