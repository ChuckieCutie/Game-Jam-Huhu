using UnityEngine;

public class AreaWeapon : Weapon
{
    [SerializeField] private GameObject prefab;

    public void ActivateWeapon(bool isOnBeat)
    {
        GameObject areaObj = Instantiate(prefab, transform.position, transform.rotation, transform);
        AreaWeaponPrefab areaScript = areaObj.GetComponent<AreaWeaponPrefab>();
        if (areaScript != null)
        {
            areaScript.Setup(isOnBeat);
        }
    }
}