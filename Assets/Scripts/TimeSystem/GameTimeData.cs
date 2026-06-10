using System;
using UnityEngine;

public enum TimePeriod { Morning = 0, Afternoon = 1, Evening = 2 }

public enum GameDay { Monday = 0, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

[Serializable]
public struct GameTimeStamp
{
    public int TotalDays;
    public TimePeriod Period;
    public GameDay DayOfWeek => (GameDay)(TotalDays % 7);
    public int Week => TotalDays / 7;
    public string PeriodName => Period.ToString();
    public string DayName => DayOfWeek.ToString();

    public override string ToString() =>
        $"Day {TotalDays} ({DayName}) – {PeriodName}  (Week {Week + 1})";

    //เลื่อนไปช่วงเวลาถัดไป และส่งค่า true ถ้าเป็นวันใหม่
    public bool Advance()
    {
        int next = (int)Period + 1;
        if (next > (int)TimePeriod.Evening)
        {
            Period = TimePeriod.Morning;
            TotalDays++;
            return true;
        }
        Period = (TimePeriod)next;
        return false;
    }
}
