using UnityEngine;

public class SlowTrapPrefab : MonoBehaviour
{
    private SlowTrapWeapon weapon;
    private float slowAmount; 
    private float slowDuration; 
    private float trapDuration; 
    private float damage; // Biến mới để lưu sát thương

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

        // Gán chỉ số 'damage' cho sát thương của bẫy
        damage = currentStats.damage; 
        
        // Dùng chỉ số 'amount' để làm mức độ làm chậm
        slowAmount = currentStats.amount; 
        
        slowDuration = currentStats.speed; 
        trapDuration = currentStats.duration; 

        Destroy(gameObject, trapDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Gây sát thương cho kẻ địch
                enemy.TakeDamage(damage);
                
                // Áp dụng hiệu ứng làm chậm
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }
    }
}