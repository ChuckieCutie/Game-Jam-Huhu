using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public AreaWeapon areaWeapon; 
    public DirectionalWeapon directionalWeapon; 
    public SpinWeapon spinWeapon; 

    private float areaWeaponCooldown;
    private float directionalWeaponCooldown;
    private float spinWeaponCooldown;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [SerializeField] private float moveSpeed;
    public Vector3 playerMoveDirection;
    public Vector3 lastMoveDirection;
    public float playerMaxHealth;
    public float playerHealth;

    public int experience;
    public int currentLevel;
    public int maxLevel;

    private Vector3 targetPosition;

    [SerializeField] private List<Weapon> inactiveWeapons;
    public List<Weapon> activeWeapons;
    [SerializeField] private List<Weapon> upgradeableWeapons;
    public List<Weapon> maxLevelWeapons;

    private bool isImmune;
    [SerializeField] private float immunityDuration;
    [SerializeField] private float immunityTimer;

    public List<int> playerLevels;
    
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
        targetPosition = transform.position;

        if (directionalWeapon != null && !activeWeapons.Contains(directionalWeapon)){
            activeWeapons.Add(directionalWeapon);
            directionalWeapon.gameObject.SetActive(true);

        if (inactiveWeapons.Contains(directionalWeapon)){
            inactiveWeapons.Remove(directionalWeapon);}}
    }

    void Update()
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
        
        if (immunityTimer > 0){
            immunityTimer -= Time.deltaTime;
        } else {
            isImmune = false;
        }

        if (areaWeaponCooldown > 0) areaWeaponCooldown -= Time.deltaTime;
        if (directionalWeaponCooldown > 0) directionalWeaponCooldown -= Time.deltaTime;
        if (spinWeaponCooldown > 0) spinWeaponCooldown -= Time.deltaTime;

        // Phím w cho Area Weapon
        if (Input.GetKeyDown(KeyCode.W) && areaWeapon != null && areaWeaponCooldown <= 0)
        {
            areaWeapon.ActivateWeapon();
            areaWeaponCooldown = areaWeapon.stats[areaWeapon.weaponLevel].cooldown; // Đặt lại thời gian hồi chiêu
        }

        // Phím W cho Directional Weapon
        if (Input.GetKeyDown(KeyCode.Q) && directionalWeapon != null && directionalWeaponCooldown <= 0)
        {
            directionalWeapon.ActivateWeapon();
            directionalWeaponCooldown = directionalWeapon.stats[directionalWeapon.weaponLevel].cooldown;
        }

        // Phím E cho Spin Weapon
        if (Input.GetKeyDown(KeyCode.E) && spinWeapon != null && spinWeaponCooldown <= 0)
        {
            spinWeapon.ActivateWeapon();
            spinWeaponCooldown = spinWeapon.stats[spinWeapon.weaponLevel].cooldown;
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
}