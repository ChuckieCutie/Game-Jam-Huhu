using Unity.VisualScripting;
using UnityEngine;

public class AreaWeapon : Weapon
{
    [SerializeField] private GameObject prefab;

    public void ActivateWeapon()
    {
        Instantiate(prefab, transform.position, transform.rotation, transform);
    }
}
