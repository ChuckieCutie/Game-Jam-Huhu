using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Slider playerExperienceSlider;
    [SerializeField] private TMP_Text experienceText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;
    [SerializeField] private TMP_Text timerText;

    void Awake(){
    if ( Instance != null && Instance != this){
        Destroy(this);
    } else {
        Instance = this;
    }
   }

   public void UpdateHeathSlider(){
    playerHealthSlider.maxValue = PlayerController.Instance.playerMaxHealth;
    playerHealthSlider.value = PlayerController.Instance.playerHealth;
    healthText.text = playerHealthSlider.value + "/" + playerHealthSlider.maxValue;
   }

   public void UpdateExperienceSlider(){
    playerExperienceSlider.maxValue = PlayerController.Instance.playerLevels[PlayerController.Instance.currentLevel -1];
    playerExperienceSlider.value = PlayerController.Instance.experience;
    experienceText.text = playerExperienceSlider.value + "/" + playerExperienceSlider.maxValue;
   }

   public void UpdateTimer(float timer){
    float min = Mathf.Floor(timer / 60f);
    float sec = Mathf.Floor(timer % 60f);
    timerText.text = min + ":" + sec.ToString("00");
   }
}
