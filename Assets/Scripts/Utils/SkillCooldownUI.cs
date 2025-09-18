using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SkillCooldownUI : MonoBehaviour
{
    public static SkillCooldownUI Instance;

    [System.Serializable]
    public class SkillSlot
    {
        // Các trường để bạn kéo thả UI từ Hierarchy vào
        public Image skillIcon;
        public GameObject cooldownOverlay; // THAY ĐỔI: Giờ là GameObject để bật/tắt
        public TextMeshProUGUI cooldownText;
        public TextMeshProUGUI hotkeyText;
        public GameObject lockIcon;

        // Các biến nội bộ, không cần quan tâm
        [HideInInspector] public bool hasSkill;
        [HideInInspector] public Weapon assignedWeapon;
    }

    [Header("Skill Slots")]
    public SkillSlot slotQ;
    public SkillSlot slotW;
    public SkillSlot slotE;
    public SkillSlot slotR;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Khởi tạo trạng thái ban đầu cho các ô skill
        InitializeSingleSlot(slotQ, "Q");
        InitializeSingleSlot(slotW, "W", false);
        InitializeSingleSlot(slotE, "E", false);
        InitializeSingleSlot(slotR, "R", false);

        if (PlayerController.Instance != null && PlayerController.Instance.directionalWeapon != null)
        {
            AssignWeaponToSlot(PlayerController.Instance.directionalWeapon, "Q");
        }
    }

    // Hàm phụ trợ để setup ban đầu
    private void InitializeSingleSlot(SkillSlot slot, string name, bool hasSkillByDefault = true)
    {
        slot.hotkeyText.text = name;
        slot.hasSkill = hasSkillByDefault;
        
        // Mặc định ẩn hết overlay và text cooldown
        if (slot.lockIcon) slot.lockIcon.SetActive(!hasSkillByDefault);
        if (slot.cooldownText) slot.cooldownText.gameObject.SetActive(false);
        if (slot.cooldownOverlay) slot.cooldownOverlay.SetActive(false);
    }

    // PlayerController sẽ gọi hàm này khi có vũ khí mới
    public void AssignWeaponToSlot(Weapon weapon, string slotName)
    {
        SkillSlot targetSlot = GetSlotByName(slotName);
        if (targetSlot != null)
        {
            targetSlot.hasSkill = true;
            targetSlot.assignedWeapon = weapon;

            // Tự động gán icon của weapon vào ô skillIcon
            if (targetSlot.skillIcon && weapon.weaponImage)
            {
                targetSlot.skillIcon.sprite = weapon.weaponImage;
                targetSlot.skillIcon.color = Color.white;
            }

            if (targetSlot.lockIcon) targetSlot.lockIcon.SetActive(false);
        }
    }

    // PlayerController sẽ gọi hàm này khi dùng skill
    public void StartCooldown(string slotName, float cooldownTime)
    {
        SkillSlot targetSlot = GetSlotByName(slotName);
        if (targetSlot != null && targetSlot.hasSkill)
        {
            StartCoroutine(CooldownEffect(targetSlot, cooldownTime));
        }
    }

    // Coroutine đếm ngược (đã được làm đơn giản hóa)
    IEnumerator CooldownEffect(SkillSlot slot, float cooldownTime)
    {
        // THAY ĐỔI: Bật lớp phủ và text lên
        slot.cooldownOverlay.SetActive(true);
        slot.cooldownText.gameObject.SetActive(true);

        float remainingTime = cooldownTime;
        while (remainingTime > 0)
        {
            // Cập nhật số đếm ngược
            slot.cooldownText.text = remainingTime.ToString("F1"); // Hiển thị 1 số lẻ, vd: 4.2
            
            // Dùng unscaledDeltaTime để UI vẫn chạy khi game pause (ví dụ lúc level up)
            remainingTime -= Time.unscaledDeltaTime; 
            yield return null;
        }
        
        // Hết giờ: Tắt lớp phủ và text đi
        slot.cooldownOverlay.SetActive(false);
        slot.cooldownText.gameObject.SetActive(false);
    }
    
    // Hàm tìm slot theo tên
    SkillSlot GetSlotByName(string name)
    {
        switch (name.ToUpper())
        {
            case "Q": return slotQ;
            case "W": return slotW;
            case "E": return slotE;
            case "R": return slotR;
            default: return null;
        }
    }
}