using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    private static TransitionUI _instance;
    [SerializeField] private CanvasGroup _group;

    private void Awake()
    {
        _instance = this;
        var panel = GameObject.FindGameObjectWithTag("TransitionPanel");
        if (panel != null)
            _group = panel.GetComponent<CanvasGroup>();
    }

    public static IEnumerator FadeOut(float duration)
    {
        if (_instance == null || _instance._group == null) yield break;
        yield return _instance.Fade(0f, 1f, duration);
    }

    public static IEnumerator FadeIn(float duration)
    {
        if (_instance == null || _instance._group == null) yield break;
        yield return _instance.Fade(1f, 0f, duration);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        _group.alpha = from;
        while (t < duration)
        {
            t += Time.deltaTime;
            _group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        _group.alpha = to;
    }
}