using UnityEngine;

public class BounceBallPrefab : MonoBehaviour
{
    private BounceBallWeapon weapon;
    private Rigidbody2D rb;

    void Start()
    {
        // Tìm vũ khí cha để lấy chỉ số
        weapon = GetComponentInParent<BounceBallWeapon>();
        rb = GetComponent<Rigidbody2D>();

        WeaponStats currentStats = weapon.stats[weapon.weaponLevel];

        // Lấy hướng ném từ player
        Vector2 throwDirection = PlayerController.Instance.lastMoveDirection;
        
        // Thêm một lực đẩy lên trên để tạo vòng cung
        throwDirection.y += 1.5f; 

        // Áp dụng lực vào quả bóng
        rb.AddForce(throwDirection.normalized * currentStats.speed, ForceMode2D.Impulse);

        // Tự hủy quả bóng sau một khoảng thời gian
        Destroy(gameObject, currentStats.duration);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Gây sát thương
                enemy.TakeDamage(weapon.stats[weapon.weaponLevel].damage);
            }
        }
    }
}