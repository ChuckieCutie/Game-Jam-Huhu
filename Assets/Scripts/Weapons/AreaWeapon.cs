using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWeapon : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private float spawnCounter;

    public float cooldown = 5f;
    public float duration = 3f;
    public float damage = 1f;
    public float range = 0.7f;
    public float speed = 0.5f;

    void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0){
            spawnCounter = cooldown;
            Instantiate (prefab, transform.position, transform.rotation, transform);
        }    
    }
}
