using UnityEngine;

public class DirectionalWeapon : Weapon
{   
    [SerializeField] private GameObject prefab;
    
    public void ActivateWeapon(){
        for (int i = 0; i < stats[weaponLevel].amount; i++)
        {
            Instantiate(prefab, transform.position, transform.rotation, transform);
        }
    }
}
