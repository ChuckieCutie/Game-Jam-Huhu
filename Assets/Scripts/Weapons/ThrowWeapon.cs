using UnityEngine;

public class ThrowWeapon : Weapon
{
    [SerializeField] private GameObject prefab;

    public void ActivateWeapon()
    {
        // Chỉ ném 1 quả bóng thẳng, không cần loop
        GameObject thrownObject = Instantiate(prefab, transform.position, Quaternion.identity, transform);
    }
}