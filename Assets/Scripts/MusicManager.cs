using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    [Tooltip("Dùng để phát các bài nhạc chính")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("Dùng để phát các hiệu ứng, đoạn intro ngắn")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Phase 1 Music")]
    [SerializeField] private AudioClip phase1Music;
    [SerializeField] private float phase1BPM = 120f;

    [Header("Phase 2 Music (Boss)")]
    [SerializeField] private AudioClip bossIntro;
    [SerializeField] private float bossIntroBPM = 130f;
    [SerializeField] private AudioClip bossFightStart;
    [SerializeField] private AudioClip bossFightLoop;
    [SerializeField] private float bossFightBPM = 150f;
    [SerializeField] private AudioClip bossOutro;
    [SerializeField] private float bossOutroBPM = 100f;

    private bool isBossDefeated = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        StartPhase1();
    }

    public void StartPhase1()
    {
        RhythmManager.Instance.UpdateBPM(phase1BPM);
        musicSource.clip = phase1Music;
        musicSource.loop = true;
        musicSource.Play();
    }
    
    public void StartPhase2Transition()
    {
        StartCoroutine(Phase2Sequence());
    }
    
    public void OnBossDefeated()
    {
        isBossDefeated = true;
    }

    private IEnumerator Phase2Sequence()
    {
        // 1. Dừng nhạc cũ và bắt đầu đoạn Intro
        musicSource.Stop();
        RhythmManager.Instance.UpdateBPM(bossIntroBPM);
        musicSource.clip = bossIntro;
        musicSource.loop = false;
        musicSource.Play();
        
        // Chờ cho đoạn intro phát xong
        // Giả sử đoạn intro dài 8s, bạn có thể thay đổi số này
        yield return new WaitForSeconds(8f); 
        
        // 2. Phát đoạn "Bắt đầu đánh boss"
        sfxSource.PlayOneShot(bossFightStart);
        
        // Cập nhật BPM cho đoạn loop sắp tới
        RhythmManager.Instance.UpdateBPM(bossFightBPM); 
        
        // 3. Bắt đầu lặp lại đoạn nhạc đánh boss
        musicSource.clip = bossFightLoop;
        musicSource.loop = true;
        musicSource.Play();

        // 4. Chờ cho đến khi boss bị đánh bại
        while (!isBossDefeated)
        {
            yield return null;
        }

        // 5. Boss đã chết, không lặp lại nhạc nữa
        musicSource.loop = false;
        
        // Chờ cho lần lặp cuối cùng của bài hát kết thúc
        yield return new WaitWhile(() => musicSource.isPlaying);
        
        // 6. Phát đoạn Outro
        Debug.Log("Playing Outro music...");
        RhythmManager.Instance.UpdateBPM(bossOutroBPM);
        musicSource.clip = bossOutro;
        musicSource.loop = false;
        musicSource.Play();
    }
}