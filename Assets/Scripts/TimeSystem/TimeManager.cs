using System;
using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    public event Action<GameTimeStamp> OnTimeChanged;
    public event Action<GameTimeStamp> OnNewDay;

    [Header("Starting Time")]
    [SerializeField] private int startDay = 0;
    [SerializeField] private TimePeriod startPeriod = TimePeriod.Morning;

    public GameTimeStamp CurrentTime { get; private set; }

    [Header("Transition (optional)")]
    [Tooltip("Duration of the fade-to-black screen transition in seconds.")]
    [SerializeField] private float transitionDuration = 0.5f;
    private bool _isTransitioning;

#region Unity Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentTime = new GameTimeStamp
        {
            TotalDays = startDay,
            Period    = startPeriod
        };
    }
#endregion

#region Public API

    public void AdvanceTime()
    {
        if (_isTransitioning) return;
        StartCoroutine(DoAdvance());
    }

    public void SetTime(int totalDays, TimePeriod period)
    {
        CurrentTime = new GameTimeStamp { TotalDays = totalDays, Period = period };
        BroadcastChange(false);
    }
#endregion

#region Internal Logic
    private IEnumerator DoAdvance()
    {
        _isTransitioning = true;

        yield return StartCoroutine(TransitionUI.FadeOut(transitionDuration));

        var stamp = CurrentTime;
        bool newDay = stamp.Advance();
        CurrentTime = stamp;

        BroadcastChange(newDay);

        yield return StartCoroutine(TransitionUI.FadeIn(transitionDuration));

        _isTransitioning = false;
    }

    private void BroadcastChange(bool isNewDay)
    {
        OnTimeChanged?.Invoke(CurrentTime);
        if (isNewDay) OnNewDay?.Invoke(CurrentTime);

        Debug.Log($"[TimeManager] {CurrentTime}");
    }
}
#endregion