using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private float moveSpeed;
    public Vector3 playerMoveDirection;
    public Vector3 lastMoveDirection;
    public float playerMaxHealth;
    public float playerHealth;

    public int experience;
    public int currentLevel = 1;
    public int maxLevel;

    public float wCooldown = 5f;
    public float eCooldown = 8f;
    public float rCooldown = 10f;
    public float tCooldown = 60f;
    public float spaceCooldown = 3f;

    private float wCooldownTimer = 0f;
    private float eCooldownTimer = 0f;
    private float rCooldownTimer = 0f;
    private float tCooldownTimer = 0f;
    private float spaceCooldownTimer = 0f;

    public float qAttackResetTime = 0.5f;
    private float qAttackTimer = 0f;

    [SerializeField] private List<Weapon> inactiveWeapons;
    public List<Weapon> activeWeapons;
    [SerializeField] private List<Weapon> upgradeableWeapons;
    public List<Weapon> maxLevelWeapons;
    
    // ===== CHỖ THÊM SỐ 1 =====
    public UIController uiController;
    // ===========================

    private bool isImmune;
    [SerializeField] private float immunityDuration;
    [SerializeField] private float immunityTimer;

    public List<int> playerLevels;

    public Vector3 mouseTargetingPosition;
    
    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Start(){
        lastMoveDirection = new Vector3(0, -1);
        for (int i = playerLevels.Count; i < maxLevel; i++){
            playerLevels.Add(Mathf.CeilToInt(playerLevels[playerLevels.Count - 1] * 1.1f + 15));
        }
        playerHealth = playerMaxHealth;
        UIController.Instance.UpdateHealthSlider();
        UIController.Instance.UpdateExperienceSlider();
        AddWeapon(Random.Range(0, inactiveWeapons.Count));
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButton(1)){
            mouseTargetingPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseTargetingPosition.z = 0;
            playerMoveDirection = (mouseTargetingPosition - transform.position).normalized;
        }else{
            playerMoveDirection = Vector3.zero;
        }
        if (playerMoveDirection == Vector3.zero){
            animator.SetBool("moving", false);
        } else if (Time.timeScale != 0) {
            animator.SetBool("moving", true);
            animator.SetFloat("moveX", playerMoveDirection.x);
            animator.SetFloat("moveY", playerMoveDirection.y);
            lastMoveDirection = playerMoveDirection;
        }

        if (immunityTimer > 0){
            immunityTimer -= Time.deltaTime;
        } else {
            isImmune = false;
        }

        if (wCooldownTimer > 0) wCooldownTimer -= Time.deltaTime;
        if (eCooldownTimer > 0) eCooldownTimer -= Time.deltaTime;
        if (rCooldownTimer > 0) rCooldownTimer -= Time.deltaTime;
        if (tCooldownTimer > 0) tCooldownTimer -= Time.deltaTime;
        if (spaceCooldownTimer > 0) spaceCooldownTimer -= Time.deltaTime;
        if (qAttackTimer > 0) qAttackTimer -= Time.deltaTime;

        // ===== CHỖ THÊM SỐ 2 =====
        if (uiController != null)
        {
            uiController.UpdateSkillCooldown(uiController.qSkill, qAttackTimer, qAttackResetTime);
            uiController.UpdateSkillCooldown(uiController.wSkill, wCooldownTimer, wCooldown);
            uiController.UpdateSkillCooldown(uiController.eSkill, eCooldownTimer, eCooldown);
            uiController.UpdateSkillCooldown(uiController.rSkill, rCooldownTimer, rCooldown);
            uiController.UpdateSkillCooldown(uiController.tSkill, tCooldownTimer, tCooldown);
            uiController.UpdateSkillCooldown(uiController.spaceSkill, spaceCooldownTimer, spaceCooldown);
        }
        // ===========================

        // Đánh thường (Q) - Không có cooldown, chỉ có reset timer
        if (Input.GetKeyDown(KeyCode.Q) && qAttackTimer <= 0)
        {
            PerformNormalAttack(); // Gọi hàm thực hiện đòn đánh
            qAttackTimer = qAttackResetTime; // Reset lại timer cho đòn đánh tiếp theo
        }

        // Kỹ năng W - Có cooldown
        if (Input.GetKeyDown(KeyCode.W) && wCooldownTimer <= 0)
        {
            PerformWSkill(); // Gọi hàm thực hiện kỹ năng W
            wCooldownTimer = wCooldown; // Bắt đầu đếm ngược cooldown
        }

        // Kỹ năng E - Có cooldown
        if (Input.GetKeyDown(KeyCode.E) && eCooldownTimer <= 0)
        {
            PerformESkill();
            eCooldownTimer = eCooldown;
        }

        // Kỹ năng R - Có cooldown
        if (Input.GetKeyDown(KeyCode.R) && rCooldownTimer <= 0)
        {
            PerformRSkill();
            rCooldownTimer = rCooldown;
        }

        // Ultimate (T) - Có cooldown
        if (Input.GetKeyDown(KeyCode.T) && tCooldownTimer <= 0)
        {
            PerformTSkill_Ultimate();
            tCooldownTimer = tCooldown;
        }

        // Lộn mèo/Lướt (Space) - Có cooldown
        if (Input.GetKeyDown(KeyCode.Space) && spaceCooldownTimer <= 0)
        {
            PerformDodge();
            spaceCooldownTimer = spaceCooldown;
        }
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
        if (experience >= playerLevels[currentLevel - 1]){
            LevelUp();
        }
    }

    public void LevelUp(){
        experience -= playerLevels[currentLevel - 1];
        currentLevel++;
        UIController.Instance.UpdateExperienceSlider();
        //UIController.Instance.levelUpButtons[0].ActivateButton(activeWeapon);

        upgradeableWeapons.Clear();

        if (activeWeapons.Count > 0){
            upgradeableWeapons.AddRange(activeWeapons);
        }
        if (inactiveWeapons.Count > 0){
            upgradeableWeapons.AddRange(inactiveWeapons);
        }
        for (int i = 0; i < UIController.Instance.levelUpButtons.Length; i++){
            if (upgradeableWeapons.ElementAtOrDefault(i) != null){
                UIController.Instance.levelUpButtons[i].ActivateButton(upgradeableWeapons[i]);
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(true);
            } else {
                UIController.Instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }

        UIController.Instance.LevelUpPanelOpen();
    }

    private void AddWeapon(int index){
        activeWeapons.Add(inactiveWeapons[index]);
        inactiveWeapons[index].gameObject.SetActive(true);
        inactiveWeapons.RemoveAt(index);
    }

    public void ActivateWeapon(Weapon weapon){
        weapon.gameObject.SetActive(true);
        activeWeapons.Add(weapon);
        inactiveWeapons.Remove(weapon);
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

 void PerformNormalAttack()
{
    Debug.Log("Bắn vũ khí bằng nút Q!");

    // Duyệt qua tất cả các vũ khí đang được trang bị
    foreach (Weapon weapon in activeWeapons)
    {
        // Nếu vũ khí là loại ProjectileWeapon mới của chúng ta
        if (weapon is ProjectileWeapon projectileWeapon)
        {
            // Gọi hàm Attack() của nó
            projectileWeapon.Attack();
        }
        
        // Bạn có thể thêm các loại vũ khí khác dùng nút Q ở đây
        // else if (weapon is AnotherQWeapon another) { ... }
    }
}
    void PerformWSkill()
    {
        Debug.Log("Sử dụng kỹ năng W!");
        // Logic cho kỹ năng W
    }

    void PerformESkill()
    {
        Debug.Log("Sử dụng kỹ năng E!");
        // Logic cho kỹ năng E
    }

    void PerformRSkill()
    {
        Debug.Log("Sử dụng kỹ năng R!");
        // Logic cho kỹ năng R
    }

    void PerformTSkill_Ultimate()
    {
        Debug.Log("Sử dụng ULTIMATE (T)!");
        // Logic cho kỹ năng Ultimate
    }

    void PerformDodge()
    {
        Debug.Log("Lộn mèo/Lướt (Space)!");
        // Logic cho việc di chuyển lướt.
        // Ví dụ: thay đổi nhanh vị trí của nhân vật bằng transform.position hoặc Rigidbody2D.velocity
    }
}