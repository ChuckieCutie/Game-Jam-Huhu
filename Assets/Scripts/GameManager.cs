using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float gameTimer;
    public bool gameActive;

    void Awake(){
    if ( Instance != null && Instance != this){
        Destroy(this);
    } else {
        Instance = this;
    }
   }

   void Start(){
    gameActive = true;
   }

   void Update(){
    if ( gameActive){
        UIController.Instance.UpdateTimer(gameTimer);   
        gameTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)){
            Pause();
        }
    }
   }

   public void GameOver(){
    gameActive = false;
    StartCoroutine(ShowGameOverScreen());
   }

   IEnumerator ShowGameOverScreen(){
    yield return new WaitForSeconds(1.5f);
    UIController.Instance.gameOverPanel.SetActive(true);
   }

   public void RestartGame(){
    SceneManager.LoadScene("Game");
   }

   public void Pause(){
    if (UIController.Instance.pausePanel.activeSelf == false && UIController.Instance.gameOverPanel.activeSelf == false ){
        UIController.Instance.pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }else{
        UIController.Instance.pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
   }

   public void QuitGame(){
    Application.Quit();
   }

   public void GotoMainMenu(){
    SceneManager.LoadScene("Main Menu");
   }
}
