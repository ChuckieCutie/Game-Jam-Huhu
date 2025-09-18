using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider playerExperienceSlider;
    [SerializeField] private TMP_Text experienceText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject levelUpPanel;
    [SerializeField] private TMP_Text timerText;

    public LevelUpButton[] levelUpButtons;
     [Header("Effects")]
     [SerializeField] private Image damageFlashImage;
    [SerializeField] private float flashDuration = 0.15f;
    private Coroutine flashRoutine;

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(this);
        } else {
            Instance = this;
        }
    }

     public void ShowDamageFlash()
    {
        // Dừng coroutine cũ nếu nó đang chạy để bắt đầu một cái mới
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        // Bắt đầu với màu đỏ mờ
        Color startColor = new Color(1f, 0f, 0f, 0.4f); // Đỏ với 40% độ mờ
        damageFlashImage.color = startColor;

        float elapsedTime = 0f;
        
        // Vòng lặp để giảm độ mờ về 0
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Dùng unscaledDeltaTime để hiệu ứng vẫn chạy khi game pause
            float newAlpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / flashDuration);
            damageFlashImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null; // Chờ đến frame tiếp theo
        }

        // Đảm bảo nó hoàn toàn trong suốt khi kết thúc
        damageFlashImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
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
}
