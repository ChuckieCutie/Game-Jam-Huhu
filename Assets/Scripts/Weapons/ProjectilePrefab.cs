using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePrefab : MonoBehaviour
{
    // Các chỉ số này sẽ được vũ khí gốc truyền vào
    public float speed;
    public float damage;
    public float lifetime; // Thời gian tồn tại của đạn

    private Vector3 direction; // Hướng bay của đạn

    // Hàm này được gọi bởi vũ khí để cài đặt hướng bay
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Start()
    {
        // Tự hủy sau một khoảng thời gian nếu không va vào đâu
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        // Di chuyển viên đạn theo hướng đã định
        transform.position += direction * speed * Time.deltaTime;
    }

    // Xử lý va chạm
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu va phải kẻ địch
        if (other.CompareTag("Enemy"))
        {
            // Gây sát thương
            other.GetComponent<Enemy>().TakeDamage(damage);
            // Và tự hủy ngay lập tức
            Destroy(gameObject);
        }
    }
}
