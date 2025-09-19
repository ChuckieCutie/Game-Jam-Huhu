using UnityEngine;

public class ThrowWeaponPrefab : MonoBehaviour
{
    private ThrowWeapon weapon;
    private Rigidbody2D rb;
    private bool wasOnBeat;

    public void Setup(ThrowWeapon weapon, bool isOnBeat)
    {
        this.weapon = weapon;
        this.wasOnBeat = isOnBeat;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        WeaponStats stats = weapon.stats[weapon.weaponLevel];
        Vector2 throwDirection = PlayerController.Instance.lastMoveDirection;

        if (throwDirection == Vector2.zero)
        {
            throwDirection = Vector2.up;
        }

        rb.velocity = throwDirection.normalized * stats.speed;
        rb.gravityScale = 0f;
        Destroy(gameObject, stats.duration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HandleHit(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleHit(collision.gameObject);
        }
    }

    private void HandleHit(GameObject target)
    {
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            WeaponStats stats = weapon.stats[weapon.weaponLevel];
            if (wasOnBeat)
            {
                enemy.TakeDamage(stats.boomDamage, stats.knockbackForce);
            }
            else
            {
                enemy.TakeDamage(stats.damage, stats.knockbackForce);
            }
        }
        Destroy(gameObject); // Hủy projectile sau khi va chạm
    }
}