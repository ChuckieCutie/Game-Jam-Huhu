using UnityEngine;

public class MicSpinWeapon : Weapon
{
    public GameObject prefab;

    public void ActivateWeapon(bool isOnBeat) // isOnBeat ở đây có thể không cần thiết, nhưng giữ để đồng bộ
    {
        GameObject spawnedWeapon = Instantiate(prefab, transform.position, Quaternion.identity);
        MicSpinProjectile projectileScript = spawnedWeapon.GetComponent<MicSpinProjectile>();
        if (projectileScript != null)
        {
            // "Đăng ký" projectile này với PlayerController
            PlayerController.Instance.activeMicProjectile = projectileScript;

            projectileScript.SetDirection(PlayerController.Instance.lastMoveDirection);
            projectileScript.Setup(this, isOnBeat);
        }
    }
}