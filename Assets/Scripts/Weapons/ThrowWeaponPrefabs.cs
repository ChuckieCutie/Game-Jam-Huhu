using UnityEngine;

public class ThrowWeaponPrefab : MonoBehaviour
{
    private ThrowWeapon weapon;
    private Rigidbody2D rb;

    void Start()
    {
        weapon = GetComponentInParent<ThrowWeapon>();
        rb = GetComponent<Rigidbody2D>();
        
        WeaponStats currentStats = weapon.stats[weapon.weaponLevel];
        Vector2 throwDirection = PlayerController.Instance.lastMoveDirection;

        if (throwDirection == Vector2.zero)
        {
            throwDirection = Vector2.up; // Ném lên trên nếu đứng yên
        }

        rb.velocity = throwDirection.normalized * currentStats.speed;
        rb.gravityScale = 0f;
        Destroy(gameObject, currentStats.duration);
    }

    void Update()
    {
        // Xoay object theo hướng bay
        if (rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    
    // Sửa cả 2 hàm va chạm dưới đây

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                WeaponStats stats = weapon.stats[weapon.weaponLevel];
                // <<< THAY ĐỔI: Truyền thêm knockbackForce
                enemy.TakeDamage(stats.damage, stats.knockbackForce);
            }
            // Có thể hủy projectile ở đây nếu muốn
            // Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                WeaponStats stats = weapon.stats[weapon.weaponLevel];
                // <<< THAY ĐỔI: Truyền thêm knockbackForce
                enemy.TakeDamage(stats.damage, stats.knockbackForce);
            }
            // Có thể hủy projectile ở đây nếu muốn
            // Destroy(gameObject);
        }
    }
}