using System.Collections;
using UnityEngine;

public class SlowTrapWeapon : Weapon
{
    [SerializeField] private GameObject prefab;
    private const float CAST_DELAY = 0.5f; 

    public void ActivateWeapon(Vector3 position)
    {
        StartCoroutine(SpawnTrapAfterDelay(position));
    }

    private IEnumerator SpawnTrapAfterDelay(Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(CAST_DELAY);
        Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
    }
}