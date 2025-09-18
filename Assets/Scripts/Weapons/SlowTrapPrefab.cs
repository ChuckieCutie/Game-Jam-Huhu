using UnityEngine;

public class SlowTrapPrefab : MonoBehaviour
{
    private SlowTrapWeapon weapon;
    private float slowAmount;
    private float slowDuration;
    private float trapDuration;
    private float damage;
    private float knockbackForce; // <<< THÊM MỚI

    public void SetWeapon(SlowTrapWeapon weapon)
    {
        this.weapon = weapon;
    }

    void Start()
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon reference not set on SlowTrapPrefab!");
            Destroy(gameObject);
            return;
        }

        WeaponStats currentStats = weapon.stats[weapon.weaponLevel];
        damage = currentStats.damage;
        slowAmount = currentStats.amount;
        slowDuration = currentStats.speed;
        trapDuration = currentStats.duration;
        knockbackForce = currentStats.knockbackForce; // <<< THÊM MỚI

        Destroy(gameObject, trapDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Gây sát thương và hiệu ứng
                // <<< THAY ĐỔI: Truyền thêm knockbackForce
                enemy.TakeDamage(damage, knockbackForce);
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }
    }
}