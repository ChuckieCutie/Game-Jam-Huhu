using UnityEngine;

// Đảm bảo tên file là MicSpinWeapon.cs
public class MicSpinWeapon : Weapon 
{
    public GameObject prefab;

    public void ActivateWeapon()
    {
        // Tạo ra một "mic" tại vị trí của người chơi
        GameObject spawnedWeapon = Instantiate(prefab, transform.position, Quaternion.identity);

        // Lấy script của "mic" và gán hướng bay cho nó
        // Hướng bay được lấy từ hướng di chuyển cuối cùng của người chơi
        spawnedWeapon.GetComponent<MicSpinProjectile>().SetDirection(PlayerController.Instance.lastMoveDirection);
        
        // Bạn có thể thêm âm thanh ném mic ở đây
        // AudioController.Instance.PlaySound(AudioController.Instance.throwSound);
    }
}