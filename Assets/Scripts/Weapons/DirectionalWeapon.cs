using UnityEngine;

public class DirectionalWeapon : Weapon
{
    [Header("Prefabs")]
    // <<< THAY ĐỔI: Đổi "private" thành "public" để các script khác có thể truy cập
    public GameObject normalAttackPrefab; 
    public GameObject spinAttackPrefab;

    public void ActivateWeapon(bool isOnBeat)
    {
        WeaponStats stats = this.stats[weaponLevel];

        if (isOnBeat)
        {
            // ĐÚNG NHỊP: Tạo hiệu ứng xoay tròn
            GameObject spinObj = Instantiate(spinAttackPrefab, transform.position, transform.rotation);
            SpinAttack spinScript = spinObj.GetComponent<SpinAttack>();
            if(spinScript != null)
            {
                // Truyền chỉ số Boom vào cho hiệu ứng
                spinScript.damage = stats.boomDamage;
                spinScript.knockback = stats.knockbackForce;
            }
        }
        else
        {
            // TRƯỢT NHỊP: Bắn đạn thường
            for (int i = 0; i < stats.amount; i++)
            {
                GameObject projectileObj = Instantiate(normalAttackPrefab, transform.position, transform.rotation, transform);
                DirectionalWeaponPrefab projectileScript = projectileObj.GetComponent<DirectionalWeaponPrefab>();
                if (projectileScript != null)
                {
                    projectileScript.Setup(false); 
                }
            }
        }
    }
}