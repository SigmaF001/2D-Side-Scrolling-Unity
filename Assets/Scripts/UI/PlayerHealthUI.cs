using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image    apFill;
    [SerializeField] private TMP_Text apText;

    [Header("Colors")]
    [SerializeField] private Color fullColor  = new Color(0.4f, 0.8f, 1f);
    [SerializeField] private Color lowColor   = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private float lowThreshold = 0.25f;

    private void Start()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.OnAPChanged += Refresh;
            Refresh(PlayerHealth.Instance.CurrentAP, PlayerHealth.Instance.MaxAP);
        }
    }

    private void OnDisable()
    {
        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.OnAPChanged -= Refresh;
    }

    private void Refresh(float current, float max)
    {
        float ratio = max > 0 ? current / max : 0f;
        if (apFill)
        {
            apFill.fillAmount = ratio;
            apFill.color = ratio <= lowThreshold ? lowColor : fullColor;
        }
        if (apText) apText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}
