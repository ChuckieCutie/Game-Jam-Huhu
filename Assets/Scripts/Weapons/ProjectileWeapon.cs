using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    // Kéo prefab viên đạn vào đây trong Unity Editor
    [SerializeField] private GameObject projectilePrefab;

    // Hàm này sẽ được PlayerController gọi khi bấm 'Q'
    public void Attack()
    {
        // 1. Tạo ra một viên đạn từ prefab
        GameObject spawnedProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // 2. Lấy script của viên đạn vừa tạo
        ProjectilePrefab prefabScript = spawnedProjectile.GetComponent<ProjectilePrefab>();
        
        // 3. Truyền các chỉ số từ vũ khí cho viên đạn
        if (prefabScript != null)
        {
            // Lấy hướng nhìn cuối cùng của người chơi để làm hướng bắn
            Vector3 shootDirection = PlayerController.Instance.lastMoveDirection;

            // Cài đặt các thông số cho viên đạn
            prefabScript.SetDirection(shootDirection);
            prefabScript.damage = stats[weaponLevel].damage;
            prefabScript.speed = stats[weaponLevel].speed;
            // Dùng duration trong WeaponStats để làm thời gian tồn tại của đạn
            prefabScript.lifetime = stats[weaponLevel].duration; 
        }
    }
}