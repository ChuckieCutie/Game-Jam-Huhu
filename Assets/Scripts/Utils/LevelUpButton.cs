using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpButton : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text weaponDescriptionText;

    private Weapon assignedWeapon;

    public void ActivateButton(Weapon weapon)
    {
        assignedWeapon = weapon;
        
        weaponIcon.sprite = weapon.weaponImage;
        weaponNameText.text = weapon.name;

        int nextLevel = weapon.weaponLevel + 1;
        if (PlayerController.Instance.activeWeapons.Contains(weapon))
        {
            if (nextLevel < weapon.stats.Count)
            {
                weaponDescriptionText.text = "LVL " + (nextLevel + 1) + "\n" + weapon.stats[nextLevel].description;
            }
            else
            {
                weaponDescriptionText.text = "MAX LEVEL";
            }
        }
        else
        {
            weaponDescriptionText.text = "NEW\n" + weapon.stats[0].description;
        }
    }

    public void SelectUpgrade()
    {
        if (assignedWeapon == null) return;

        if (PlayerController.Instance.activeWeapons.Contains(assignedWeapon))
        {
            assignedWeapon.LevelUp();
        }
        else
        {
            PlayerController.Instance.AssignWeaponToSlot(assignedWeapon);
        }

        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }
}