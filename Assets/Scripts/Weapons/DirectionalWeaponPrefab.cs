using UnityEngine;

public class DirectionalWeaponPrefab : MonoBehaviour
{
    private DirectionalWeapon weapon;
    private Rigidbody2D rb;
    private Vector3 direction;
    private float duration;

    void Start()
    {
        weapon = GameObject.Find("Directional Weapon").GetComponent<DirectionalWeapon>();
        rb = GetComponent<Rigidbody2D>();
        
        direction = PlayerController.Instance.lastMoveDirection;
        duration = weapon.stats[weapon.weaponLevel].duration;

        float randomAngle = Random.Range(-0.2f, 0.2f);
        rb.velocity = new Vector3(
            direction.x * weapon.stats[weapon.weaponLevel].speed + randomAngle,
            direction.y * weapon.stats[weapon.weaponLevel].speed + randomAngle);

        AudioController.Instance.PlaySound(AudioController.Instance.directionalWeaponSpawn);
    }

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            // Hiệu ứng co lại trước khi biến mất
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * 5);
            if (transform.localScale.x == 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    // <<< SỬA LỖI 4: Đảm bảo code trong hàm này đúng thứ tự
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Khai báo 'enemy' trước khi sử dụng
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                WeaponStats stats = weapon.stats[weapon.weaponLevel];
                enemy.TakeDamage(stats.damage, stats.knockbackForce);
            }
            
            AudioController.Instance.PlaySound(AudioController.Instance.directionalWeaponHit);
        }
    }
}