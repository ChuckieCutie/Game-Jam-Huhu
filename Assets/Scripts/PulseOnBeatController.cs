using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PulseOnBeatController : MonoBehaviour
{
    [Header("Beat Pulse Effect")]
    [Tooltip("Danh sách các GameObject sẽ thực hiện hiệu ứng đập.")]
    public List<GameObject> visualsToPulse = new List<GameObject>();

    [Tooltip("Mức độ phình to khi đập.")]
    public float pulseScaleAmount = 1.05f;

    [Tooltip("Thời gian hoàn thành một lần đập (phình to rồi co lại).")]
    public float pulseDuration = 0.1f;

    private List<Vector3> _initialScales = new List<Vector3>();
    private Coroutine _activePulseCoroutine = null;

    // <<< THÊM MỚI: Tự động kết nối khi script được bật
    void OnEnable()
    {
        RhythmManager.OnBeat += TriggerPulseOnBeat;
    }

    // <<< THÊM MỚI: Tự động ngắt kết nối để tránh lỗi
    void OnDisable()
    {
        RhythmManager.OnBeat -= TriggerPulseOnBeat;
    }

    void Awake()
    {
        if (visualsToPulse.Count == 0)
        {
            visualsToPulse.Add(this.gameObject);
        }

        _initialScales.Clear();
        foreach (GameObject visual in visualsToPulse)
        {
            if (visual != null)
            {
                _initialScales.Add(visual.transform.localScale);
            }
        }
    }

    // Hàm này giờ sẽ được gọi tự động bởi RhythmManager
    public void TriggerPulseOnBeat()
    {
        if (!this.gameObject.activeInHierarchy) return;

        if (_activePulseCoroutine != null)
        {
            StopCoroutine(_activePulseCoroutine);
        }
        _activePulseCoroutine = StartCoroutine(PulseAllCoroutine());
    }

    private IEnumerator PulseAllCoroutine()
    {
        float halfDuration = pulseDuration / 2f;
        if (halfDuration <= 0f) yield break;

        List<Vector3> targetScales = new List<Vector3>();
        for (int i = 0; i < visualsToPulse.Count; i++)
        {
            targetScales.Add(_initialScales[i] * pulseScaleAmount);
        }

        float timer = 0f;

        // Giai đoạn phình to
        while (timer < halfDuration)
        {
            for (int i = 0; i < visualsToPulse.Count; i++)
            {
                visualsToPulse[i].transform.localScale = Vector3.Lerp(_initialScales[i], targetScales[i], timer / halfDuration);
            }
            // <<< THAY ĐỔI: Dùng unscaledDeltaTime để không bị ảnh hưởng bởi pause
            timer += Time.unscaledDeltaTime; 
            yield return null;
        }

        // Giai đoạn co lại
        timer = 0f;
        while (timer < halfDuration)
        {
            for (int i = 0; i < visualsToPulse.Count; i++)
            {
                visualsToPulse[i].transform.localScale = Vector3.Lerp(targetScales[i], _initialScales[i], timer / halfDuration);
            }
            // <<< THAY ĐỔI: Dùng unscaledDeltaTime
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        for (int i = 0; i < visualsToPulse.Count; i++)
        {
            visualsToPulse[i].transform.localScale = _initialScales[i];
        }
        _activePulseCoroutine = null;
    }
}