using UnityEngine;

// Tên file: MicSpinProjectile.cs
public class MicSpinProjectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private float knockback; // <<< THÊM MỚI
    private Vector3 direction;
    private float lifetime = 1f;

    void Start()
    {
        // Tìm vũ khí MicSpinWeapon trong các vũ khí đang có của người chơi để lấy chỉ số
        foreach (var weapon in PlayerController.Instance.activeWeapons)
        {
            if (weapon is MicSpinWeapon micSpinWeapon)
            {
                WeaponStats stats = micSpinWeapon.stats[micSpinWeapon.weaponLevel];
                damage = stats.damage;
                speed = stats.speed;
                knockback = stats.knockbackForce; // <<< THÊM MỚI
                break;
            }
        }
        
        // Hẹn giờ tự hủy sau lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Di chuyển viên đạn theo hướng đã định
        transform.position += direction * speed * Time.deltaTime;
    }

    // Hàm này được gọi từ MicSpinWeapon.cs để set hướng bay
    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        // Xoay viên đạn theo hướng di chuyển
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Xử lý va chạm với kẻ địch
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // <<< THAY ĐỔI: Truyền thêm knockback
                enemy.TakeDamage(damage, knockback);
            }
            // Viên đạn bay xuyên qua kẻ địch
        }
    }
}