using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;
    private Vector3 direction;

    [SerializeField] private float moveSpeed; // Giữ lại dòng này
    [SerializeField] private float damage;
    [SerializeField] private float health;
    [SerializeField] private int experienceToGive;
    [SerializeField] private float pushTime;

    private float currentMoveSpeed; 
    private bool isSlowed; 

    private float pushCounter;

    [SerializeField] private GameObject destroyEffect;

    
    void Awake()
    {
        currentMoveSpeed = moveSpeed; 
    }

    void FixedUpdate()
    {
        if (PlayerController.Instance.gameObject.activeSelf){

            if (PlayerController.Instance.transform.position.x > transform.position.x){
                spriteRenderer.flipX = true;
            } else {
                spriteRenderer.flipX = false;
            }
            
            if (pushCounter > 0){
                pushCounter -= Time.deltaTime;
                rb.velocity = -direction * moveSpeed; 
                if (pushCounter <= 0){
                }
                return; 
            }

            direction = (PlayerController.Instance.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * currentMoveSpeed, direction.y * currentMoveSpeed);
        } else {
            rb.velocity = Vector2.zero;
        }
    }

    void OnCollisionStay2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Player")){
            PlayerController.Instance.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage){
        health -= damage;
        DamageNumberController.Instance.CreateNumber(damage, transform.position);
        pushCounter = pushTime;
        if (health <= 0){
            Destroy(gameObject);
            Instantiate(destroyEffect, transform.position, transform.rotation);
            PlayerController.Instance.GetExperience(experienceToGive);
            AudioController.Instance.PlayModifiedSound(AudioController.Instance.enemyDie);
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