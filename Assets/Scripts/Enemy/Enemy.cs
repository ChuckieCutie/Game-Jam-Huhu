using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;
    [SerializeField] private int experienceToGive;

    [Header("Movement Optimization")]
    [SerializeField] private float directionUpdateInterval = 0.2f;

    [Header("Effects")]
    [SerializeField] private float knockbackDuration; // <<< SỬA LỖI 3: Đảm bảo biến này được khai báo (và đổi tên từ pushTime)
    [SerializeField] private GameObject destroyEffect;
        [Header("Boss Settings")] // <<< THÊM MỚI
    public bool isFinalBoss = false;

    // --- Private Variables ---
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private float currentMoveSpeed;
    private bool isSlowed;
    private float pushCounter;
    private Coroutine updateDirectionCoroutine;
    private float currentKnockbackForce; 
    private bool isStunned = false;// <<< SỬA LỖI 1 & 2: Đảm bảo biến này được khai báo

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentMoveSpeed = moveSpeed;
    }

    void OnEnable()
    {
        ResetStats();
        if (updateDirectionCoroutine != null) StopCoroutine(updateDirectionCoroutine);
        updateDirectionCoroutine = StartCoroutine(UpdateDirectionRoutine());
    }

    void OnDisable()
    {
        if (updateDirectionCoroutine != null) StopCoroutine(updateDirectionCoroutine);
    }

    void FixedUpdate()
    {
                if (isStunned) // <<< THÊM MỚI
        {
            rb.velocity = Vector2.zero;
            return; // Nếu bị choáng, không làm gì cả
        }
        if (!PlayerController.Instance.gameObject.activeSelf)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        
        if (pushCounter > 0)
        {
            pushCounter -= Time.deltaTime;
            rb.velocity = -direction.normalized * currentKnockbackForce; 
            if (pushCounter <= 0)
            {
                rb.velocity = Vector2.zero;
            }
            return;
        }
        
        rb.velocity = new Vector2(direction.x * currentMoveSpeed, direction.y * currentMoveSpeed);
    }
    
    private IEnumerator UpdateDirectionRoutine()
    {
        while (true)
        {
            if (!isStunned)
            {
                direction = (PlayerController.Instance.transform.position - transform.position).normalized;
                spriteRenderer.flipX = PlayerController.Instance.transform.position.x > transform.position.x;
            }
            else
            {
                direction = Vector3.zero;
            }
            yield return new WaitForSeconds(directionUpdateInterval);
        }
    }

    
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        // Có thể thêm hiệu ứng hình ảnh (ví dụ: các ngôi sao xoay trên đầu) ở đây
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }


    public void ResetStats()
    {
        health = maxHealth;
        isSlowed = false;
        currentMoveSpeed = moveSpeed;
        pushCounter = 0;
        isStunned = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(damage);
        }
    }

     public void TakeDamage(float damageAmount, float knockbackForce)
    {
        health -= damageAmount;
        DamageNumberController.Instance.CreateNumber(damageAmount, transform.position);
        
        currentKnockbackForce = knockbackForce;
        pushCounter = knockbackDuration;
        
        if (health <= 0)
        {
            // <<< THÊM MỚI: Kiểm tra nếu đây là trùm cuối
            if (isFinalBoss)
            {
                // Chỉ gọi MusicManager khi trùm cuối chết
                MusicManager.Instance.OnBossDefeated();
            }

            PlayerController.Instance.GetExperience(experienceToGive);
            if(destroyEffect != null) Instantiate(destroyEffect, transform.position, transform.rotation);
            AudioController.Instance.PlayModifiedSound(AudioController.Instance.enemyDie);
            
            gameObject.SetActive(false); // Trả về pool
        }
    }
    public void ApplySlow(float slowAmount, float duration)
    {
        if (!isSlowed)
        {
            isSlowed = true;
            currentMoveSpeed = moveSpeed * (1 - slowAmount);
            Invoke("ResetSpeed", duration);
        }
    }

    private void ResetSpeed()
    {
        currentMoveSpeed = moveSpeed;
        isSlowed = false;
    }
}