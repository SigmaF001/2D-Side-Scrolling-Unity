using UnityEngine;

public class CombatAreaTrigger : MonoBehaviour
{
    [Header("Reset AP on exit?")]
    [SerializeField] private bool resetOnExit = true;
    [SerializeField] private bool resetOnEnter = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (resetOnEnter)
        {
            Debug.Log("[CombatArea] Player entered, resetting AP.");
            PlayerHealth.Instance?.ResetAP();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (resetOnExit)
        {
            Debug.Log("[CombatArea] Player exited, resetting AP.");
            PlayerHealth.Instance?.ResetAP();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.2f);
        var c = GetComponent<Collider2D>();
        if (c is BoxCollider2D box)
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
    }
}