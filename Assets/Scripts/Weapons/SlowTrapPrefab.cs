using UnityEngine;

public class SlowTrapPrefab : MonoBehaviour
{
    private SlowTrapWeapon weapon;
    private float slowAmount;
    private float slowDuration;
    private float trapDuration;
    
    // Các chỉ số
    private float normalDamage;
    private float boomDamage;
    private float knockbackForce;
    private bool wasOnBeat;

    public void SetWeapon(SlowTrapWeapon weapon)
    {
        this.weapon = weapon;
    }

    public void Setup(bool isOnBeat)
    {
        wasOnBeat = isOnBeat;
    }

    void Start()
    {
        if (weapon == null)
        {
            Destroy(gameObject);
            return;
        }

        WeaponStats stats = weapon.stats[weapon.weaponLevel];
        slowAmount = stats.amount;
        slowDuration = stats.speed;
        trapDuration = stats.duration;
        normalDamage = stats.damage;
        boomDamage = stats.boomDamage;
        knockbackForce = stats.knockbackForce;

        Destroy(gameObject, trapDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if(wasOnBeat)
                {
                    // ĐÚNG NHỊP: Gây choáng và Boom Damage
                    enemy.Stun(1.5f); // <<< THAY ĐỔI LỚN
                    enemy.TakeDamage(boomDamage, knockbackForce);
                }
                else
                {
                    // TRƯỢT NHỊP: Gây sát thương và làm chậm như cũ
                    enemy.TakeDamage(normalDamage, knockbackForce);
                    enemy.ApplySlow(slowAmount, slowDuration);
                }
            }
        }
    }
}