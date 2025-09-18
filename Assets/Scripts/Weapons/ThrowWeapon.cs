using UnityEngine;

public class ThrowWeapon : Weapon
{
    [SerializeField] private GameObject prefab;

    public void ActivateWeapon()
    {
        for (int i = 0; i < stats[weaponLevel].amount; i++)
        {
            // Tạo một góc ném khác nhau cho mỗi quả bóng
            float angleOffset = (i - (stats[weaponLevel].amount - 1) / 2f) * 15f; // Góc lệch 15 độ giữa mỗi quả
            
            GameObject thrownObject = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            ThrowWeaponPrefab throwScript = thrownObject.GetComponent<ThrowWeaponPrefab>();
            
            if (throwScript != null)
            {
                throwScript.SetThrowAngle(angleOffset);
            }
        }
    }
}