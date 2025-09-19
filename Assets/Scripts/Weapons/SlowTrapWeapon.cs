using System.Collections;
using UnityEngine;

public class SlowTrapWeapon : Weapon
{
    [SerializeField] private GameObject prefab;
    private const float CAST_DELAY = 0.5f;

    public void ActivateWeapon(Vector3 position, bool isOnBeat)
    {
        StartCoroutine(SpawnTrapAfterDelay(position, isOnBeat));
    }

    private IEnumerator SpawnTrapAfterDelay(Vector3 spawnPosition, bool isOnBeat)
    {
        yield return new WaitForSeconds(CAST_DELAY);
        GameObject trapObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        SlowTrapPrefab trapPrefab = trapObject.GetComponent<SlowTrapPrefab>();
        if (trapPrefab != null)
        {
            trapPrefab.SetWeapon(this);
            trapPrefab.Setup(isOnBeat);
        }
    }
}