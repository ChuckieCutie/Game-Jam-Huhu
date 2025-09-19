using System.Collections;
using UnityEngine;

public class MicSpinProjectile : MonoBehaviour
{
    [Header("Boom Effect")]
    [Tooltip("Prefab của vụ nổ sẽ được tạo ra dọc đường bay")]
    [SerializeField] private GameObject explosionPrefab;
    [Tooltip("Khoảng thời gian giữa các vụ nổ")]
    [SerializeField] private float explosionInterval = 0.15f;

    // Các chỉ số được truyền vào
    private float speed;
    private Vector3 direction;
    private float lifetime = 1f;
    private float normalDamage;
    private float boomDamage;
    private float knockback;
    private bool wasOnBeat;

    public void Setup(MicSpinWeapon weapon, bool isOnBeat)
    {
        this.wasOnBeat = isOnBeat;
        WeaponStats stats = weapon.stats[weapon.weaponLevel];
        speed = stats.speed;
        normalDamage = stats.damage;
        boomDamage = stats.boomDamage;
        knockback = stats.knockbackForce;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    
    // <<< HÀM MỚI: Được gọi từ PlayerController
    public void TriggerBoomEffect()
    {
        Debug.Log("Mic Spin Boom Activated!");
        // Không gây sát thương khi va chạm nữa
        GetComponent<Collider2D>().enabled = false;
        // Bắt đầu chuỗi nổ
        StartCoroutine(BoomEffectRoutine());
    }

    private IEnumerator BoomEffectRoutine()
    {
        // Vòng lặp để tạo ra các vụ nổ cho đến hết thời gian tồn tại
        while(true)
        {
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                // Gây sát thương tại vị trí vụ nổ
                DealExplosionDamage();
            }
            yield return new WaitForSeconds(explosionInterval);
        }
    }

    private void DealExplosionDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 1.5f); // Bán kính vụ nổ
        foreach (var enemyCollider in enemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Vụ nổ luôn gây boom damage
                    enemy.TakeDamage(boomDamage, knockback);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Gây sát thương bình thường nếu không kích hoạt boom
                enemy.TakeDamage(normalDamage, knockback);
            }
        }
    }
    
    void OnDisable()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.activeMicProjectile == this)
        {
            PlayerController.Instance.activeMicProjectile = null;
        }
    }
}