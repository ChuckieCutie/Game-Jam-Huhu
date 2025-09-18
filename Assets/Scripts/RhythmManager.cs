using UnityEngine;
using UnityEngine.Events; // Cần thiết để sử dụng UnityEvent

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    [Header("Rhythm Settings")]
    [Tooltip("Số nhịp mỗi phút của bài hát hiện tại")]
    public float bpm = 120f;
    [Tooltip("Độ 'dễ' khi bấm vào nhịp. Số càng lớn càng dễ. 0.1 = 100ms trước và sau nhịp.")]
    [Range(0.05f, 0.3f)]
    public float timingWindow = 0.15f;

    [Header("Events")]
    [Tooltip("Sự kiện này sẽ được kích hoạt mỗi khi đến một nhịp mới")]
    public UnityEvent OnBeat;

    // --- Private Variables ---
    private float beatInterval;      // Khoảng thời gian giữa 2 nhịp (tính bằng giây)
    private float nextBeatTime;      // Mốc thời gian của nhịp tiếp theo
    private AudioSource musicSource; // Nguồn phát nhạc

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        if (musicSource != null && musicSource.clip != null)
        {
            // Bắt đầu tính toán nhịp điệu khi nhạc bắt đầu
            InitializeBeatTimer();
            musicSource.Play();
        }
        else
        {
            Debug.LogError("RhythmManager cần một AudioSource component với một AudioClip để hoạt động!");
        }
    }

    void Update()
    {
        // Kiểm tra nếu thời gian hiện tại đã vượt qua mốc nhịp tiếp theo
        if (Time.time >= nextBeatTime)
        {
            // Kích hoạt sự kiện OnBeat (để UI có thể lắng nghe và nhấp nháy)
            OnBeat?.Invoke();

            // Tính toán mốc thời gian cho nhịp kế tiếp
            nextBeatTime += beatInterval;
        }
    }

    /// <summary>
    /// Các script khác (như PlayerController) sẽ gọi hàm này để kiểm tra.
    /// </summary>
    /// <returns>Trả về true nếu người chơi bấm trong khoảng thời gian cho phép.</returns>
    public bool CheckTiming()
    {
        // Kiểm tra xem thời gian bấm phím có nằm trong "cửa sổ thời gian" của nhịp gần nhất không
        // Cửa sổ này bao gồm một chút trước và một chút sau nhịp
        return Mathf.Abs(Time.time - nextBeatTime) <= timingWindow || 
               Mathf.Abs(Time.time - (nextBeatTime - beatInterval)) <= timingWindow;
    }

    private void InitializeBeatTimer()
    {
        // Công thức tính khoảng cách giữa các nhịp từ BPM
        beatInterval = 60f / bpm;
        // Đặt mốc thời gian cho nhịp đầu tiên
        nextBeatTime = Time.time + beatInterval;
    }
}