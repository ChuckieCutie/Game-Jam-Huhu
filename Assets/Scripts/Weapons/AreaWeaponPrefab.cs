using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWeaponPrefab : MonoBehaviour
{
    private AreaWeapon weapon;
    private Vector3 targetSize;
    private float timer;
    public List<Enemy> enemiesInRange = new List<Enemy>();
    private float counter;
    private bool wasOnBeat = false;

    public void Setup(bool isOnBeat)
    {
        wasOnBeat = isOnBeat;
    }

    void Start()
    {
        weapon = GetComponentInParent<AreaWeapon>();
        WeaponStats stats = weapon.stats[weapon.weaponLevel];
        targetSize = Vector3.one * stats.range;
        transform.localScale = Vector3.zero;
        timer = stats.duration;
        AudioController.Instance.PlaySound(AudioController.Instance.areaWeaponSpawn);

        // Nếu đúng nhịp, khởi động bộ đếm cho vụ nổ thứ hai
        if (wasOnBeat)
        {
            StartCoroutine(SecondExplosionRoutine());
        }
    }

    // Coroutine cho vụ nổ thứ hai
    private IEnumerator SecondExplosionRoutine()
    {
        // Lấy khoảng thời gian của 1 nhịp từ RhythmManager
        float beatInterval = 60f / RhythmManager.Instance.bpm;
        yield return new WaitForSeconds(beatInterval);

        Debug.Log("Bass Drop: Second Explosion!");
        WeaponStats stats = weapon.stats[weapon.weaponLevel];

        // Tìm tất cả kẻ địch trong một bán kính lớn hơn và gây Boom Damage
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(transform.position, stats.range * 1.2f);
        foreach (var enemyCollider in enemiesToDamage)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                Enemy enemy = enemyCollider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // Vụ nổ thứ hai luôn gây Boom Damage
                    enemy.TakeDamage(stats.boomDamage, stats.knockbackForce * 1.5f);
                }
            }
        }
        // Có thể thêm hiệu ứng hình ảnh cho vụ nổ thứ 2 ở đây
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * 5);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            targetSize = Vector3.zero;
            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
            return;
        }
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            counter = weapon.stats[weapon.weaponLevel].speed;
            WeaponStats stats = weapon.stats[weapon.weaponLevel];
            for (int i = 0; i < enemiesInRange.Count; i++)
            {
                if (enemiesInRange[i] != null)
                {
                    if(wasOnBeat)
                    {
                        enemiesInRange[i].TakeDamage(stats.boomDamage, stats.knockbackForce);
                    }
                    else
                    {
                        enemiesInRange[i].TakeDamage(stats.damage, stats.knockbackForce);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemiesInRange.Add(collider.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(collider.GetComponent<Enemy>());
        }
    }
}