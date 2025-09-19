using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Weapon Slots")]
    public DirectionalWeapon directionalWeapon;
    private Dictionary<string, Weapon> weaponSlots = new Dictionary<string, Weapon>();
    private Dictionary<string, float> cooldowns = new Dictionary<string, float>();
    private Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>();

    [Header("Weapon Management")]
    [SerializeField] private List<Weapon> inactiveWeaponsPool;
    public List<Weapon> activeWeapons;
    [SerializeField] private List<Weapon> upgradeableWeapons;
    public List<Weapon> maxLevelWeapons;

    [Header("Player Stats")]
    [SerializeField] private float moveSpeed;
    public Vector3 playerMoveDirection;
    public Vector3 lastMoveDirection;
    public float playerMaxHealth;
    public float playerHealth;

    [Header("Experience & Level")]
    public int experience;
    public int currentLevel;
    public int maxLevel;
    public List<int> playerLevels;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    
    private Vector3 targetPosition;
    private bool isImmune;
    [SerializeField] private float immunityDuration;
    [SerializeField] private float immunityTimer;
    [HideInInspector] public MicSpinProjectile activeMicProjectile;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } 
        else { Instance = this; }
        InitializeWeaponSlots();
    }

    void Start()
    {
        lastMoveDirection = new Vector3(0, -1);
        for (int i = playerLevels.Count; i < maxLevel; i++)
        {
            playerLevels.Add(Mathf.CeilToInt(playerLevels[playerLevels.Count - 1] * 1.1f + 15));
        }
        playerHealth = playerMaxHealth;
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.UpdateExperienceSlider();
        targetPosition = transform.position;
        activeWeapons.Add(directionalWeapon);
        directionalWeapon.gameObject.SetActive(true);
    }

    void Update()
    {
        HandleMovement();
        HandleImmunity();
        HandleCooldowns();
        HandleWeaponInputs();
        if (activeMicProjectile != null)
    {
        // Giả sử phím E cũng là phím kích hoạt
        if (Input.GetKeyDown(KeyCode.E) && RhythmManager.Instance.CheckTiming())
        {
            activeMicProjectile.TriggerBoomEffect();
        }
    }
    }

    void InitializeWeaponSlots()
    {
        weaponSlots.Add("W", null);
        weaponSlots.Add("E", null);
        weaponSlots.Add("R", null);
        cooldowns.Add("Q", 0f);
        cooldowns.Add("W", 0f);
        cooldowns.Add("E", 0f);
        cooldowns.Add("R", 0f);
        keyMappings.Add("W", KeyCode.W);
        keyMappings.Add("E", KeyCode.E);
        keyMappings.Add("R", KeyCode.R);
    }

    void HandleCooldowns()
    {
        List<string> keys = new List<string>(cooldowns.Keys);
        foreach (string key in keys)
        {
            if (cooldowns[key] > 0)
            {
                cooldowns[key] -= Time.deltaTime;
            }
        }
    }

    // <<< PHIÊN BẢN ĐÚNG CỦA HÀM GÂY LỖI
    void HandleWeaponInputs()
    {
        if (Input.GetKeyDown(KeyCode.Q) && IsSkillReady("Q"))
        {
            bool onBeat = RhythmManager.Instance.CheckTiming();
            directionalWeapon.ActivateWeapon(onBeat);
            WeaponStats stats = directionalWeapon.stats[directionalWeapon.weaponLevel];
            if (onBeat)
            {
                Debug.Log("<color=lime>Q: ĐÚNG NHỊP!</color>");
                cooldowns["Q"] = stats.cooldown - stats.cooldownReductionOnBeat;
            }
            else
            {
                Debug.Log("<color=red>Q: TRƯỢT NHỊP!</color>");
                cooldowns["Q"] = stats.cooldown;
            }
            SkillCooldownUI.Instance.StartCooldown("Q", cooldowns["Q"]);
        }

        foreach (var mapping in keyMappings)
        {
            string slotKey = mapping.Key;
            KeyCode keyCode = mapping.Value;
            if (Input.GetKeyDown(keyCode) && IsSkillReady(slotKey))
            {
                bool onBeat = RhythmManager.Instance.CheckTiming();
                Weapon weapon = weaponSlots[slotKey];
                ActivateWeaponInSlot(weapon, onBeat); // Gọi hàm với đủ 2 tham số
                WeaponStats stats = weapon.stats[weapon.weaponLevel];
                if (onBeat)
                {
                    Debug.Log("<color=lime>" + slotKey + ": ĐÚNG NHỊP!</color>");
                    cooldowns[slotKey] = stats.cooldown - stats.cooldownReductionOnBeat;
                }
                else
                {
                    Debug.Log("<color=red>" + slotKey + ": TRƯỢT NHỊP!</color>");
                    cooldowns[slotKey] = stats.cooldown;
                }
                SkillCooldownUI.Instance.StartCooldown(slotKey, cooldowns[slotKey]);
            }
        }
    }
    
    // <<< PHIÊN BẢN ĐÚNG CỦA HÀM GÂY LỖI
    void ActivateWeaponInSlot(Weapon weapon, bool isOnBeat)
    {
        if (weapon is SlowTrapWeapon slowTrap)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            slowTrap.ActivateWeapon(mousePosition, isOnBeat);
        }
        else
        {
            weapon.SendMessage("ActivateWeapon", isOnBeat, SendMessageOptions.DontRequireReceiver);
        }
    }

    #region Các hàm không thay đổi
    public void AssignWeaponToSlot(Weapon newWeapon)
    {
        string assignedSlot = "";
        if (weaponSlots["W"] == null) assignedSlot = "W";
        else if (weaponSlots["E"] == null) assignedSlot = "E";
        else if (weaponSlots["R"] == null) assignedSlot = "R";
        if (!string.IsNullOrEmpty(assignedSlot))
        {
            weaponSlots[assignedSlot] = newWeapon;
            newWeapon.gameObject.SetActive(true);
            activeWeapons.Add(newWeapon);
            inactiveWeaponsPool.Remove(newWeapon);
            if (SkillCooldownUI.Instance != null)
            {
                SkillCooldownUI.Instance.AssignWeaponToSlot(newWeapon, assignedSlot);
            }
        }
    }
    public float GetRemainingCooldown(string slot)
    {
        if (cooldowns.ContainsKey(slot.ToUpper())) { return cooldowns[slot.ToUpper()]; }
        return 0;
    }
    public bool IsSkillReady(string slot)
    {
        string upperSlot = slot.ToUpper();
        if (!cooldowns.ContainsKey(upperSlot) || cooldowns[upperSlot] > 0) return false;
        if (upperSlot == "Q") return directionalWeapon != null;
        if (weaponSlots.ContainsKey(upperSlot)) return weaponSlots[upperSlot] != null;
        return false;
    }
    void HandleMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z;
        }
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f && Time.timeScale != 0)
        {
            playerMoveDirection = (targetPosition - transform.position).normalized;
            lastMoveDirection = playerMoveDirection;
            animator.SetBool("moving", true);
            animator.SetFloat("moveX", playerMoveDirection.x);
            animator.SetFloat("moveY", playerMoveDirection.y);
        }
        else
        {
            playerMoveDirection = Vector3.zero;
            animator.SetBool("moving", false);
        }
    }
    void HandleImmunity()
    {
        if (immunityTimer > 0) { immunityTimer -= Time.deltaTime; }
        else { isImmune = false; }
    }
    public void LevelUp()
    {
        if (currentLevel >= maxLevel) return;
        experience -= playerLevels[currentLevel - 1];
        currentLevel++;
        UIController.Instance.UpdateExperienceSlider();
        upgradeableWeapons.Clear();
        var upgradableActiveWeapons = activeWeapons.Where(w => w.weaponLevel < w.stats.Count - 1).ToList();
        if ((weaponSlots["W"] == null || weaponSlots["E"] == null || weaponSlots["R"] == null) && inactiveWeaponsPool.Any())
        {
            upgradeableWeapons.AddRange(inactiveWeaponsPool);
        }
        upgradeableWeapons.AddRange(upgradableActiveWeapons);
        upgradeableWeapons = upgradeableWeapons.OrderBy(x => Random.value).ToList();
        for (int i = 0; i < UIController.Instance.levelUpButtons.Length; i++)
        {
            if (i < upgradeableWeapons.Count)
            {
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(true);
                UIController.Instance.levelUpButtons[i].ActivateButton(upgradeableWeapons[i]);
            }
            else
            {
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }
        UIController.Instance.LevelUpPanelOpen();
    }
    void FixedUpdate() { rb.velocity = new Vector3(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed); }
    public void TakeDamage(float damage)
    {
        if (!isImmune)
        {
            isImmune = true;
            immunityTimer = immunityDuration;
            playerHealth -= damage;
            UIController.Instance.ShowDamageFlash();
            UIController.Instance.UpdateHealthSlider();
            if (playerHealth <= 0)
            {
                gameObject.SetActive(false);
                GameManager.Instance.GameOver();
            }
        }
    }
    public void GetExperience(int experienceToGet)
    {
        experience += experienceToGet;
        UIController.Instance.UpdateExperienceSlider();
        if (currentLevel < maxLevel && experience >= playerLevels[currentLevel - 1])
        {
            LevelUp();
        }
    }
    public void IncreaseMaxHealth(int value)
    {
        playerMaxHealth += value;
        playerHealth = playerMaxHealth;
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }
    public void IncreaseMovementSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }
    #endregion
}