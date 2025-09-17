using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // KHÔNG CÓ DÒNG "public static UIController Instance;" thừa ở đây nữa

    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider playerExperienceSlider;
    [SerializeField] private TMP_Text experienceText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject levelUpPanel;
    [SerializeField] private TMP_Text timerText;

    public LevelUpButton[] levelUpButtons;

    private static UIController _instance;
    public static UIController Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<UIController>();
            }
            return _instance;
        }
    }

    void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public void UpdateHealthSlider(){
        playerHealthSlider.maxValue = PlayerController.Instance.playerMaxHealth;
        playerHealthSlider.value = PlayerController.Instance.playerHealth;
        healthText.text = playerHealthSlider.value + " / " + playerHealthSlider.maxValue;
    }

    public void UpdateExperienceSlider(){
        playerExperienceSlider.maxValue = PlayerController.Instance.playerLevels[PlayerController.Instance.currentLevel - 1];
        playerExperienceSlider.value = PlayerController.Instance.experience;
        experienceText.text = playerExperienceSlider.value + " / " + playerExperienceSlider.maxValue;
    }

    public void UpdateTimer(float timer){
        float min = Mathf.FloorToInt(timer / 60f);
        float sec = Mathf.FloorToInt(timer % 60f);

        timerText.text = min + ":" + sec.ToString("00");
    }

    public void LevelUpPanelOpen(){
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LevelUpPanelClose(){
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

     [System.Serializable]
    public struct SkillUI
    {
        public Slider cooldownSlider; // Thay Image bằng Slider
        public TextMeshProUGUI cooldownText;
    }

    public SkillUI qSkill;
    public SkillUI wSkill;
    public SkillUI eSkill;
    public SkillUI rSkill;
    public SkillUI tSkill;
    public SkillUI spaceSkill;

    // Hàm này sẽ được PlayerController gọi mỗi frame để cập nhật một kỹ năng
    public void UpdateSkillCooldown(SkillUI skill, float cooldownTimer, float maxCooldown)
    {
        if (cooldownTimer > 0)
        {
            // Nếu đang cooldown thì bật Slider và Text lên
            skill.cooldownSlider.gameObject.SetActive(true);
            skill.cooldownText.gameObject.SetActive(true);

            // Tính toán và cập nhật giá trị của slider
            // Giá trị sẽ đi từ 1 về 0 khi đang hồi chiêu
            skill.cooldownSlider.value = cooldownTimer / maxCooldown;
            
            // Hiển thị số giây còn lại
            skill.cooldownText.text = cooldownTimer.ToString("F1");
        }
        else
        {
            // Nếu hết cooldown thì tắt chúng đi
            skill.cooldownSlider.gameObject.SetActive(false);
            skill.cooldownText.gameObject.SetActive(false);
        }
    }
}