using UnityEngine;

public class ThrowWeaponPrefab : MonoBehaviour
{
    private ThrowWeapon weapon;
    private Rigidbody2D rb;
    private float angleOffset = 0f;
    private bool hasHitGround = false;
    private float gravityScale = 1f;
    private Vector2 initialVelocity;

    void Start()
    {
        weapon = GetComponentInParent<ThrowWeapon>();
        rb = GetComponent<Rigidbody2D>();

        WeaponStats currentStats = weapon.stats[weapon.weaponLevel];
        
        // Tính toán hướng ném từ player
        Vector2 throwDirection = PlayerController.Instance.lastMoveDirection;
        
        // Nếu không có hướng di chuyển, ném về phía trước
        if (throwDirection == Vector2.zero)
        {
            throwDirection = Vector2.up;
        }
        
        // Áp dụng góc lệch
        float angle = Mathf.Atan2(throwDirection.y, throwDirection.x) * Mathf.Rad2Deg + angleOffset;
        throwDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        
        // Thêm lực đẩy lên trên để tạo vòng cung bay
        throwDirection.y += 0.5f;
        throwDirection = throwDirection.normalized;
        
        // Áp dụng lực vào quả bóng
        initialVelocity = throwDirection * currentStats.speed;
        rb.velocity = initialVelocity;
        
        // Thiết lập gravity cho chuyển động parabol
        rb.gravityScale = gravityScale;
        
        // Tự hủy sau thời gian duration
        Destroy(gameObject, currentStats.duration);
    }

    void Update()
    {
        // Xoay object theo hướng bay để tạo hiệu ứng quay
        if (!hasHitGround && rb.velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void SetThrowAngle(float offset)
    {
        angleOffset = offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Gây sát thương
                enemy.TakeDamage(weapon.stats[weapon.weaponLevel].damage);
                
                // Có thể thêm hiệu ứng nảy lại hoặc tiếp tục bay
                CreateImpactEffect();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Xử lý va chạm với địa hình (nếu có)
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasHitGround = true;
            // Có thể làm nảy lại hoặc dừng lại
            HandleGroundHit(collision);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(weapon.stats[weapon.weaponLevel].damage);
                CreateImpactEffect();
            }
        }
    }

    private void HandleGroundHit(Collision2D collision)
    {
        // Tạo hiệu ứng nảy nhẹ
        Vector2 bounceDirection = Vector2.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
        rb.velocity = bounceDirection * rb.velocity.magnitude * 0.3f; // Giảm tốc độ sau khi nảy
        
        // Giảm gravity để không nảy quá nhiều
        rb.gravityScale *= 0.5f;
    }

    private void CreateImpactEffect()
    {
        // Có thể thêm particle effect hoặc animation khi va chạm
        // Instantiate impact effect here nếu có
    }
}