using UnityEngine;

public class TimeHopTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If true, each entry advances time. If false, requires pressing the interact key.")]
    [SerializeField] private bool autoHopOnEnter = true;
    [SerializeField] private KeyCode interactKey  = KeyCode.E;

    [Header("Cooldown (prevent rapid re-triggers)")]
    [SerializeField] private float cooldown = 1f;

    private bool _playerInside;
    private float _lastHopTime = -99f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        if (autoHopOnEnter) TryHop();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerInside = false;
    }

    private void Update()
    {
        if (!autoHopOnEnter && _playerInside && Input.GetKeyDown(interactKey))
            TryHop();
    }

    private void TryHop()
    {
        if (Time.time - _lastHopTime < cooldown) return; //เช็คคูลดาวน์
        _lastHopTime = Time.time; //บันทึกเวลาที่กดล่าสุด
        TimeManager.Instance?.AdvanceTime(); //เลื่อนเวลา
    }

    // Draw trigger gizmo in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        var col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
    }
}
