using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;
    
    // <<< THAY ĐỔI: Chuyển sang dùng C# event để kết nối tự động
    public static event System.Action OnBeat;

    [Header("Rhythm Settings")]
    [Tooltip("Số nhịp mỗi phút của bài hát hiện tại")]
    public float bpm = 120f;

    [Tooltip("Độ 'dễ' khi bấm vào nhịp. 0.1 tương đương sai số 100ms.")]
    [Range(0.05f, 0.3f)]
    public float timingWindow = 0.1f;

    private float beatInterval;
    private float nextBeatTime;
    private bool isBeatTimerRunning = false;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        if (!isBeatTimerRunning) return;

        if (Time.unscaledTime >= nextBeatTime)
        {
            OnBeat?.Invoke(); // Gửi tín hiệu đi
            nextBeatTime += beatInterval;
        }
    }

    public bool CheckTiming()
    {
        return Mathf.Abs(Time.unscaledTime - nextBeatTime) <= timingWindow || 
               Mathf.Abs(Time.unscaledTime - (nextBeatTime - beatInterval)) <= timingWindow;
    }

    public void UpdateBPM(float newBpm)
    {
        bpm = newBpm;
        beatInterval = 60f / bpm;
        nextBeatTime = Time.unscaledTime + beatInterval;
        isBeatTimerRunning = true;
    }
}