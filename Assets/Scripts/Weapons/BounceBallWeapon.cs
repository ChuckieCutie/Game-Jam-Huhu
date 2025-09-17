using UnityEngine;

public class BounceBallWeapon : Weapon
{
    [SerializeField] private GameObject prefab;

    public void ActivateWeapon()
    {
        Instantiate(prefab, transform.position, Quaternion.identity, transform);
    }
}