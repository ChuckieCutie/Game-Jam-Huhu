using UnityEngine;

public class SlowTrapPrefab : MonoBehaviour
{
    private SlowTrapWeapon weapon;
    private float slowAmount; 
    private float slowDuration; 
    private float trapDuration; 

    void Start()
    {
        weapon = GetComponentInParent<SlowTrapWeapon>();
        WeaponStats currentStats = weapon.stats[weapon.weaponLevel];
        slowAmount = currentStats.damage; 
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
                enemy.ApplySlow(slowAmount, slowDuration);
            }
        }
    }
}