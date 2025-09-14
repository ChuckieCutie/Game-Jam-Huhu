using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaWeaponPrefab : MonoBehaviour
{
    public AreaWeapon weapon;
    private Vector3 targetSize;
    private float timer;
    public List<Enemy> enemiesInRange = new List<Enemy>();
    private float counter;

    void Start()
    {
        weapon = GameObject.Find("Area Weapon").GetComponent<AreaWeapon>();   
        targetSize = Vector3.one * weapon.range;
        transform.localScale = Vector3.zero;
        timer = weapon.duration;
        counter = 1f;
    }

    void Update()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * 3f);
        timer -= Time.deltaTime;
        
        if (timer <= 0)
        {
            targetSize = Vector3.zero;
            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
        
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            counter = 1f; 
            counter = weapon.speed;

            for (int i = 0; i < enemiesInRange.Count; i++)
            {
             enemiesInRange[i].TakeDamage(weapon.damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemiesInRange.Add(collider.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(collider.GetComponent<Enemy>());
        }
    }
}