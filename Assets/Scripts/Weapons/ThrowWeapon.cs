using UnityEngine;

public class ThrowWeapon : Weapon
{
    [SerializeField] private GameObject prefab;

    public void ActivateWeapon(bool isOnBeat)
    {
        GameObject thrownObject = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        ThrowWeaponPrefab thrownScript = thrownObject.GetComponent<ThrowWeaponPrefab>();
        if (thrownScript != null)
        {
            thrownScript.Setup(this, isOnBeat);
        }
    }
}