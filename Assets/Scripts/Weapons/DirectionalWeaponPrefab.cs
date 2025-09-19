using UnityEngine;

public class DirectionalWeaponPrefab : MonoBehaviour
{
    private DirectionalWeapon weapon;
    private Rigidbody2D rb;
    private float duration;
    private bool wasOnBeat = false;
    private bool hasSplit = false; // Thêm biến cờ để đảm bảo chỉ tách 1 lần

    public void Setup(bool isOnBeat)
    {
        wasOnBeat = isOnBeat;
    }

    void Start()
    {
        weapon = GameObject.Find("Directional Weapon").GetComponent<DirectionalWeapon>();
        rb = GetComponent<Rigidbody2D>();
        Vector3 direction = PlayerController.Instance.lastMoveDirection;
        duration = weapon.stats[weapon.weaponLevel].duration;

        float randomAngle = Random.Range(-0.2f, 0.2f);
        rb.velocity = new Vector2(
            direction.x * weapon.stats[weapon.weaponLevel].speed + randomAngle,
            direction.y * weapon.stats[weapon.weaponLevel].speed + randomAngle);

        AudioController.Instance.PlaySound(AudioController.Instance.directionalWeaponSpawn);
        Destroy(gameObject, duration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                WeaponStats stats = weapon.stats[weapon.weaponLevel];
                if (wasOnBeat)
                {
                    enemy.TakeDamage(stats.boomDamage, stats.knockbackForce);
                    // Kích hoạt hiệu ứng tách đạn
                    SplitShot();
                }
                else
                {
                    enemy.TakeDamage(stats.damage, stats.knockbackForce);
                }
            }
            AudioController.Instance.PlaySound(AudioController.Instance.directionalWeaponHit);
            Destroy(gameObject); // Hủy đạn gốc khi va chạm
        }
    }

    // <<< HÀM MỚI ĐỂ XỬ LÝ TÁCH ĐẠN
    private void SplitShot()
    {
        if (hasSplit) return; // Nếu đã tách rồi thì không làm gì nữa
        hasSplit = true;

        Debug.Log("Melody Shot: Splitting!");
        WeaponStats stats = weapon.stats[weapon.weaponLevel];
        
        // Lấy hướng của viên đạn gốc
        Vector2 originalDirection = rb.velocity.normalized;

        // Tạo 2 hướng mới, lệch 30 độ so với hướng gốc
        Vector2 dir1 = Quaternion.Euler(0, 0, 30) * originalDirection;
        Vector2 dir2 = Quaternion.Euler(0, 0, -30) * originalDirection;

        // Tạo 2 viên đạn con
        // Lưu ý: Chúng ta đang tạo lại chính prefab này, nhưng không kích hoạt hiệu ứng tách đạn cho chúng
        // Cách tốt hơn là tạo một prefab "đạn con" riêng, nhưng cách này nhanh và đơn giản hơn cho Game Jam
        CreateSplitProjectile(dir1, stats);
        CreateSplitProjectile(dir2, stats);
    }

    private void CreateSplitProjectile(Vector2 direction, WeaponStats stats)
    {
        // Lấy prefab gốc từ script weapon chính
        GameObject projectileObj = Instantiate(weapon.GetComponent<DirectionalWeapon>().normalAttackPrefab, transform.position, Quaternion.identity);
        
        // Tắt hiệu ứng "on beat" cho đạn con
        projectileObj.GetComponent<DirectionalWeaponPrefab>().Setup(false); 
        
        // Gán tốc độ và hướng bay
        Rigidbody2D childRb = projectileObj.GetComponent<Rigidbody2D>();
        childRb.velocity = direction * stats.speed;
        
        // Sát thương của đạn con có thể giảm đi một nửa
        // (Để làm được điều này cần nâng cấp code phức tạp hơn, tạm thời chúng gây sát thương bằng đòn đánh thường)
    }
}