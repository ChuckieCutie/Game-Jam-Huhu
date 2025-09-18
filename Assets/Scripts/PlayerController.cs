using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    // --- CẤU TRÚC VŨ KHÍ ---
    [Header("Weapon Slots")]
    public DirectionalWeapon directionalWeapon; // Vũ khí Q (cố định)

    private Weapon weaponSlotW;
    private Weapon weaponSlotE;
    private Weapon weaponSlotR;

    private float cooldownQ, cooldownW, cooldownE, cooldownR;

    [Header("Weapon Management")]
    [SerializeField] private List<Weapon> inactiveWeaponsPool; // Pool vũ khí chưa mở khóa
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

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } 
        else { Instance = this; }
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

        // Bắt đầu với vũ khí Q
        activeWeapons.Add(directionalWeapon);
        directionalWeapon.gameObject.SetActive(true);
    }

    void Update()
    {
        HandleMovement();
        HandleImmunity();
        HandleCooldowns();
        HandleWeaponInputs();
    }

    void HandleMovement()
    {
        // Trả lại di chuyển bằng chuột phải
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
    
    void HandleCooldowns()
    {
        if (cooldownQ > 0) cooldownQ -= Time.deltaTime;
        if (cooldownW > 0) cooldownW -= Time.deltaTime;
        if (cooldownE > 0) cooldownE -= Time.deltaTime;
        if (cooldownR > 0) cooldownR -= Time.deltaTime;
    }

    void HandleWeaponInputs()
    {
        // Phím Q (cố định)
        if (Input.GetKeyDown(KeyCode.Q) && directionalWeapon != null && cooldownQ <= 0)
        {
            directionalWeapon.ActivateWeapon();
            cooldownQ = directionalWeapon.stats[directionalWeapon.weaponLevel].cooldown;
        }

        // Phím W
        if (Input.GetKeyDown(KeyCode.W) && weaponSlotW != null && cooldownW <= 0)
        {
            ActivateWeaponInSlot(weaponSlotW, ref cooldownW);
        }

        // Phím E
        if (Input.GetKeyDown(KeyCode.E) && weaponSlotE != null && cooldownE <= 0)
        {
            ActivateWeaponInSlot(weaponSlotE, ref cooldownE);
        }

        // Phím R
        if (Input.GetKeyDown(KeyCode.R) && weaponSlotR != null && cooldownR <= 0)
        {
            ActivateWeaponInSlot(weaponSlotR, ref cooldownR);
        }
    }
    
    // Hàm phụ trợ để kích hoạt vũ khí trong ô, xử lý các trường hợp đặc biệt
    void ActivateWeaponInSlot(Weapon weapon, ref float cooldown)
    {
        if (weapon is SlowTrapWeapon slowTrap)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            slowTrap.ActivateWeapon(mousePosition);
        }
        else // Dành cho các vũ khí khác không cần vị trí chuột (Area, Spin, BounceBall)
        {
            weapon.SendMessage("ActivateWeapon", SendMessageOptions.DontRequireReceiver);
        }
        
        cooldown = weapon.stats[weapon.weaponLevel].cooldown;
    }

    void HandleImmunity()
    {
        if (immunityTimer > 0) { immunityTimer -= Time.deltaTime; }
        else { isImmune = false; }
    }

    public void LevelUp()
    {
        experience -= playerLevels[currentLevel - 1];
        currentLevel++;
        UIController.Instance.UpdateExperienceSlider();

        upgradeableWeapons.Clear();
        
        // Kiểm tra xem còn ô W, E, hoặc R trống không
        if (weaponSlotW == null || weaponSlotE == null || weaponSlotR == null)
        {
            // Nếu còn, hiển thị các vũ khí mới có thể chọn
            upgradeableWeapons.AddRange(inactiveWeaponsPool);
        }
        else // Nếu tất cả các ô đã có vũ khí
        {
            // Hiển thị các vũ khí đang sở hữu để nâng cấp
            upgradeableWeapons.AddRange(activeWeapons.Where(w => w.weaponLevel < w.stats.Count - 1));
        }
        
        for (int i = 0; i < UIController.Instance.levelUpButtons.Length; i++)
        {
            if (i < upgradeableWeapons.Count)
            {
                UIController.Instance.levelUpButtons[i].ActivateButton(upgradeableWeapons[i]);
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(true);
            }
            else
            {
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }

        UIController.Instance.LevelUpPanelOpen();
    }
    
    public void AssignWeaponToSlot(Weapon newWeapon)
    {
        // Gán vũ khí mới vào ô trống đầu tiên tìm thấy
        if (weaponSlotW == null) 
        {
            weaponSlotW = newWeapon;
        }
        else if (weaponSlotE == null) 
        {
            weaponSlotE = newWeapon;
        }
        else if (weaponSlotR == null) 
        {
            weaponSlotR = newWeapon;
        }
        
        newWeapon.gameObject.SetActive(true);
        activeWeapons.Add(newWeapon);
        inactiveWeaponsPool.Remove(newWeapon);
    }
    
    void FixedUpdate(){
        rb.velocity = new Vector3(playerMoveDirection.x * moveSpeed, playerMoveDirection.y * moveSpeed);
    }

    public void TakeDamage(float damage){
        if (!isImmune){
            isImmune = true;
            immunityTimer = immunityDuration;
            playerHealth -= damage;
            UIController.Instance.UpdateHealthSlider();
            if (playerHealth <= 0){
                gameObject.SetActive(false);
                GameManager.Instance.GameOver();
            }
        }
    }

    public void GetExperience(int experienceToGet){
        experience += experienceToGet;
        UIController.Instance.UpdateExperienceSlider();
        if (currentLevel < maxLevel && experience >= playerLevels[currentLevel - 1]){
            LevelUp();
        }
    }
    
    public void IncreaseMaxHealth(int value){
        playerMaxHealth += value;
        playerHealth = playerMaxHealth;
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }

    public void IncreaseMovementSpeed(float multiplier){
        moveSpeed *= multiplier;
        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }
}