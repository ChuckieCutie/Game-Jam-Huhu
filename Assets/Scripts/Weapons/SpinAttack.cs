using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    // Các chỉ số này sẽ được truyền từ DirectionalWeapon
    public float damage;
    public float knockback;
    public float lifetime = 0.3f;

    void Start()
    {
        // Tự hủy sau một khoảng thời gian ngắn
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, knockback);
            }
        }
    }
}
