using UnityEngine;
using TMPro;

public class TimeHopTrigger : MonoBehaviour
{
    [Header("Cooldown")]
    [SerializeField] private float cooldown = 1f;

    [Header("Hud")]
    [SerializeField] private string interactPrompt = "Press E to rest";
    [SerializeField] private TextMeshPro hintText;

    private float _lastHopTime = -99f;

    private void Awake()
    {
        if (hintText == null) return;
        if (hintText != null) hintText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hintText != null) 
        {
            hintText.text = interactPrompt;
            hintText.gameObject.SetActive(true);
        }
        if (InputManager.Instance == null)
        {
            Debug.LogError("[TimeHopTrigger] InputManager.Instance is null!");
            return;
        }
        InputManager.Instance.OnInteractPressed += TryHop;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        InputManager.Instance.OnInteractPressed -= TryHop;
        if (hintText != null) hintText.gameObject.SetActive(false);
    }

    private void TryHop()
    {
        if (Time.time - _lastHopTime < cooldown) return; //เช็คคูลดาวน์
        _lastHopTime = Time.time; //บันทึกเวลาที่กดล่าสุด
        TimeManager.Instance?.AdvanceTime(); //เลื่อนเวลา
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
    }
}
