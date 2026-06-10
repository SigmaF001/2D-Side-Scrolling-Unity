using UnityEngine;
using TMPro;
public class TimeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text periodText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text weekText;

    [Header("Period Colors")]
    [SerializeField] private Color morningColor   = new Color(1f, 0.95f, 0.6f);
    [SerializeField] private Color afternoonColor = new Color(0.6f, 0.9f, 1f);
    [SerializeField] private Color eveningColor   = new Color(0.5f, 0.4f, 0.9f);

    private void OnEnable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged += Refresh;
            Refresh(TimeManager.Instance.CurrentTime);
        }
    }
    
    void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged += Refresh;
            Refresh(TimeManager.Instance.CurrentTime);
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= Refresh;
    }

    private void Refresh(GameTimeStamp t)
    {
        if (periodText)
        {
            //เปลี่ยนสีข้อความตามช่วงเวลา
            periodText.text = t.PeriodName.ToUpper();
            periodText.color = t.Period switch
            {
                TimePeriod.Morning   => morningColor,
                TimePeriod.Afternoon => afternoonColor,
                TimePeriod.Evening   => eveningColor,
                _                    => Color.white //ค่า Default เผื่อเกิด Bug
            };
        }
        if (dayText)   dayText.text  = $"{t.DayName}  |  Day {t.TotalDays}";
        if (weekText)  weekText.text = $"Week {t.Week + 1}";
    }
}
